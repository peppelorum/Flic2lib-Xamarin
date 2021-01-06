using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IO.Flic.Flic2libandroid;
using Xamarin.Essentials;

namespace Flic2libDemo.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IFlic2ScanCallback
    {
        private Button ScanButton { get; set; }
        private EditText LogEditText { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            ScanButton = FindViewById<Button>(Resource.Id.ScanButton);
            ScanButton.Click += ScanButton_Click;
            LogEditText = FindViewById<EditText>(Resource.Id.LogEditText);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void LogMsg(string msg)
        {
            LogEditText.Text += $"\n {msg}";
        }

        private void ClearLog()
        {
            LogEditText.Text = string.Empty;
        }

        private async void ScanButton_Click(object sender, EventArgs e)
        {
            ClearLog();
            var status = await Permissions.RequestAsync<Permissions.LocationAlways>();
            if (status == PermissionStatus.Granted)
            {
                Flic2Manager.Instance.StartScan(this);
            }
            else
            {
                LogMsg("Permission not granted");
            }
        }

        #region IFlic2ScanCallback
        public void OnComplete(int result, int subCode, Flic2Button button)
        {
            if (result == Flic2ScanCallback.ResultSuccess)
            {
                // Success!
                // The button object can now be used
                LogMsg($"Connection Name:{button.Name} SerialNumber:{button.SerialNumber} BdAddr:{button.BdAddr}");
                button.AddListener(new MyFlic2ButtonListener(this));
            }
            else
            {
                LogMsg($"Connection error code {result} - {subCode}");
            }
        }

        public void OnConnected()
        {
            LogMsg("Connected. Now pairing...");
        }

        public void OnDiscovered(string bdAddr)
        {
            LogMsg($"Found Flic2, now connecting bdAddr:{bdAddr}");
        }

        public void OnDiscoveredAlreadyPairedButton(Flic2Button p0)
        {
            LogMsg("Found an already paired button. Try another button.");
        }
        #endregion IFlic2ScanCallback

        class MyFlic2ButtonListener : Flic2ButtonListener
        {
            private MainActivity _owner;
            public MyFlic2ButtonListener(MainActivity owner)
            {
                _owner = owner;
            }

            public override void OnButtonUpOrDown(Flic2Button button, bool wasQueued, bool lastQueued, long timestamp, bool isUp, bool isDown)
            {
                base.OnButtonUpOrDown(button, wasQueued, lastQueued, timestamp, isUp, isDown);
                if (isDown)
                {
                    _owner.LogMsg ($"Button {button} was pressed");
                }
            }
        }
    }
}
