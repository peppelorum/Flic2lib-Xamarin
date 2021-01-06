using Foundation;
using System;
using UIKit;
using Flic2lib;

namespace Flic2libDemo.IOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            var fLICManager = FLICManager.ConfigureWithDelegate( new MyFLICManagerDelegate(this), new MyFLICButtonDelegate(this), true);

            StartButton.TouchUpInside += StartButton_TouchUpInside;
        }

        private void LogMsg(string msg)
        {
            LogTextView.Text += $"\n {msg}";
        }

        private void ClearLog()
        {
            LogTextView.Text = string.Empty;
        }

        private void StartButton_TouchUpInside(object sender, EventArgs e)
        {
            ClearLog();
            var assdad = FLICManager.SharedManager();
            FLICManager.SharedManager().ScanForButtonsWithStateChangeHandler((FLICButtonScannerStatusEvent statusEvent) =>
            {
                switch (statusEvent)
                {
                    case FLICButtonScannerStatusEvent.Connected:
                        LogMsg("A Flic is being verified.");
                        break;
                    case FLICButtonScannerStatusEvent.Discovered:
                        LogMsg("A Flic was discovered.");
                        break;
                    case FLICButtonScannerStatusEvent.VerificationFailed:
                        LogMsg("The Flic verification failed.");
                        break;
                    case FLICButtonScannerStatusEvent.Verified:
                        LogMsg("The Flic was verified successfully.");
                        break;
                }
            }, (FLICButton fLICButton, NSError error) =>
            {
                LogMsg($"Scanner completed with error: {error?.Description}");
                if (error == null)
                {
                    LogMsg($"Successfully verified: {fLICButton?.Name}, {fLICButton?.BluetoothAddress}, {fLICButton?.SerialNumber}");
                    // Listen to single click only.
                    fLICButton.TriggerMode = FLICButtonTriggerMode.FLICButtonTriggerModeClick;
                }
            });
        }

        class MyFLICManagerDelegate : FLICManagerDelegate
        {
            private ViewController _owner;
            public MyFLICManagerDelegate(ViewController owner)
            {
                _owner = owner;
            }

            public override void Manager(FLICManager manager, FLICManagerState state)
            {
                switch (state)
                {
                    case FLICManagerState.PoweredOff:
                        _owner.LogMsg("Bluetooth is turned off");
                        break;
                    case FLICManagerState.Unknown:
                        _owner.LogMsg("FLICManagerStateUnknown");
                        break;
                    case FLICManagerState.Resetting:
                        _owner.LogMsg("FLICManagerStateResetting");
                        break;
                    case FLICManagerState.Unsupported:
                        _owner.LogMsg("FLICManagerStateUnsupported");
                        break;
                    case FLICManagerState.Unauthorized:
                        _owner.LogMsg("FLICManagerStateUnauthorized");
                        break;
                    case FLICManagerState.PoweredOn:
                        _owner.LogMsg("Bluetooth is turned on");
                        break;
                }
            }

            public override void ManagerDidRestoreState(FLICManager manager)
            {
                foreach (var button in manager.Buttons)
                {
                    _owner.LogMsg($"Did restore Flic: {button.Name}");
                }
            }
        }

        class MyFLICButtonDelegate : FLICButtonDelegate
        {
            private ViewController _owner;
            public MyFLICButtonDelegate(ViewController owner)
            {
                _owner = owner;
            }

            public override void ButtonDidConnect(FLICButton button)
            {
                _owner.LogMsg($"Did connect Flic: {button.Name}");
            }

            public override void ButtonIsReady(FLICButton button)
            {
                //_owner.LogMsg($"Did connect Flic: {button.Name}");
            }

            public override void DidDisconnectWithError(FLICButton button, NSError error)
            {
                _owner.LogMsg($"Did disconnect Flic: {button.Name} Error:{error?.Description}");
            }

            public override void DidFailToConnectWithError(FLICButton button, NSError error)
            {
                
            }

            public override void DidReceiveButtonClick(FLICButton button, bool queued, nint age)
            {
                _owner.LogMsg($"Flic: {button.Name} was clicked");
            }
        }
    }
}