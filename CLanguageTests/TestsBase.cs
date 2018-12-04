using System;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;

namespace CLanguage.Tests
{
    public class TestsBase
    {
        protected CInterpreter Run (string code, params int[] expectedErrors)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var printer = new TestPrinter (expectedErrors);
            var i = CLanguageService.CreateInterpreter (fullCode, new TestMachineInfo (), printer: printer);
            printer.CheckForErrors ();
            i.Reset ("start");
            i.Step ();
            return i;
        }
    }
}

