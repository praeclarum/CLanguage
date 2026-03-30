#!/bin/sh
set -e
cd ../Jay && gcc -DSKEL_DIRECTORY='"."' -o jay src/closure.c src/error.c src/lalr.c src/lr0.c src/main.c src/mkpar.c src/output.c src/reader.c src/symtab.c src/verbose.c src/warshall.c
cd ../CLanguage/Parser && ../../Jay/jay -vc CParser.jay < ../../Jay/skeleton.cs > CParser.cs
