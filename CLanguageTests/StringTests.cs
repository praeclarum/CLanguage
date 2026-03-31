using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
    [TestClass]
    public class StringTests
    {
        CInterpreter Run (string code)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var i = CLanguageService.CreateInterpreter (fullCode, new ArduinoTestMachineInfo (), printer: new TestPrinter ());
            i.Reset ("start");
            i.Run ();
            return i;
        }

        [TestMethod]
        public void SingleChar ()
        {
            var i = Run (@"
char f = 'f';
void main () {
    assertAreEqual ('f', f);
}");
        }

        [TestMethod]
        public void NullTerminated ()
        {
            var i = Run (@"
char *bar = ""bar"";
void main () {
    assertAreEqual ('b', bar[0]);
    assertAreEqual ('a', bar[1]);
    assertAreEqual ('r', bar[2]);
    assertAreEqual (0, bar[3]);
}");
        }

        [TestMethod]
        public void NullTerminatedEmpty ()
        {
            var i = Run (@"
char *bar = """";
void main () {
    assertAreEqual (0, bar[0]);
}");
        }

        [TestMethod]
        public void Newline ()
        {
            var i = Run (@"
char *bar = ""b\nr"";
void main () {
    assertAreEqual ('b', bar[0]);
    assertAreEqual ('\n', bar[1]);
    assertAreEqual ('r', bar[2]);
    assertAreEqual (0, bar[3]);
}");
        }

        [TestMethod]
        public void NullCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\0';
    assertAreEqual (0, c);
}");
        }

        [TestMethod]
        public void NullEscapeInString ()
        {
            var i = Run (@"
char *bar = ""ab\0cd"";
void main () {
    assertAreEqual ('a', bar[0]);
    assertAreEqual ('b', bar[1]);
    assertAreEqual (0, bar[2]);
    assertAreEqual ('c', bar[3]);
    assertAreEqual ('d', bar[4]);
}");
        }

        [TestMethod]
        public void Multiline ()
        {
            var i = Run (@"
char *bar = ""a\
b\   
c"";
void main () {
    assertAreEqual ('a', bar[0]);
    assertAreEqual ('b', bar[1]);
    assertAreEqual ('c', bar[2]);
    assertAreEqual (0, bar[3]);
}");
        }

        [TestMethod]
        public void EscapedSingleQuoteCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\'';
    assertAreEqual (39, c);
}");
        }

        [TestMethod]
        public void HexEscapeCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\x41';
    assertAreEqual (65, c);
}");
        }

        [TestMethod]
        public void HexEscapeLowercaseCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\x61';
    assertAreEqual (97, c);
}");
        }

        [TestMethod]
        public void HexEscapeSingleDigitCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\xA';
    assertAreEqual (10, c);
}");
        }

        [TestMethod]
        public void HexEscapeInString ()
        {
            var i = Run (@"
char *bar = ""a\x42z"";
void main () {
    assertAreEqual ('a', bar[0]);
    assertAreEqual ('B', bar[1]);
    assertAreEqual ('z', bar[2]);
    assertAreEqual (0, bar[3]);
}");
        }

        [TestMethod]
        public void BackslashCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\\';
    assertAreEqual (92, c);
}");
        }

        [TestMethod]
        public void TabCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\t';
    assertAreEqual (9, c);
}");
        }

        [TestMethod]
        public void CarriageReturnCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\r';
    assertAreEqual (13, c);
}");
        }

        [TestMethod]
        public void NewlineCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\n';
    assertAreEqual (10, c);
}");
        }

        [TestMethod]
        public void HexEscapeZeroCharLiteral ()
        {
            var i = Run (@"
void main () {
    char c = '\x00';
    assertAreEqual (0, c);
}");
        }

        [TestMethod]
        public void AllEscapesInOneFunction ()
        {
            var i = Run (@"
void main () {
    assertAreEqual (0, '\0');
    assertAreEqual (39, '\'');
    assertAreEqual (92, '\\');
    assertAreEqual (10, '\n');
    assertAreEqual (13, '\r');
    assertAreEqual (9, '\t');
    assertAreEqual (17, '\x11');
    assertAreEqual (255, '\xFF');
}");
        }
    }
}
