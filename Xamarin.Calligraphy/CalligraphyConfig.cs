using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using String = System.String;

namespace Calligraphy
{
    public class CalligraphyConfig
    {
        private static readonly Dictionary<Type, int> DEFAULT_STYLES = new Dictionary<Type, int>();

        static CalligraphyConfig() 
        {
            DEFAULT_STYLES.Add(typeof(TextView), Android.Resource.Attribute.TextViewStyle);
            DEFAULT_STYLES.Add(typeof(Button), Android.Resource.Attribute.ButtonStyle);
            DEFAULT_STYLES.Add(typeof(EditText), Android.Resource.Attribute.EditTextStyle);
            DEFAULT_STYLES.Add(typeof(AutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle);
            DEFAULT_STYLES.Add(typeof(MultiAutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle);
            DEFAULT_STYLES.Add(typeof(CheckBox), Android.Resource.Attribute.CheckboxStyle);
            DEFAULT_STYLES.Add(typeof(RadioButton), Android.Resource.Attribute.RadioButtonStyle);
            DEFAULT_STYLES.Add(typeof(ToggleButton), Android.Resource.Attribute.ButtonStyleToggle);
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
            DEFAULT_STYLES.Add(typeof(Android.Support.V7.Widget.AppCompatTextView), Android.Resource.Attribute.TextViewStyle);
            DEFAULT_STYLES.Add(typeof(Android.Support.V7.Widget.AppCompatButton), Android.Resource.Attribute.ButtonStyle);
            DEFAULT_STYLES.Add(typeof(Android.Support.V7.Widget.AppCompatEditText), Android.Resource.Attribute.EditTextStyle);
            DEFAULT_STYLES.Add(typeof(Android.Support.V7.Widget.AppCompatAutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle);
            DEFAULT_STYLES.Add(typeof(Android.Support.V7.Widget.AppCompatMultiAutoCompleteTextView), Android.Resource.Attribute.AutoCompleteTextViewStyle);
            DEFAULT_STYLES.Add(typeof(Android.Support.V7.Widget.AppCompatCheckBox), Android.Resource.Attribute.CheckboxStyle);
            DEFAULT_STYLES.Add(typeof(Android.Support.V7.Widget.AppCompatRadioButton), Android.Resource.Attribute.RadioButtonStyle);
            DEFAULT_STYLES.Add(typeof(Android.Support.V7.Widget.AppCompatCheckedTextView), Android.Resource.Attribute.CheckedTextViewStyle);
        }

        private static CalligraphyConfig sInstance;

        /**
         * Set the default Calligraphy Config
         *
         * @param calligraphyConfig the config build using the builder.
         * @see uk.co.chrisjenx.calligraphy.CalligraphyConfig.Builder
         */
        public static void InitDefault(CalligraphyConfig calligraphyConfig)
        {
            sInstance = calligraphyConfig;
        }

        /**
         * The current Calligraphy Config.
         * If not set it will create a default config.
         */
        public static CalligraphyConfig Get()
        {
            if (sInstance == null)
                sInstance = new CalligraphyConfig(new Builder());
            return sInstance;
        }

        /**
         * Is a default font set?
         */
        private bool mIsFontSet;
        /**
         * The default Font Path if nothing else is setup.
         */
        private string mFontPath;
        /**
         * Default Font Path Attr Id to lookup
         */
        private int mAttrId;
        /**
         * Use Reflection to inject the private factory.
         */
        private bool mReflection;
        /**
         * Use Reflection to intercept CustomView inflation with the correct Context.
         */
        private bool mCustomViewCreation;
        /**
         * Use Reflection to try to set typeface for custom views if they has setTypeface method
         */
        private bool mCustomViewTypefaceSupport;
        /**
         * Class Styles. Build from DEFAULT_STYLES and the builder.
         */
        private ReadOnlyDictionary<Type, int> mClassStyleAttributeMap;
        /**
         * Collection of custom non-{@code TextView}'s registered for applying typeface during inflation
         * @see uk.co.chrisjenx.calligraphy.CalligraphyConfig.Builder#addCustomViewWithSetTypeface(Class)
         */
        private HashSet<Type> hasTypefaceViews;

        public CalligraphyConfig(Builder builder)
        {
            mIsFontSet = builder.isFontSet;
            mFontPath = builder.fontAssetPath;
            mAttrId = builder.attrId;
            mReflection = builder.reflection;
            mCustomViewCreation = builder.customViewCreation;
            mCustomViewTypefaceSupport = builder.customViewTypefaceSupport;
           var tempMap = new Dictionary<Type, int>(DEFAULT_STYLES);
            foreach (var i in builder.mStyleClassMap)
            {
                tempMap.Add(i.Key,i.Value);
            }
            mClassStyleAttributeMap = new ReadOnlyDictionary<Type, int>(tempMap);
            hasTypefaceViews = new HashSet<Type>(builder.mHasTypefaceClasses.ToList());
        }

        /**
         * @return mFontPath for text views might be null
         */
        public string getFontPath()
        {
            return mFontPath;
        }

        /**
         * @return true if set, false if null|empty
         */
        public bool isFontSet()
        {
            return mIsFontSet;
        }

        public bool isReflection()
        {
            return mReflection;
        }

        public bool isCustomViewCreation()
        {
            return mCustomViewCreation;
        }

        public bool isCustomViewTypefaceSupport()
        {
            return mCustomViewTypefaceSupport;
        }

        public bool isCustomViewHasTypeface(View view)
        {
            return hasTypefaceViews.Contains(typeof(View));
        }

        /* default */
        public ReadOnlyDictionary<Type, int> getClassStyles()
        {
            return mClassStyleAttributeMap;
        }

        /**
         * @return the custom attrId to look for, -1 if not set.
         */
        public int AttrId
        {
            get
            {
                return mAttrId;
            }
        }

        public class Builder
        {
            /**
             * Default AttrID if not set.
             */
            public static int INVALID_ATTR_ID = -1;
            /**
             * Use Reflection to inject the private factory. Doesn't exist pre HC. so defaults to false.
             */
            internal bool reflection = Build.VERSION.SdkInt >= Build.VERSION_CODES.Honeycomb;
            /**
             * Use Reflection to intercept CustomView inflation with the correct Context.
             */
            internal bool customViewCreation = true;
            /**
             * Use Reflection during view creation to try change typeface via setTypeface method if it exists
             */
            internal bool customViewTypefaceSupport = false;
            /**
             * The fontAttrId to look up the font path from.
             */
            internal int attrId = Resource.Attribute.fontPath;
            /**
             * Has the user set the default font path.
             */
            internal bool isFontSet = false;
            /**
             * The default fontPath
             */
            internal string fontAssetPath = null;
            /**
             * Additional Class Styles. Can be empty.
             */
            internal Dictionary <Type, int> mStyleClassMap = new Dictionary<Type, int>();

            internal HashSet<Type> mHasTypefaceClasses = new HashSet<Type>();

            /**
             * This defaults to R.attr.fontPath. So only override if you want to use your own attrId.
             *
             * @param fontAssetAttrId the custom attribute to look for fonts in assets.
             * @return this builder.
             */
            public Builder setFontAttrId(int fontAssetAttrId)
            {
                this.attrId = fontAssetAttrId != INVALID_ATTR_ID ? fontAssetAttrId : INVALID_ATTR_ID;
                return this;
            }

            /**
             * Set the default font if you don't define one else where in your styles.
             *
             * @param defaultFontAssetPath a path to a font file in the assets folder, e.g. "fonts/Roboto-light.ttf",
             *                             passing null will default to the device font-family.
             * @return this builder.
             */
            public Builder setDefaultFontPath(String defaultFontAssetPath)
            {
                this.isFontSet = !TextUtils.IsEmpty(defaultFontAssetPath);
                this.fontAssetPath = defaultFontAssetPath;
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
            public Builder disablePrivateFactoryInjection()
            {
                this.reflection = false;
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
            public Builder disableCustomViewInflation()
            {
                this.customViewCreation = false;
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
            public Builder addCustomStyle(Type styleClass, int styleResourceAttribute)
            {
                if (styleClass == null || styleResourceAttribute == 0) return this;
                mStyleClassMap.Add(styleClass, styleResourceAttribute);
                return this;
            }

            /**
             * Register custom non-{@code TextView}'s which implement {@code setTypeface} so they can have the Typeface applied during inflation.
             */
            public Builder addCustomViewWithSetTypeface(Type clazz)
            {
                customViewTypefaceSupport = true;
                mHasTypefaceClasses.Add(clazz);
                return this;
            }

            public CalligraphyConfig build()
            {
                this.isFontSet = !TextUtils.IsEmpty(fontAssetPath);
                return new CalligraphyConfig(this);
            }
        }
    }
}