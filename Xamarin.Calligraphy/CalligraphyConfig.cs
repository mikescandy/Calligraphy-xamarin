using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Views;
using Android.Widget;

namespace Calligraphy
{
    public class CalligraphyConfig
    {
        private static CalligraphyConfig _instance;
        private static readonly Dictionary<Type, int> DefaultStyles = new Dictionary<Type, int>();
        /**
   * Collection of custom non-{@code TextView}'s registered for applying typeface during inflation
   * @see uk.co.chrisjenx.calligraphy.CalligraphyConfig.Builder#addCustomViewWithSetTypeface(Class)
   */
        private readonly HashSet<Type> _typefaceViews;
        private int? _attrId;
        /* default */
        public ReadOnlyDictionary<Type, int> ClassStyleAttributeMap { get; set; }

        /**
         * @return the custom attrId to look for, -1 if not set.
         */

        public int AttrId
        {
            get { return _attrId ?? -1; }
            set { _attrId = value; }
        }
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
        
        public bool IsCustomViewHasTypeface(View view)
        {
            return _typefaceViews.Contains(typeof(View));
        }
      }
}