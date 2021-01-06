using System;
using Android.App;
using IO.Flic.Flic2libandroid;

namespace Flic2libDemo.Droid
{
    [Application(Label = "@string/app_name")]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Flic2Manager manager = Flic2Manager.InitAndGetInstance(ApplicationContext, new Android.OS.Handler());
        }
    }
}
