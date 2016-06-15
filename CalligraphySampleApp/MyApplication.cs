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
               
               .SetDefaultFontPath("fonts/gtw.ttf")
               .SetFontAttrId(Resource.Attribute.fontPath)
               .AddCustomViewWithSetTypeface(Java.Lang.Class.FromType(typeof(CustomViewWithTypefaceSupport)))
                .AddCustomStyle(Java.Lang.Class.FromType(typeof(TextField)), Resource.Attribute.textFieldStyle)
                .Build()
        );
        }
    }
}