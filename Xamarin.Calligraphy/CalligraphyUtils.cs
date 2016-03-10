using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Widget;

using Java.Lang;


namespace Calligraphy
{
    internal class CalligraphyUtils
    {
        public static int[] AndroidAttrTextAppearance = { Android.Resource.Attribute.TextAppearance };
        /// <summary>
        /// Applies a custom typeface span to the text.
        /// </summary>
        /// <param name="s">text to apply it too.</param>
        /// <param name="typeface">typeface to apply.</param>
        /// <returns>Either the passed in Object or new Spannable with the typeface span applied.</returns>
        internal static ICharSequence ApplyTypefaceSpan(ICharSequence s, Typeface typeface)
        {
            if (s == null || s.Length() <= 0)
            {
                return s;
            }
            if (!(s is ISpannable))
            {
                s = new SpannableString(s);
            }
            ((ISpannable)s).SetSpan(TypefaceUtils.GetSpan(typeface), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            return s;
        }

        /// <summary>
        /// Applies a Typeface to a TextView.
        /// Defaults to false for deferring, if you are having issues with the textview keeping
        /// the custom Typeface, use <see cref="ApplyFontToTextView(TextView, Typeface, bool)"/>. 
        /// </summary>
        /// <param name="textView">Not null, TextView or child of.</param>
        /// <param name="typeface">Typeface to apply to the TextView.</param>
        /// <returns>true if applied otherwise false</returns>
        /// <see cref="ApplyFontToTextView(Android.Widget.TextView,Android.Graphics.Typeface, bool)"/>
        internal static bool ApplyFontToTextView(TextView textView, Typeface typeface)
        {
            return ApplyFontToTextView(textView, typeface, false);
        }

        /// <summary>
        /// Applies a Typeface to a TextView, if deferred,its recommend you don't call this multiple
        /// times, as this adds a TextWatcher.
        /// Deferring should really only be used on tricky views which get Typeface set by the system at
        /// weird times.
        /// </summary>
        /// <param name="textView">Not null, TextView or child of.</param>
        /// <param name="typeface">Not null, Typeface to apply to the TextView.</param>
        /// <param name="deferred">if set to <c>true</c> [deferred].
        /// If true we use Typefaces and TextChange listener to make sure font is always
        /// applied, but this sometimes conflicts with other <see cref="Android.Text.ISpannable"/>.
        /// </param>
        /// <returns>true if applied otherwise false.</returns>
        /// <see cref="ApplyFontToTextView(Android.Widget.TextView,Android.Graphics.Typeface)"/>
        internal static bool ApplyFontToTextView(TextView textView, Typeface typeface, bool deferred)
        {
            if (textView == null || typeface == null)
            {
                return false;
            }

            textView.PaintFlags = textView.PaintFlags | PaintFlags.SubpixelText | PaintFlags.AntiAlias;
            textView.Typeface = typeface;
            if (!deferred)
            {
                return true;
            }

            textView.SetText(ApplyTypefaceSpan(textView.TextFormatted, typeface), TextView.BufferType.Spannable);

            textView.AddTextChangedListener(new TextWatcher(typeface));
            return true;
        }

        /// <summary>
        /// Useful for manually fonts to a TextView. Will not default back to font
        /// set in <see cref="CalligraphyConfig"/>
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="textView">Not null, TextView to apply to.</param>
        /// <param name="filePath">The file path.if null/empty will do nothing.</param>
        /// <returns>true if fonts been applied</returns>
        internal static bool ApplyFontToTextView(Context context, TextView textView, string filePath)
        {
            return ApplyFontToTextView(context, textView, filePath, false);
        }

        internal static bool ApplyFontToTextView(Context context, TextView textView, string filePath, bool deferred)
        {
            if (textView == null || context == null)
            {
                return false;
            }

            var assetManager = context.Assets;
            var typeface = TypefaceUtils.Load(assetManager, filePath);
            return ApplyFontToTextView(textView, typeface, deferred);
        }

        internal static void ApplyFontToTextView(Context context, TextView textView, CalligraphyConfig config)
        {
            ApplyFontToTextView(context, textView, config, false);
        }

        internal static void ApplyFontToTextView(Context context, TextView textView, CalligraphyConfig config, bool deferred)
        {
            if (context == null || textView == null || config == null)
            {
                return;
            }

            if (!config.IsFontSet)
            {
                return;
            }

            ApplyFontToTextView(context, textView, config.FontPath, deferred);
        }

        /// <summary>
        /// Applies font to TextView. Will fall back to the default one if not set.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="textView"> textView to apply to.</param>
        /// <param name="config">Default Config.</param>
        /// <param name="textViewFont">nullable, will use Default Config if null or fails to find the defined font.</param>
        internal static void ApplyFontToTextView(Context context, TextView textView, CalligraphyConfig config, string textViewFont)
        {
            ApplyFontToTextView(context, textView, config, textViewFont, false);
        }

        internal static void ApplyFontToTextView(Context context, TextView textView, CalligraphyConfig config, string textViewFont, bool deferred)
        {
            if (context == null || textView == null || config == null)
            {
                return;
            }

            if (!TextUtils.IsEmpty(textViewFont) && ApplyFontToTextView(context, textView, textViewFont, deferred))
            {
                return;
            }

            ApplyFontToTextView(context, textView, config, deferred);
        }

        /// <summary>
        /// Tries to pull the Custom Attribute directly from the TextView.
        /// </summary>
        /// <param name="context">Activity Context.</param>
        /// <param name="attrs">View Attributes.</param>
        /// <param name="attributeId">f -1 returns null.</param>
        /// <returns>null if attribute is not defined or added to View</returns>
        internal static string PullFontPathFromView(Context context, IAttributeSet attrs, int[] attributeId)
        {
            if (attributeId == null || attrs == null)
            {
                return null;
            }

            string attributeName;
            try
            {
                attributeName = context.Resources.GetResourceEntryName(attributeId[0]);
            }
            catch (Resources.NotFoundException e)
            {
                Log.Debug("CalligraphyUtils - invalid attribute ID", e.Message);
                // invalid attribute ID
                return null;
            }

            var stringResourceId = attrs.GetAttributeResourceValue(null, attributeName, -1);
            return stringResourceId > 0
                    ? context.GetString(stringResourceId)
                    : attrs.GetAttributeValue(null, attributeName);
        }

        /// <summary>
        /// Tries to pull the Font Path from the View Style as this is the next decendent after being
        /// defined in the View's xml.
        /// </summary>
        /// <param name="context">Activity Activity Context.</param>
        /// <param name="attrs">View Attributes.</param>
        /// <param name="attributeId">if -1 returns null.</param>
        /// <returns>null if attribute is not defined or found in the Style</returns>
        internal static string PullFontPathFromStyle(Context context, IAttributeSet attrs, int[] attributeId)
        {
            if (attributeId == null || attrs == null)
            {
                return null;
            }

            var typedArray = context.ObtainStyledAttributes(attrs, new[] { attributeId[0] });
            if (typedArray == null)
            {
                return null;
            }

            try
            {
                // First defined attribute
                var fontFromAttribute = typedArray.GetString(0);
                if (!TextUtils.IsEmpty(fontFromAttribute))
                {
                    return fontFromAttribute;
                }
            }
            catch (System.Exception ignore)
            {
                // Failed for some reason.
                Log.Error("Calligraphy", ignore.Message);
            }
            finally
            {
                typedArray.Recycle();
            }
            return null;
        }

        /// <summary>
        /// Tries to pull the Font Path from the Text Appearance.
        /// </summary>
        /// <param name="context">Activity Context.</param>
        /// <param name="attrs">View Attributes.</param>
        /// <param name="attributeId">if -1 returns null.</param>
        /// <returns>returns null if attribute is not defined or if no TextAppearance is found.</returns>
        internal static string PullFontPathFromTextAppearance(Context context, IAttributeSet attrs, int[] attributeId)
        {
            if (attributeId == null || attrs == null)
            {
                return null;
            }

            var textAppearanceId = -1;
            var typedArrayAttr = context.ObtainStyledAttributes(attrs, AndroidAttrTextAppearance);
            if (typedArrayAttr != null)
            {
                try
                {
                    textAppearanceId = typedArrayAttr.GetResourceId(0, -1);
                }
                catch (System.Exception ignored)
                {
                    // Failed for some reason
                    Log.Error("Calligraphy", ignored.Message);
                    return null;
                }
                finally
                {
                    typedArrayAttr.Recycle();
                }
            }

            var textAppearanceAttrs = context.ObtainStyledAttributes(textAppearanceId, new[] { attributeId[0] });
            if (textAppearanceAttrs != null)
            {
                try
                {
                    return textAppearanceAttrs.GetString(0);
                }
                catch (System.Exception ignore)
                {
                    // Failed for some reason.
                    Log.Error("Calligraphy", ignore.Message);
                    return null;
                }
                finally
                {
                    textAppearanceAttrs.Recycle();
                }
            }
            return null;
        }

        /// <summary>
        /// Last but not least, try to pull the Font Path from the Theme, which is defined.
        /// </summary>
        /// <param name="context">Activity Context.</param>
        /// <param name="styleAttrId">Theme style id.</param>
        /// <param name="attributeId">if -1 returns null.</param>
        /// <returns>null if no theme or attribute defined.</returns>
        internal static string PullFontPathFromTheme(Context context, int styleAttrId, int[] attributeId)
        {
            if (styleAttrId == -1 || attributeId == null)
            {
                return null;
            }

            var theme = context.Theme;
            var value = new TypedValue();

            theme.ResolveAttribute(styleAttrId, value, true);
            var typedArray = theme.ObtainStyledAttributes(value.ResourceId, new[] { attributeId[0] });
            try
            {
                var font = typedArray.GetString(0);
                return font;
            }
            catch (System.Exception ignore)
            {
                // Failed for some reason.
                Log.Error("Calligraphy", ignore.Message);
                return null;
            }
            finally
            {
                typedArray.Recycle();
            }
        }

        /// <summary>
        /// Last but not least, try to pull the Font Path from the Theme, which is defined.
        /// </summary>
        /// <param name="context">Activity Context.</param>
        /// <param name="styleAttrId">Theme style id.</param>
        /// <param name="subStyleAttrId">the sub style from the theme to look up after the first style.</param>
        /// <param name="attributeId">if -1 returns null.</param>
        /// <returns>null if no theme or attribute defined.</returns>
        internal static string PullFontPathFromTheme(Context context, int styleAttrId, int subStyleAttrId, int[] attributeId)
        {
            if (styleAttrId == -1 || attributeId == null)
                return null;

            var theme = context.Theme;
            var value = new TypedValue();

            theme.ResolveAttribute(styleAttrId, value, true);
            int subStyleResId;
            var parentTypedArray = theme.ObtainStyledAttributes(value.ResourceId, new[] { subStyleAttrId });
            try
            {
                subStyleResId = parentTypedArray.GetResourceId(0, -1);
            }
            catch (System.Exception ignore)
            {
                // Failed for some reason.
                Log.Error("Calligraphy", ignore.Message);
                return null;
            }
            finally
            {
                parentTypedArray.Recycle();
            }

            if (subStyleResId == -1)
            {
                return null;
            }

            var subTypedArray = context.ObtainStyledAttributes(subStyleResId, new[] { attributeId[0] });
            if (subTypedArray != null)
            {
                try
                {
                    return subTypedArray.GetString(0);
                }
                catch (System.Exception ignore)
                {
                    // Failed for some reason.
                    Log.Error("Calligraphy", ignore.Message);
                    return null;
                }
                finally
                {
                    subTypedArray.Recycle();
                }
            }
            return null;
        }

        private static bool? _toolbarCheck;
        private static bool? _appCompatViewCheck;

        /**
         * See if the user has added appcompat-v7, this is done at runtime, so we only check once.
         *
         * @return true if the v7.Toolbar is on the classpath
         */
        public static bool CanCheckForV7Toolbar()
        {
            if (_toolbarCheck != null) return _toolbarCheck.Value;
            try
            {
                // ReSharper disable once UnusedVariable
                var x = Activator.CreateInstance(null, "android.support.v7.widget.Toolbar");
                _toolbarCheck = true;
            }
            catch (System.Exception e)
            {
                Log.Debug("CalligraphyUtils", e.Message);
                _toolbarCheck = false;
            }
            return _toolbarCheck.Value;
        }

        public static bool CanAddV7AppCompatViews()
        {
            if (_appCompatViewCheck != null) return _appCompatViewCheck.Value;
            try
            {
                // ReSharper disable once UnusedVariable
                var x = Activator.CreateInstance("Xamarin.Android.Support.v7.AppCompat", "Android.Support.V7.Widget.AppCompatTextView");
                _appCompatViewCheck = true;
            }
            catch (System.Exception e)
            {
                Log.Debug("CalligraphyUtils", e.Message);
                _appCompatViewCheck = false;
            }
            return _appCompatViewCheck.Value;
        }

        private CalligraphyUtils()
        {
        }
    }
}