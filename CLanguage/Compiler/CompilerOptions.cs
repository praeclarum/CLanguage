using System;
using CLanguage.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CLanguage.Compiler
{
    public class CompilerOptions
    {
        public readonly MachineInfo MachineInfo;
        public readonly Report Report;
        public readonly Document[] Documents;

        public CompilerOptions (MachineInfo machineInfo, Report report, IEnumerable<Document> documents)
        {
            MachineInfo = machineInfo;
            Report = report;
            Documents = documents.ToArray ();
        }

        public CompilerOptions (MachineInfo machineInfo)
            : this (machineInfo, new Report (), Enumerable.Empty<Document> ())
        {
        }
    }
}
