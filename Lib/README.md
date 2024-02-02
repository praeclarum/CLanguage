# What is `jay`

`jay` is a Yacc parser generator for C#.

It is used in this repository to generate the C# parser code for the C language.  
This repository contains a precompiled binary of `jay` for macOS/iOS (x86-64) and Windows (x86-64).

The source can be found here:
https://github.com/mono/mono/tree/main/mcs/jay

More information can be found here:
https://github.com/brannon/cs-yacc

The exact version of `jay` this repository contains precompiled binaries for is:
https://github.com/mono/mono/tree/38b0227c1ce0c53058a5d78d080923435132773a/mcs/jay

# How to use it

This repository contains a shell and powershell script to execute `jay`.

They are located here:
[prebuild.sh](/CLanguage/prebuild.sh)
[prebuild.ps1](/CLanguage/prebuild.ps1)