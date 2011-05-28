using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class Compiler
    {
        CompiledBlock _rootBlock;
        CompilerContext _compilerCtx;

        public Compiler(Report report, MachineInfo m)
        {
            _compilerCtx = new CompilerContext (report, m);
        }

        public void Add(IFunction func)
        {
            if (_rootBlock == null)
            {
                _rootBlock = new CompiledBlock(null, null, _compilerCtx);
            }

            _rootBlock.AddFunction(func);
        }

        public void Add(TranslationUnit translationUnit)
        {
            var ctx = new TranslationUnitContext(this, _compilerCtx);
            translationUnit.Emit(ctx);
        }

        public void AddCode(string code)
        {
            var pp = new Preprocessor(new Config());
            pp.AddCode("stdin", code);
            var lexer = new Lexer(pp);
            var parser = new CParser();
            Add(parser.ParseTranslationUnit(lexer));
        }

        void IntegrateRootBlock(CompiledBlock block)
        {
            _rootBlock = block;
        }

        class CompiledFunction : CompiledBlock, IFunction
        {
            public string Name { get; private set; }
            public CFunctionType FunctionType { get; private set; }

            public CompiledFunction(FunctionDeclaration fdecl, CompiledBlock parent, CompilerContext c)
                : base(fdecl.Body, parent, c)
            {
                Name = fdecl.Name;
                FunctionType = fdecl.FunctionType;
            }
        }

        public interface IFunction
        {
            string Name { get; }
            CFunctionType FunctionType { get; }
        }

        class CompiledBlock : EmitContext
        {
            public CompiledBlock Parent { get; private set; }

            Dictionary<string, IFunction> Functions;
            Dictionary<string, VariableDeclaration> Variables;

            /*class LocalVariable
            {
                public int FunctionIndex;
                public int BlockIndex;
                public string Name;
                public CType VariableType;
            }*/

            public CompiledBlock(Block block, CompiledBlock parent, CompilerContext c)
                : base(c)
            {
                Parent = parent;
                Functions = new Dictionary<string, IFunction>();
                Variables = new Dictionary<string, VariableDeclaration>();

                var b = new System.Reflection.Emit.DynamicMethod("Foo", typeof(int), null);
                var g = b.GetILGenerator();
            }

            public void AddFunction(IFunction fun)
            {
                Functions[fun.Name] = fun;
            }

            public override void DeclareFunction(FunctionDeclaration f)
            {
                var fun = new CompiledFunction(f, this, Compiler);
                if (f.Body != null)
                {
                    f.Body.Emit(fun);
                }
                AddFunction(fun);                
            }

            public override void DeclareVariable(VariableDeclaration v)
            {
                Variables[v.Name] = v;
            }

            public override Expression ResolveVariable(string name, VariableExpression original)
            {
                var v = ResolveVariableR(name);

                if (v != null)
                {
                    return v;
                }
                else
                {
                    Report.Error(3861, "'{0}': identifier not found", name);
                    original.HasError = true;
                    return original;
                }
            }

            protected virtual Expression ResolveVariableR(string name)
            {
                VariableDeclaration v = null;
                if (Variables.TryGetValue(name, out v))
                {
                    return new LocalVariableRefExpression(v); 
                }
                
                IFunction f = null;
                if (Functions.TryGetValue(name, out f))
                {
                    return new FunctionRefExpression(f);
                }

                if (Parent != null)
                {
                    return Parent.ResolveVariableR(name);
                }
                else
                {
                    return null;
                }
            }

            class LocalVariableRefExpression : Expression
            {
                string _name;
                CType _type;

                public LocalVariableRefExpression(VariableDeclaration v)
                {
                    _name = v.Name;
                    _type = v.VariableType;
                }

                public override CType ExpressionType
                {
                    get { return _type; }
                }

                protected override Expression DoResolve(ResolveContext rc)
                {
                    return this;
                }

                protected override void DoEmit(EmitContext ec)
                {
                    ec.EmitLoadLocal(0);
                }

                public override string ToString()
                {
                    return _name;
                }
            }

            class FunctionRefExpression : Expression
            {
                string _name;
                CType _type;

                public FunctionRefExpression(IFunction f)
                {
                    _name = f.Name;
                    _type = f.FunctionType;
                }

                public override CType ExpressionType
                {
                    get { return _type; }
                }

                protected override Expression DoResolve(ResolveContext rc)
                {
                    return this;
                }

                protected override void DoEmit(EmitContext ec)
                {
                    throw new NotImplementedException();
                }
            }

			List<Op> _body = new List<Op>();

            public override void EmitBranchIfFalse(EmitContext.Label l)
            {
                Console.WriteLine("  BRFALSE " + l);
            }

            public override void EmitBranchIfTrue(EmitContext.Label l)
            {
                Console.WriteLine("  BRTRUE " + l);
            }

            public override void EmitJump(EmitContext.Label l)
            {
                Console.WriteLine("  JUMP " + l);
            }

            public override void EmitLabel(EmitContext.Label l)
            {
                Console.WriteLine(l + ":");
            }

            public override void EmitUnop(Unop op)
            {
                Console.WriteLine("  " + op);
            }

            public override void EmitLoadLocal(int index)
            {
                Console.WriteLine("  LDLOC " + index);
            }

            public override void EmitBinop(Binop op)
            {
                Console.WriteLine("  " + op);
            }

            public override void EmitCall(CFunctionType type, int argsCount)
            {
                Console.WriteLine("  -> " + type + "/" + argsCount);
            }

            public override void EmitConstant(object value, CType type)
            {
                Console.WriteLine("  PUSH " + value);
            }

            public override void EmitAssign(Expression left)
            {
                Console.WriteLine("  => " + left);
				//_body.Add (Ops.Assign ());
            }

            public override void EmitPop()
            {
                _body.Add (new PopOp ());
            }
        }

        class TranslationUnitContext : EmitContext
        {
            Compiler _i;

            CompiledBlock _currentBlock;

            public TranslationUnitContext(Compiler i, CompilerContext c)
                : base(c)
            {
                _i = i;
                _currentBlock = null;
            }

            public override void BeginBlock(Block block)
            {
                var b = new CompiledBlock(block, _currentBlock, Compiler);
                _currentBlock = b;
            }

            public override void EndBlock()
            {
                if (_currentBlock.Parent == null)
                {
                    _i.IntegrateRootBlock(_currentBlock);
                }
                _currentBlock = _currentBlock.Parent;
            }

            public override void DeclareFunction(FunctionDeclaration f)
            {
                _currentBlock.DeclareFunction(f);
            }

            public override void DeclareVariable(VariableDeclaration v)
            {
                _currentBlock.DeclareVariable(v);
            }
        }
    }
}
