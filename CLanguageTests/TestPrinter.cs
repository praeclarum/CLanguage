using System;
using System.Linq;
using System.Collections.Generic;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#endif

namespace CLanguage.Tests
{
	class TestPrinter : ReportPrinter
	{
		int[] expectedErrors;
		List<int> encounteredErrors;

		public TestPrinter (params int[] expectedErrors)
		{
			this.expectedErrors = expectedErrors;
			encounteredErrors = new List<int> ();
		}

		public override void Print (AbstractMessage msg)
		{
			if (msg.MessageType == "Error") {
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

