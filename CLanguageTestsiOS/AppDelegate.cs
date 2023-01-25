using Foundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIKit;

namespace CLanguageTestsiOS
{
    [Register ("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate {
    
        [Export("window")]
        public UIWindow Window { get; set; }

        [Export ("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = new UINavigationController (new UnitTestingViewController ());
            Window.MakeKeyAndVisible();
            return true;
        }
    }
}


