using System.Threading.Tasks;
using Foundation;
using UIKit;
using System;
using System.IO;

namespace CEditor
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UINavigationBar.Appearance.BarStyle = UIBarStyle.Black;
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = new UINavigationController(new ViewController(GetInitialUrlAsync ()));
            Window.MakeKeyAndVisible();

            return true;
        }

        Task<NSUrl> GetInitialUrlAsync ()
        {
            return Task.Run (() => {
                var docs = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
                var file = Path.Combine (docs, "main.cpp");
                if (!File.Exists (file)) {
                    File.WriteAllText (file, "");
                }
                return NSUrl.FromFilename (file);
            });
        }

        public override void OnResignActivation(UIApplication application)
        {
        }

        public override void DidEnterBackground(UIApplication application)
        {
        }

        public override void WillEnterForeground(UIApplication application)
        {
        }

        public override void OnActivated(UIApplication application)
        {
        }

        public override void WillTerminate(UIApplication application)
        {
        }
    }
}

