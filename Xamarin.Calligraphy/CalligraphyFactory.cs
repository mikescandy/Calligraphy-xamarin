using System;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;

using Exception = System.Exception;

namespace Calligraphy
{
    public class CalligraphyFactory : Java.Lang.Object
    {
        private readonly LayoutInflater.IFactory _factory;
        private static readonly string ActionBarTitle = "action_bar_title";
        private static readonly string ActionBarSubtitle = "action_bar_subtitle";
        private readonly int[] _attributeId;

        public CalligraphyFactory(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        /// <summary>
        /// Some styles are in sub styles, such as actionBarTextStyle etc..
        /// </summary>
        /// <param name="view">view to check.</param>
        /// <returns>2 element array, default to -1 unless a style has been found.</returns>
        protected static int[] GetStyleForTextView(TextView view)
        {
            var styleIds = new[] { -1, -1 };
            // Try to find the specific actionbar styles
            if (IsActionBarTitle(view))
            {
                styleIds[0] = Android.Resource.Attribute.ActionBarStyle;
                styleIds[1] = Android.Resource.Attribute.TitleTextStyle;
            }
            else if (IsActionBarSubTitle(view))
            {
                styleIds[0] = Android.Resource.Attribute.ActionBarStyle;
                styleIds[1] = Android.Resource.Attribute.SubtitleTextStyle;
            }
            if (styleIds[0] == -1)
            {
                // Use TextAppearance as default style
                styleIds[0] = CalligraphyConfig.Get().ClassStyleAttributeMap.ContainsKey(view.GetType())
                        ? CalligraphyConfig.Get().ClassStyleAttributeMap[view.GetType()]
                        : Android.Resource.Attribute.TextAppearance;
            }
            return styleIds;
        }

        /// <summary>
        /// An even dirtier way to see if the TextView is part of the ActionBar
        /// </summary>
        /// <param name="view">TextView to check is Title.</param>
        /// <returns>true if it is.</returns>
        protected static bool IsActionBarTitle(TextView view)
        {
            if (MatchesResourceIdName(view, ActionBarTitle)) return true;
            if (!ParentIsToolbarV7(view)) return false;
            var parent = (Android.Support.V7.Widget.Toolbar)view.Parent;
            return TextUtils.Equals(parent.Title, view.Text);
        }

        /// <summary>
        ///  An even dirtier way to see if the TextView is part of the ActionBar
        /// </summary>
        /// <param name="view">TextView to check is Title.</param>
        /// <returns>true if it is.</returns>
        protected static bool IsActionBarSubTitle(TextView view)
        {
            if (MatchesResourceIdName(view, ActionBarSubtitle)) return true;
            if (!ParentIsToolbarV7(view)) return false;
            var parent = (Android.Support.V7.Widget.Toolbar)view.Parent;
            return TextUtils.Equals(parent.Subtitle, view.Text);
        }

        protected static bool ParentIsToolbarV7(View view)
        {
            return CalligraphyUtils.CanCheckForV7Toolbar() && view.Parent != null && (view.Parent is Android.Support.V7.Widget.Toolbar);
        }

        /// <summary>
        /// Use to match a view against a potential view id. Such as ActionBar title etc.
        /// </summary>
        /// <param name="view">not null view you want to see has resource matching name.</param>
        /// <param name="matches">not null resource name to match against. Its not case sensitive.</param>
        /// <returns>true if matches false otherwise.</returns>
        protected static bool MatchesResourceIdName(View view, string matches)
        {
            if (view.Id == View.NoId) return false;
            var resourceEntryName = view.Resources.GetResourceEntryName(view.Id);
            return resourceEntryName.Equals(matches, StringComparison.InvariantCultureIgnoreCase);
        }

        public CalligraphyFactory(int attributeId)
        {
            _attributeId = new[] { attributeId };
        }

        protected View CreateViewOrFailQuietly(string name, string prefix, Context context, IAttributeSet attrs)
        {
            try
            {
                return LayoutInflater.From(context).CreateView(name, prefix, attrs);
            }
            catch (Exception ignore)
            {
                Log.Error("Calligraphy", ignore.Message);
                return null;
            }
        }

        /**
   * Handle the created view
   *
   * @param view    nullable.
   * @param context shouldn't be null.
   * @param attrs   shouldn't be null.
   * @return null if null is passed in.
   */

        public View OnViewCreated(View view, Context context, IAttributeSet attrs)
        {
            if (view == null || (bool)view.GetTag(Resource.Id.calligraphy_tag_id)) return view;
            OnViewCreatedInternal(view, context, attrs);
            view.SetTag(Resource.Id.calligraphy_tag_id, true);
            return view;
        }

        private void OnViewCreatedInternal(View view, Context context, IAttributeSet attrs)
        {
            if (view.GetType() == typeof(TextView))
            {
                // Fast path the setting of TextView's font, means if we do some delayed setting of font,
                // which has already been set by use we skip this TextView (mainly for inflating custom,
                // TextView's inside the Toolbar/ActionBar).
                if (TypefaceUtils.IsLoaded(((TextView)view).Typeface))
                {
                    return;
                }
                // Try to get typeface attribute value
                // Since we're not using namespace it's a little bit tricky

                // Check xml attrs, style attrs and text appearance for font path
                var textViewFont = ResolveFontPath(context, attrs);

                // Try theme attributes
                if (TextUtils.IsEmpty(textViewFont))
                {
                    var styleForTextView = GetStyleForTextView((TextView)view);
                    if (styleForTextView[1] != -1)
                        textViewFont = CalligraphyUtils.PullFontPathFromTheme(context, styleForTextView[0], styleForTextView[1], _attributeId);
                    else
                        textViewFont = CalligraphyUtils.PullFontPathFromTheme(context, styleForTextView[0], _attributeId);
                }

                // Still need to defer the Native action bar, appcompat-v7:21+ uses the Toolbar underneath. But won't match these anyway.
                var deferred = MatchesResourceIdName(view, ActionBarTitle) || MatchesResourceIdName(view, ActionBarSubtitle);

                CalligraphyUtils.ApplyFontToTextView(context, (TextView)view, CalligraphyConfig.Get(), textViewFont, deferred);
            }

            // AppCompat API21+ The ActionBar doesn't inflate default Title/SubTitle, we need to scan the
            // Toolbar(Which underlies the ActionBar) for its children.
            if (CalligraphyUtils.CanCheckForV7Toolbar() && view.GetType() == typeof(Android.Support.V7.Widget.Toolbar))
            {
                var toolbar = (Toolbar)view;
                toolbar.ViewTreeObserver.AddOnGlobalLayoutListener(new ToolbarLayoutListener(this, context, toolbar));
            }

            // Try to set typeface for custom views using interface method or via reflection if available
            if (view.GetType() == typeof(IHasTypeFace))
            {
                var typeface = getDefaultTypeface(context, ResolveFontPath(context, attrs));
                if (typeface != null)
                {
                    ((IHasTypeFace)view).SetTypeface(typeface);
                }
            }
            else if (CalligraphyConfig.Get().IsCustomViewTypefaceSupport && CalligraphyConfig.Get().IsCustomViewHasTypeface(view))
            {
                var setTypeface = ReflectionUtils.GetMethod(view.GetType(), "setTypeface");
                var fontPath = ResolveFontPath(context, attrs);
                var typeface = getDefaultTypeface(context, fontPath);
                if (setTypeface != null && typeface != null)
                {
                    ReflectionUtils.InvokeMethod(view, setTypeface, new object[] { typeface });
                }
            }
        }

        private Typeface getDefaultTypeface(Context context, string fontPath)
        {
            if (string.IsNullOrEmpty(fontPath))
            {
                fontPath = CalligraphyConfig.Get().FontPath;
            }
            if (!string.IsNullOrEmpty(fontPath))
            {
                return TypefaceUtils.Load(context.Assets, fontPath);
            }
            return null;
        }

        /**
         * Resolving font path from xml attrs, style attrs or text appearance
         */
        private string ResolveFontPath(Context context, IAttributeSet attrs)
        {
            // Try view xml attributes
            var textViewFont = CalligraphyUtils.PullFontPathFromView(context, attrs, _attributeId);

            // Try view style attributes
            if (string.IsNullOrEmpty(textViewFont))
            {
                textViewFont = CalligraphyUtils.PullFontPathFromStyle(context, attrs, _attributeId);
            }

            // Try View TextAppearance
            if (string.IsNullOrEmpty(textViewFont))
            {
                textViewFont = CalligraphyUtils.PullFontPathFromTextAppearance(context, attrs, _attributeId);
            }

            return textViewFont;
        }
    }
}