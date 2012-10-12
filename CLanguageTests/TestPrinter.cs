using System;
using NUnit.Framework;

namespace CLanguage.Tests
{
	class TestPrinter : ReportPrinter
	{
		public override void Print (AbstractMessage msg)
		{
			if (msg.MessageType == "Error") {
				Assert.Fail (msg.ToString ());
			}
			else {
				Console.WriteLine (msg);
			}
		}
	}
}

