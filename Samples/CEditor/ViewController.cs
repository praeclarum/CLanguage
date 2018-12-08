using System;

using AppKit;
using Foundation;

namespace CEditor
{
    public partial class ViewController : NSViewController
    {
        public Document Document { get; set; }

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override NSObject RepresentedObject
        {
            get => Document;
            set => Document = value as Document;
        }
    }
}
