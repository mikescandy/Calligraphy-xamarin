using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Calligraphy
{
    class CalligraphyFactory : Java.Lang.Object, LayoutInflater.IFactory
    {
        private readonly LayoutInflater.IFactory factory;
        private readonly int attributeId;
        private static readonly string[] ClassPrefixList = {
            "android.widget.",
            "android.webkit."
    };
        private const string ActionBarTitle = "action_bar_title";
        private const string ActionBarSubtitle = "action_bar_subtitle";
        private static readonly Dictionary<Type, int> Styles
                = new Dictionary<Type, int>() {
                    {typeof(TextView), Android.Resource.Attribute.TextViewStyle},
                    {typeof(Button), Android.Resource.Attribute.ButtonStyle},
                    {typeof(EditText), Android.Resource.Attribute.EditTextStyle},
                    {typeof(AutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle},
                    { typeof(MultiAutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle },
                    { typeof(CheckBox), Android.Resource.Attribute.CheckboxStyle },
                    { typeof(RadioButton), Android.Resource.Attribute.RadioButtonStyle },
                    { typeof(ToggleButton), Android.Resource.Attribute.ButtonStyleToggle }
                                              };

        /// <inheritdoc />
        public View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            View view = null;

            if (context is LayoutInflater.IFactory)
            {
                view = ((LayoutInflater.IFactory)context).OnCreateView(name, context, attrs);
            }

            if (factory != null && view == null)
            {
                view = factory.OnCreateView(name, context, attrs);
            }

            if (view == null)
            {
                view = CreateViewOrFailQuietly(name, context, attrs);
            }

            if (view != null)
            {
                OnViewCreated(view, name, context, attrs);
            }

            return view;
        }

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
                styleIds[0] = Styles.ContainsKey(view.GetType())
                        ? Styles[view.GetType()]
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
            //if (parentIsToolbarV7(view))
            //{
            //    Android.Support.V7.Widget.Toolbar parent = (android.support.v7.widget.Toolbar)view.Parent;
            //    return TextUtils.Equals(parent.Title, view.Text);
            //}
            return false;
        }

        /// <summary>
        ///  An even dirtier way to see if the TextView is part of the ActionBar
        /// </summary>
        /// <param name="view">TextView to check is Title.</param>
        /// <returns>true if it is.</returns>
        protected static bool IsActionBarSubTitle(TextView view)
        {
            if (MatchesResourceIdName(view, ActionBarSubtitle)) return true;
            //if (parentIsToolbarV7(view)) {
            //     android.support.v7.widget.Toolbar parent = (android.support.v7.widget.Toolbar) view.Parent;
            //    return TextUtils.equals(parent.Subtitle, view.Text);
            //}
            return false;
        }

        //protected static bool parentIsToolbarV7(View view) {
        //    return CalligraphyUtils.canCheckForV7Toolbar() && view.Parent != null && (view.Parent is android.support.v7.widget.Toolbar);
        //}

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

        public CalligraphyFactory(LayoutInflater.IFactory factory, int attributeId)
        {
            this.factory = factory;
            this.attributeId = attributeId;
        }

        protected View CreateViewOrFailQuietly(string name, Context context, IAttributeSet attrs)
        {
            if (name.Contains("."))
            {
                return CreateViewOrFailQuietly(name, null, context, attrs);
            }

            return ClassPrefixList.Select(prefix => CreateViewOrFailQuietly(name, prefix, context, attrs)).FirstOrDefault(view => view != null);
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

        protected void OnViewCreated(View view, string name, Context context, IAttributeSet attrs)
        {
            if (!(view is TextView))
            {
                return;
            }
            // Fast path the setting of TextView's font, means if we do some delayed setting of font,
            // which has already been set by use we skip this TextView (mainly for inflating custom,
            // TextView's inside the Toolbar/ActionBar).
            if (TypefaceUtils.IsLoaded(((TextView)view).Typeface))
            {
                return;
            }
            // Try to get typeface attribute value
            // Since we're not using namespace it's a little bit tricky

            // Try view xml attributes
            var textViewFont = CalligraphyUtils.PullFontPathFromView(context, attrs, attributeId);

            // Try view style attributes
            if (TextUtils.IsEmpty(textViewFont))
            {
                textViewFont = CalligraphyUtils.PullFontPathFromStyle(context, attrs, attributeId);
            }

            // Try View TextAppearance
            if (TextUtils.IsEmpty(textViewFont))
            {
                textViewFont = CalligraphyUtils.PullFontPathFromTextAppearance(context, attrs, attributeId);
            }

            // Try theme attributes
            if (TextUtils.IsEmpty(textViewFont))
            {
                var styleForTextView = GetStyleForTextView((TextView)view);
                if (styleForTextView[1] != -1)
                    textViewFont = CalligraphyUtils.PullFontPathFromTheme(context, styleForTextView[0], styleForTextView[1], attributeId);
                else
                    textViewFont = CalligraphyUtils.PullFontPathFromTheme(context, styleForTextView[0], attributeId);
            }


            // Still need to defer the Native action bar, appcompat-v7:21+ uses the Toolbar underneath. But won't match these anyway.
            var deferred = MatchesResourceIdName(view, ActionBarTitle) || MatchesResourceIdName(view, ActionBarSubtitle);

            CalligraphyUtils.ApplyFontToTextView(context, (TextView)view, CalligraphyConfig.Get(), textViewFont, deferred);

            // AppCompat API21+ The ActionBar doesn't inflate default Title/SubTitle, we need to scan the
            // Toolbar(Which underlies the ActionBar) for its children.
            //if (CalligraphyUtils.canCheckForV7Toolbar() && view is Android.Support.V7.Widget.Toolbar) {
            //     ViewGroup parent = (ViewGroup) view;
            //    parent.getViewTreeObserver().addOnGlobalLayoutListener(new ViewTreeObserver.OnGlobalLayoutListener() {
            //        @Override
            //        public void onGlobalLayout() {
            //            // No children, do nuffink!
            //            if (parent.getChildCount() <= 0) return;
            //            // Process children, defer draw as it has set the typeface.
            //            for (int i = 0; i < parent.getChildCount(); i++) {
            //                onViewCreated(parent.getChildAt(i), null, context, null);
            //            }
            //        }
            //    });
        }
    }
}