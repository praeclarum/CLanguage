# Jay Parser Generator

## Overview

`jay` is a Yacc parser generator for C#.

It is used in this repository to generate the C# parser code for the C language.  

More information can be found here:
https://github.com/brannon/cs-yacc

## Building

```bash
gcc -DSKEL_DIRECTORY='"."' -o jay src/closure.c src/error.c src/lalr.c src/lr0.c src/main.c src/mkpar.c src/output.c src/reader.c src/symtab.c src/verbose.c src/warshall.c
```

## How to use it

This repository contains a bash script to compile and execute `jay`.

They are located here:
[prebuild.sh](/CLanguage/prebuild.sh)

It can be run from within the `CLanguage` directory with:

```bash
bash prebuild.sh
```
