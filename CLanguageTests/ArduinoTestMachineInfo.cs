using System;
using NUnit.Framework;

namespace CLanguage.Tests
{
	public class ArduinoTestMachineInfo : ArduinoMachineInfo
	{
		public ArduinoTestMachineInfo ()
		{
			InternalFunctions.Add (new InternalFunction ("void assertAreEqual (int expected, int actual)"));
		}
	}
}

