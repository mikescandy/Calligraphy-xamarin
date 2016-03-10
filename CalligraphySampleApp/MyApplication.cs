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
            CalligraphyConfig.InitDefault(new Builder()
               .SetDefaultFontPath("fonts/gtw.ttf")
               .SetFontAttrId(Resource.Attribute.fontPath)
               .AddCustomViewWithSetTypeface(typeof(CustomViewWithTypefaceSupport))
                .AddCustomStyle(typeof(TextField), Resource.Attribute.textFieldStyle)
                .Build()
        );
        }
    }
}