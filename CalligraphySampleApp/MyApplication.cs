using System;

using Android.App;
using Android.Runtime;

using Calligraphy;

namespace CalligraphySampleApp
{
    [Application(ManageSpaceActivity = typeof(MainActivity))]
    public class MyApplication : Application
    {
        /// <inheritdoc />
        public MyApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        /// <inheritdoc />
        public override void OnCreate()
        {
            base.OnCreate();
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
               .setDefaultFontPath("fonts/gtw.ttf")
               .setFontAttrId(Resource.Attribute.fontPath)
               .addCustomViewWithSetTypeface(typeof(CustomViewWithTypefaceSupport))
                .addCustomStyle(typeof(TextField), Resource.Attribute.textFieldStyle)
                .build()
        );
        }
    }
}