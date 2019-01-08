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
            return Run (code, null, expectedErrors);
        }

        protected CInterpreter Run (string code, MachineInfo mi, params int[] expectedErrors)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var printer = new TestPrinter (expectedErrors);
            var i = CLanguageService.CreateInterpreter (fullCode, mi ?? new TestMachineInfo (), printer: printer);
            printer.CheckForErrors ();
            i.Reset ("start");
            i.Step ();
            return i;
        }

        protected Executable Compile (string code, params int[] expectedErrors)
        {
            return Compile (code, null, expectedErrors);
        }

        protected Executable Compile (string code, MachineInfo mi, params int[] expectedErrors)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var printer = new TestPrinter (expectedErrors);
            var i = CLanguageService.CreateInterpreter (fullCode, mi ?? new TestMachineInfo (), printer: printer);
            printer.CheckForErrors ();
            return i.Executable;
        }
    }
}

