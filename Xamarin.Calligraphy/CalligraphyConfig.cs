using System;

using Android.Text;

namespace Calligraphy
{
    public class CalligraphyConfig
    {
        private static CalligraphyConfig instance;

        public string FontPath { get; private set; }
        public bool IsFontSet { get; private set; }
        public int AttrId { get; private set; }

        /// <summary>
        /// Init the Calligraphy Config file. Each time you call this you set a new default. Of course setting this multiple
        /// times during runtime could have undesired effects.
        /// </summary>
        /// <param name="defaultFontAssetPath">a path to a font file in the assets folder, e.g. "fonts/roboto-light.ttf", 
        /// passing null will default to the device font-family.</param>
        public static void InitDefault(string defaultFontAssetPath)
        {
            instance = new CalligraphyConfig(defaultFontAssetPath);
        }

        /// <summary>
        /// Init only the custom attribute to lookup.
        /// </summary>
        /// <param name="defaultAttributeId">Ththe custom attribute to look for.</param>
        /// <see cref="InitDefault(string, int)"/>
        public static void InitDefault(int defaultAttributeId)
        {
            instance = new CalligraphyConfig(defaultAttributeId);
        }

        /// <summary>
        /// Define the default font and the custom attribute to lookup globally.
        /// </summary>
        /// <param name="defaultFontAssetPath">path to a font file in the assets folder, e.g. "fonts/Roboto-light.ttf",</param>
        /// <param name="defaultAttributeId">the custom attribute to look for.</param>
        /// <see cref="InitDefault(string)"/>
        /// <see cref="InitDefault(int)"/>
        public static void InitDefault(String defaultFontAssetPath, int defaultAttributeId)
        {
            instance = new CalligraphyConfig(defaultFontAssetPath, defaultAttributeId);
        }

        public static CalligraphyConfig Get()
        {
            return instance ?? (instance = new CalligraphyConfig());
        }

        private CalligraphyConfig()
            : this(null)
        {
        }

        private CalligraphyConfig(int attrId)
            : this(null, attrId)
        {

        }

        private CalligraphyConfig(string defaultFontAssetPath, int attrId = -1)
        {
            FontPath = defaultFontAssetPath;
            IsFontSet = !TextUtils.IsEmpty(defaultFontAssetPath);
            AttrId = attrId != -1 ? attrId : -1;
        }
    }
}