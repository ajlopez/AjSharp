# AjSharp

C#-like interpreted programming language, implemented using C#, with access .NET types and objects. It has
experimental actor model, distributed objects and software transactional memory.

Imported from https://ajcodekatas.googlecode.com/svn/trunk/AjLanguage

## Compiler

It compiles Smalltalk code to bytecodes, and run the bytecodes. Or it compiles to JavaScript code, to be run
by [AjTalkJs](https://github.com/ajlopez/AjTalkJs).

The idea is to traverse the internal AST to compile to different languages. Next target: Python.

## References

- [http://stackoverflow.com/questions/972/adding-a-method-to-an-existing-object](http://stackoverflow.com/questions/972/adding-a-method-to-an-existing-object)