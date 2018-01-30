using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
	public class ArduinoTestMachineInfo : ArduinoMachineInfo
	{
		public ArduinoTestMachineInfo ()
		{
			InternalFunctions.Add (new InternalFunction ("void assertAreEqual (int expected, int actual)", AssertAreEqual));
		}

		static void AssertAreEqual (ExecutionState state)
		{
			var expected = state.ActiveFrame.Args[0];
			var actual = state.ActiveFrame.Args[1];
			Assert.AreEqual (actual, expected);
		}
	}
}

