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
            CalligraphyConfig.InitDefault("fonts/gtw.ttf", Calligraphy.Resource.Attribute.fontPath);
        }
    }
}