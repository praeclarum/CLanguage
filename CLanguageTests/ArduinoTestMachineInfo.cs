using System;

using NUnit.Framework;

#if NETFX_CORE
using TestFixtureAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using TestAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif VS_UNIT_TESTING
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

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
			Assert.That (actual, Is.EqualTo (expected));
		}
	}
}

