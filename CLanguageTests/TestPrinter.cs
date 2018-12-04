using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
	class TestPrinter : Report.Printer
	{
		int[] expectedErrors;
		List<int> encounteredErrors;

		public TestPrinter (params int[] expectedErrors)
		{
			this.expectedErrors = expectedErrors;
			encounteredErrors = new List<int> ();
		}

		public override void Print (Report.AbstractMessage msg)
		{
			if (msg.MessageType != "Info") {
				encounteredErrors.Add (msg.Code);
				if (!expectedErrors.Contains (msg.Code)) {
					Assert.Fail (msg.ToString ());
				}
			}
		}

		public void CheckForErrors ()
		{
			foreach (var e in expectedErrors) {
				if (!encounteredErrors.Contains (e)) {
					Assert.Fail ("Expected error " + e + " but never got it.");
				}
			}
		}
	}
}

