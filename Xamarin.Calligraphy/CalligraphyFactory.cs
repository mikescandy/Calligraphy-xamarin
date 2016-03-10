using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Exception = System.Exception;
using Object = Java.Lang.Object;
using String = System.String;

namespace Calligraphy
{
    class CalligraphyFactory : Java.Lang.Object
    {
        private readonly LayoutInflater.IFactory factory;
        private static string ACTION_BAR_TITLE = "action_bar_title";
        private static string ACTION_BAR_SUBTITLE = "action_bar_subtitle";
        private int[] mAttributeId;




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
                styleIds[0] = CalligraphyConfig.Get().getClassStyles().ContainsKey(view.GetType())
                        ? CalligraphyConfig.Get().getClassStyles()[view.GetType()]
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
            if (MatchesResourceIdName(view, ACTION_BAR_TITLE)) return true;
            if (parentIsToolbarV7(view))
            {
                Android.Support.V7.Widget.Toolbar parent = (Android.Support.V7.Widget.Toolbar)view.Parent;
                return TextUtils.Equals(parent.Title, view.Text);
            }
            return false;
        }

        /// <summary>
        ///  An even dirtier way to see if the TextView is part of the ActionBar
        /// </summary>
        /// <param name="view">TextView to check is Title.</param>
        /// <returns>true if it is.</returns>
        protected static bool IsActionBarSubTitle(TextView view)
        {
            if (MatchesResourceIdName(view, ACTION_BAR_SUBTITLE)) return true;
            if (parentIsToolbarV7(view))
            {
                Android.Support.V7.Widget.Toolbar parent = (Android.Support.V7.Widget.Toolbar)view.Parent;
                return TextUtils.Equals(parent.Subtitle, view.Text);
            }
            return false;
        }

        protected static bool parentIsToolbarV7(View view)
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
            this.mAttributeId = new int[] { attributeId };
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
            if (view != null && (bool)view.GetTag(Resource.Id.calligraphy_tag_id) != true)
            {
                onViewCreatedInternal(view, context, attrs);
                view.SetTag(Resource.Id.calligraphy_tag_id, true);
            }
            return view;
        }

        void onViewCreatedInternal(View view, Context context, IAttributeSet attrs)
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
                String textViewFont = resolveFontPath(context, attrs);

                // Try theme attributes
                if (TextUtils.IsEmpty(textViewFont))
                {
                    int[] styleForTextView = GetStyleForTextView((TextView)view);
                    if (styleForTextView[1] != -1)
                        textViewFont = CalligraphyUtils.PullFontPathFromTheme(context, styleForTextView[0], styleForTextView[1], mAttributeId);
                    else
                        textViewFont = CalligraphyUtils.PullFontPathFromTheme(context, styleForTextView[0], mAttributeId);
                }

                // Still need to defer the Native action bar, appcompat-v7:21+ uses the Toolbar underneath. But won't match these anyway.
                bool deferred = MatchesResourceIdName(view, ACTION_BAR_TITLE) || MatchesResourceIdName(view, ACTION_BAR_SUBTITLE);

                CalligraphyUtils.ApplyFontToTextView(context, (TextView)view, CalligraphyConfig.Get(), textViewFont, deferred);
            }

            // AppCompat API21+ The ActionBar doesn't inflate default Title/SubTitle, we need to scan the
            // Toolbar(Which underlies the ActionBar) for its children.
            if (CalligraphyUtils.CanCheckForV7Toolbar() && view.GetType() == typeof(Android.Support.V7.Widget.Toolbar))
            {
                Toolbar toolbar = (Toolbar)view;
                toolbar.ViewTreeObserver.AddOnGlobalLayoutListener(new ToolbarLayoutListener(this, context, toolbar));
            }

            // Try to set typeface for custom views using interface method or via reflection if available
            if (view.GetType() == typeof(IHasTypeFace))
            {
                Typeface typeface = getDefaultTypeface(context, resolveFontPath(context, attrs));
                if (typeface != null)
                {
                    ((IHasTypeFace)view).setTypeface(typeface);
                }
            }
            else if (CalligraphyConfig.Get().isCustomViewTypefaceSupport() && CalligraphyConfig.Get().isCustomViewHasTypeface(view))
            {
                var setTypeface = ReflectionUtils.getMethod(view.GetType(), "setTypeface");
                string fontPath = resolveFontPath(context, attrs);
                Typeface typeface = getDefaultTypeface(context, fontPath);
                if (setTypeface != null && typeface != null)
                {
                    ReflectionUtils.invokeMethod(view, setTypeface, new object[] { typeface });
                }
            }
        }

        private Typeface getDefaultTypeface(Context context, String fontPath)
        {
            if (TextUtils.IsEmpty(fontPath))
            {
                fontPath = CalligraphyConfig.Get().getFontPath();
            }
            if (!TextUtils.IsEmpty(fontPath))
            {
                return TypefaceUtils.Load(context.Assets, fontPath);
            }
            return null;
        }

        /**
         * Resolving font path from xml attrs, style attrs or text appearance
         */
        private String resolveFontPath(Context context, IAttributeSet attrs)
        {
            // Try view xml attributes
            String textViewFont = CalligraphyUtils.PullFontPathFromView(context, attrs, mAttributeId);

            // Try view style attributes
            if (TextUtils.IsEmpty(textViewFont))
            {
                textViewFont = CalligraphyUtils.PullFontPathFromStyle(context, attrs, mAttributeId);
            }

            // Try View TextAppearance
            if (TextUtils.IsEmpty(textViewFont))
            {
                textViewFont = CalligraphyUtils.PullFontPathFromTextAppearance(context, attrs, mAttributeId);
            }

            return textViewFont;
        }

        protected class ToolbarLayoutListener : Object, ViewTreeObserver.IOnGlobalLayoutListener
        {

            static string BLANK = " ";

            private WeakReference<CalligraphyFactory> mCalligraphyFactory;
            private WeakReference<Context> mContextRef;
            private WeakReference<Toolbar> mToolbarReference;
            private string originalSubTitle;

            public ToolbarLayoutListener(CalligraphyFactory calligraphyFactory,
                                       Context context, Toolbar toolbar)
            {
                mCalligraphyFactory = new WeakReference<CalligraphyFactory>(calligraphyFactory);
                mContextRef = new WeakReference<Context>(context);
                mToolbarReference = new WeakReference<Toolbar>(toolbar);
                originalSubTitle = toolbar.Subtitle;
                toolbar.Subtitle = BLANK;
            }

            //   @TargetApi(Build.VERSION_CODES.JELLY_BEAN)
            public void OnGlobalLayout()
            {
                Toolbar toolbar;
                mToolbarReference.TryGetTarget(out toolbar);
                Context context;
                mContextRef.TryGetTarget(out context);

                CalligraphyFactory factory;
                mCalligraphyFactory.TryGetTarget(out factory);
                if (toolbar == null) return;
                if (factory == null || context == null)
                {
                    removeSelf(toolbar);
                    return;
                }

                int childCount = toolbar.ChildCount;
                if (childCount != 0)
                {
                    // Process children, defer draw as it has set the typeface.
                    for (int i = 0; i < childCount; i++)
                    {
                        factory.OnViewCreated(toolbar.GetChildAt(i), context, null);
                    }
                }
                removeSelf(toolbar);
                toolbar.Subtitle = originalSubTitle;
            }

            private void removeSelf(Toolbar toolbar)
            {// Our dark deed is done
                if (Build.VERSION.SdkInt < Build.VERSION_CODES.JellyBean)
                {
                    //noinspection deprecation
                    toolbar.ViewTreeObserver.RemoveGlobalOnLayoutListener(this);
                }
                else {
                    toolbar.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
                }
            }

        }
    }
}