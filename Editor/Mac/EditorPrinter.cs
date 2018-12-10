using System.Collections.Generic;

namespace CLanguage.Editor
{
    class EditorPrinter : Report.Printer
    {
        public readonly List<Report.AbstractMessage> Messages = new List<Report.AbstractMessage> ();
        public override void Print (Report.AbstractMessage msg)
        {
            base.Print (msg);
            Messages.Add (msg);
        }
    }
}
