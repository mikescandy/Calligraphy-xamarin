using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace Calligraphy
{
    public class CalligraphyConfig
    {
        private static CalligraphyConfig _instance;
        private static readonly Dictionary<Type, int> DefaultStyles = new Dictionary<Type, int>();

        public bool IsFontSet { get; set; }
        public bool IsReflection { get; set; }
        public bool IsCustomViewCreation { get; set; }
        public bool IsCustomViewTypefaceSupport { get; set; }
        public string FontPath { get; set; }


        static CalligraphyConfig()
        {
            DefaultStyles.Add(typeof(TextView), Android.Resource.Attribute.TextViewStyle);
            DefaultStyles.Add(typeof(Button), Android.Resource.Attribute.ButtonStyle);
            DefaultStyles.Add(typeof(EditText), Android.Resource.Attribute.EditTextStyle);
            DefaultStyles.Add(typeof(AutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle);
            DefaultStyles.Add(typeof(MultiAutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle);
            DefaultStyles.Add(typeof(CheckBox), Android.Resource.Attribute.CheckboxStyle);
            DefaultStyles.Add(typeof(RadioButton), Android.Resource.Attribute.RadioButtonStyle);
            DefaultStyles.Add(typeof(ToggleButton), Android.Resource.Attribute.ButtonStyleToggle);
            if (CalligraphyUtils.CanAddV7AppCompatViews())
            {
                AddAppCompatViews();
            }
        }

        /**
         * AppCompat will inflate special versions of views for Material tinting etc,
         * this adds those classes to the style lookup map
         */
        private static void AddAppCompatViews()
        {
            DefaultStyles.Add(typeof(Android.Support.V7.Widget.AppCompatTextView), Android.Resource.Attribute.TextViewStyle);
            DefaultStyles.Add(typeof(Android.Support.V7.Widget.AppCompatButton), Android.Resource.Attribute.ButtonStyle);
            DefaultStyles.Add(typeof(Android.Support.V7.Widget.AppCompatEditText), Android.Resource.Attribute.EditTextStyle);
            DefaultStyles.Add(typeof(Android.Support.V7.Widget.AppCompatAutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle);
            DefaultStyles.Add(typeof(Android.Support.V7.Widget.AppCompatMultiAutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle);
            DefaultStyles.Add(typeof(Android.Support.V7.Widget.AppCompatCheckBox), Android.Resource.Attribute.CheckboxStyle);
            DefaultStyles.Add(typeof(Android.Support.V7.Widget.AppCompatRadioButton), Android.Resource.Attribute.RadioButtonStyle);
            DefaultStyles.Add(typeof(Android.Support.V7.Widget.AppCompatCheckedTextView), Android.Resource.Attribute.CheckedTextViewStyle);
        }


        /**
         * Set the default Calligraphy Config
         *
         * @param calligraphyConfig the config build using the builder.
         * @see uk.co.chrisjenx.calligraphy.CalligraphyConfig.Builder
         */
        public static void InitDefault(CalligraphyConfig calligraphyConfig)
        {
            _instance = calligraphyConfig;
        }

        /**
         * The current Calligraphy Config.
         * If not set it will create a default config.
         */
        public static CalligraphyConfig Get()
        {
            return _instance ?? (_instance = new CalligraphyConfig(new Builder()));
        }

        /**
         * Collection of custom non-{@code TextView}'s registered for applying typeface during inflation
         * @see uk.co.chrisjenx.calligraphy.CalligraphyConfig.Builder#addCustomViewWithSetTypeface(Class)
         */
        private readonly HashSet<Type> _typefaceViews;

        public CalligraphyConfig(Builder builder)
        {
            IsFontSet = builder.IsFontSet;
            FontPath = builder.FontAssetPath;
            AttrId = builder.AttrId;
            IsReflection = builder.Reflection;
            IsCustomViewCreation = builder.CustomViewCreation;
            IsCustomViewTypefaceSupport = builder.CustomViewTypefaceSupport;
            var tempMap = new Dictionary<Type, int>(DefaultStyles);
            foreach (var i in builder.MStyleClassMap)
            {
                tempMap.Add(i.Key, i.Value);
            }
            ClassStyleAttributeMap = new ReadOnlyDictionary<Type, int>(tempMap);
            _typefaceViews = new HashSet<Type>(builder.MHasTypefaceClasses.ToList());
        }

        /**
         * @return mFontPath for text views might be null
         */

        /**
         * @return true if set, false if null|empty
         */


        public bool IsCustomViewHasTypeface(View view)
        {
            return _typefaceViews.Contains(typeof(View));
        }

        /* default */
        public ReadOnlyDictionary<Type, int> ClassStyleAttributeMap { get; set; }

        /**
         * @return the custom attrId to look for, -1 if not set.
         */
        public int AttrId { get; set; }


        public class Builder
        {
            /**
             * Default AttrID if not set.
             */
            public static int InvalidAttrId = -1;
            /**
             * Use Reflection to inject the private factory. Doesn't exist pre HC. so defaults to false.
             */
            internal bool Reflection = Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb;
            /**
             * Use Reflection to intercept CustomView inflation with the correct Context.
             */
            internal bool CustomViewCreation = true;
            /**
             * Use Reflection during view creation to try change typeface via setTypeface method if it exists
             */
            internal bool CustomViewTypefaceSupport = false;
            /**
             * The fontAttrId to look up the font path from.
             */
            internal int AttrId = Resource.Attribute.fontPath;
            /**
             * Has the user set the default font path.
             */
            internal bool IsFontSet = false;
            /**
             * The default fontPath
             */
            internal string FontAssetPath = null;
            /**
             * Additional Class Styles. Can be empty.
             */
            internal Dictionary<Type, int> MStyleClassMap = new Dictionary<Type, int>();

            internal HashSet<Type> MHasTypefaceClasses = new HashSet<Type>();

            /**
             * This defaults to R.attr.fontPath. So only override if you want to use your own attrId.
             *
             * @param fontAssetAttrId the custom attribute to look for fonts in assets.
             * @return this builder.
             */
            public Builder SetFontAttrId(int fontAssetAttrId)
            {
                AttrId = fontAssetAttrId != InvalidAttrId ? fontAssetAttrId : InvalidAttrId;
                return this;
            }

            /**
             * Set the default font if you don't define one else where in your styles.
             *
             * @param defaultFontAssetPath a path to a font file in the assets folder, e.g. "fonts/Roboto-light.ttf",
             *                             passing null will default to the device font-family.
             * @return this builder.
             */
            public Builder SetDefaultFontPath(string defaultFontAssetPath)
            {
                IsFontSet = !string.IsNullOrEmpty(defaultFontAssetPath);
                FontAssetPath = defaultFontAssetPath;
                return this;
            }

            /**
             * <p>Turn of the use of Reflection to inject the private factory.
             * This has operational consequences! Please read and understand before disabling.
             * <b>This is already disabled on pre Honeycomb devices. (API 11)</b></p>
             *
             * <p> If you disable this you will need to override your {@link android.app.Activity#onCreateView(android.view.View, String, android.content.Context, android.util.AttributeSet)}
             * as this is set as the {@link android.view.LayoutInflater} private factory.</p>
             * <br>
             * <b> Use the following code in the Activity if you disable FactoryInjection:</b>
             * <pre><code>
             * {@literal @}Override
             * {@literal @}TargetApi(Build.VERSION_CODES.HONEYCOMB)
             * public View onCreateView(View parent, String name, Context context, AttributeSet attrs) {
             *   return CalligraphyContextWrapper.onActivityCreateView(this, parent, super.onCreateView(parent, name, context, attrs), name, context, attrs);
             * }
             * </code></pre>
             */
            public Builder DisablePrivateFactoryInjection()
            {
                Reflection = false;
                return this;
            }

            /**
             * Due to the poor inflation order where custom views are created and never returned inside an
             * {@code onCreateView(...)} method. We have to create CustomView's at the latest point in the
             * overrideable injection flow.
             *
             * On HoneyComb+ this is inside the {@link android.app.Activity#onCreateView(android.view.View, String, android.content.Context, android.util.AttributeSet)}
             * Pre HoneyComb this is in the {@link android.view.LayoutInflater.Factory#onCreateView(String, android.util.AttributeSet)}
             *
             * We wrap base implementations, so if you LayoutInflater/Factory/Activity creates the
             * custom view before we get to this point, your view is used. (Such is the case with the
             * TintEditText etc)
             *
             * The problem is, the native methods pass there parents context to the constructor in a really
             * specific place. We have to mimic this in {@link uk.co.chrisjenx.calligraphy.CalligraphyLayoutInflater#createCustomViewInternal(android.view.View, android.view.View, String, android.content.Context, android.util.AttributeSet)}
             * To mimic this we have to use reflection as the Class constructor args are hidden to us.
             *
             * We have discussed other means of doing this but this is the only semi-clean way of doing it.
             * (Without having to do proxy classes etc).
             *
             * Calling this will of course speed up inflation by turning off reflection, but not by much,
             * But if you want Calligraphy to inject the correct typeface then you will need to make sure your CustomView's
             * are created before reaching the LayoutInflater onViewCreated.
             */
            public Builder DisableCustomViewInflation()
            {
                CustomViewCreation = false;
                return this;
            }

            /**
             * Add a custom style to get looked up. If you use a custom class that has a parent style
             * which is not part of the default android styles you will need to add it here.
             *
             * The Calligraphy inflater is unaware of custom styles in your custom classes. We use
             * the class type to look up the style attribute in the theme resources.
             *
             * So if you had a {@code MyTextField.class} which looked up it's default style as
             * {@code R.attr.textFieldStyle} you would add those here.
             *
             * {@code builder.addCustomStyle(MyTextField.class,R.attr.textFieldStyle}
             *
             * @param styleClass             the class that related to the parent styleResource. null is ignored.
             * @param styleResourceAttribute e.g. {@code R.attr.textFieldStyle}, 0 is ignored.
             * @return this builder.
             */
            public Builder AddCustomStyle(Type styleClass, int styleResourceAttribute)
            {
                if (styleClass == null || styleResourceAttribute == 0) return this;
                MStyleClassMap.Add(styleClass, styleResourceAttribute);
                return this;
            }

            /**
             * Register custom non-{@code TextView}'s which implement {@code setTypeface} so they can have the Typeface applied during inflation.
             */
            public Builder AddCustomViewWithSetTypeface(Type clazz)
            {
                CustomViewTypefaceSupport = true;
                MHasTypefaceClasses.Add(clazz);
                return this;
            }

            public CalligraphyConfig Build()
            {
                IsFontSet = !TextUtils.IsEmpty(FontAssetPath);
                return new CalligraphyConfig(this);
            }
        }
    }
}