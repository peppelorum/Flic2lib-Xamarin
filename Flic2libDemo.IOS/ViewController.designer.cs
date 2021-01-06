// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Flic2libDemo.IOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UITextView LogTextView { get; set; }

		[Outlet]
		UIKit.UIButton StartButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (StartButton != null) {
				StartButton.Dispose ();
				StartButton = null;
			}

			if (LogTextView != null) {
				LogTextView.Dispose ();
				LogTextView = null;
			}
		}
	}
}
