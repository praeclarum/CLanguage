using NUnit.Framework;
using CLanguage.Types;
using CLanguage.Interpreter;
using CLanguage.Compiler;
using System.Collections.Generic; // Required for List<CStructMember>

namespace CLanguage.Tests
{
    [TestFixture]
    public class VirtualMethodTests : TestsBase
    {
        MachineInfo TestMachineInfo => CLanguageTests.TestMachineInfo.Instance;

        // Helper to create a basic EmitContext for tests
        EmitContext CreateTestEmitContext() 
        {
            var executable = new Executable(TestMachineInfo);
            var report = new Report(new CompilerOptions());
            // For these tests, a simple ExecutableContext might be sufficient
            // as we are not fully compiling functions, just testing type layout.
            return new ExecutableContext(executable, report);
        }

        [Test]
        public void TestStructSizeWithBaseClass()
        {
            var emitContext = CreateTestEmitContext();

            // Base Class
            var baseStruct = new CStructType("Base");
            var intField = new CStructField { Name = "baseInt", MemberType = CBasicType.SignedInt };
            baseStruct.Members.Add(intField);
            // Manually calculate expected base size, assuming no vtable pointer for this simple test yet
            // Or rely on GetByteSize after members are set if it doesn't assume finalization for size.
            // For this test, we're focused on member layout size.
            // Let CStructType calculate its own size based on members.
            // If FinalizeLayout is needed for GetByteSize to be accurate, we might need to call it
            // or ensure GetByteSize works before full finalization for layout tests.
            // The current CStructType.GetByteSize directly iterates members.

            int expectedBaseSize = intField.MemberType.GetByteSize(emitContext);
            Assert.AreEqual(expectedBaseSize, baseStruct.GetByteSize(emitContext), "Base class size mismatch before derived.");


            // Derived Class
            var derivedStruct = new CStructType("Derived");
            var floatField = new CStructField { Name = "derivedFloat", MemberType = CBasicType.Float };
            derivedStruct.Members.Add(floatField);
            derivedStruct.BaseClass = baseStruct;

            int expectedDerivedOwnSize = floatField.MemberType.GetByteSize(emitContext);
            int expectedTotalSize = baseStruct.GetByteSize(emitContext) + expectedDerivedOwnSize;
            
            // Note: CStructType.GetByteSize as implemented in previous subtasks
            // already includes BaseClass.GetByteSize().
            Assert.AreEqual(expectedTotalSize, derivedStruct.GetByteSize(emitContext), "Derived class total size mismatch.");
        }

        [Test]
        public void TestMemberOffsetWithBaseClass()
        {
            var emitContext = CreateTestEmitContext();

            // Base Class
            var baseStruct = new CStructType("Base");
            var intFieldBase = new CStructField { Name = "baseInt", MemberType = CBasicType.SignedInt };
            var charFieldBase = new CStructField { Name = "baseChar", MemberType = CBasicType.SignedChar };
            baseStruct.Members.Add(intFieldBase);
            baseStruct.Members.Add(charFieldBase);
            // Base size: int (4) + char (1) = 5. Assuming alignment/packing makes it sum directly.
            // GetByteSize will use MachineInfo for actual sizes.
            
            // Derived Class
            var derivedStruct = new CStructType("Derived");
            var floatFieldDerived = new CStructField { Name = "derivedFloat", MemberType = CBasicType.Float };
            derivedStruct.Members.Add(floatFieldDerived);
            derivedStruct.BaseClass = baseStruct;

            // Offsets:
            // baseInt: offset 0 within base, so 0 in derived.
            // baseChar: offset of intFieldBase.GetByteSize() within base.
            // derivedFloat: offset of baseStruct.GetByteSize() + 0 (as it's the first in derived).

            int offsetBaseInt = derivedStruct.GetFieldValueOffset(intFieldBase, emitContext);
            Assert.AreEqual(0, offsetBaseInt, "Offset of first base class member in derived class incorrect.");

            int expectedOffsetBaseChar = intFieldBase.MemberType.GetByteSize(emitContext);
            int offsetBaseChar = derivedStruct.GetFieldValueOffset(charFieldBase, emitContext);
            Assert.AreEqual(expectedOffsetBaseChar, offsetBaseChar, "Offset of second base class member in derived class incorrect.");
            
            int expectedOffsetDerivedFloat = baseStruct.GetByteSize(emitContext); // Offset is after the entire base class block
            int offsetDerivedFloat = derivedStruct.GetFieldValueOffset(floatFieldDerived, emitContext);
            Assert.AreEqual(expectedOffsetDerivedFloat, offsetDerivedFloat, "Offset of derived class member incorrect.");
        }

        [Test]
        public void TestVTablePopulationSimpleInheritance()
        {
            var emitContext = CreateTestEmitContext();

            // Base Class
            var baseS = new CStructType("BaseS");
            var func1Type = new CFunctionType(CBasicType.Void, isInstance: true, declaringType: baseS); 
            // Parameters could be added to func1Type if needed for signature uniqueness
            var base_virtual_func1 = new CStructMethod { Name = "virtual_func1", MemberType = func1Type };
            baseS.Members.Add(base_virtual_func1);
            
            baseS.FinalizeLayout(emitContext); // This should populate BaseS.VTable
            Assert.IsNotNull(baseS.VTable, "Base VTable should not be null after FinalizeLayout.");
            Assert.AreEqual(1, baseS.VTable.Count, "Base VTable should have 1 entry.");
            Assert.AreEqual("virtual_func1", baseS.VTable[0].Name, "Base VTable entry name mismatch.");
            // Assert.AreEqual(baseS.Name, baseS.VTable[0].NameContext, "Base VTable entry context should be BaseS name.");

            // Derived Class
            var derivedS = new CStructType("DerivedS");
            derivedS.BaseClass = baseS;

            // Override virtual_func1
            var derived_virtual_func1 = new CStructMethod { Name = "virtual_func1", MemberType = func1Type }; // Same signature
            derivedS.Members.Add(derived_virtual_func1);

            // New virtual function in Derived
            var func2Type = new CFunctionType(CBasicType.SignedInt, isInstance: true, declaringType: derivedS);
            var derived_virtual_func2 = new CStructMethod { Name = "virtual_func2", MemberType = func2Type };
            derivedS.Members.Add(derived_virtual_func2);

            derivedS.FinalizeLayout(emitContext);
            Assert.IsNotNull(derivedS.VTable, "Derived VTable should not be null after FinalizeLayout.");
            Assert.AreEqual(2, derivedS.VTable.Count, "Derived VTable should have 2 entries.");

            // Check overridden virtual_func1
            Assert.AreEqual("virtual_func1", derivedS.VTable[0].Name, "Derived VTable entry 0 (overridden) name mismatch.");
            // The CompiledFunction in VTable has NameContext set to the struct that *defines* this version of the method.
            Assert.AreEqual(derivedS.Name, derivedS.VTable[0].NameContext, "Derived VTable entry 0 (overridden) should point to DerivedS's method.");
            
            // Check new virtual_func2
            Assert.AreEqual("virtual_func2", derivedS.VTable[1].Name, "Derived VTable entry 1 (new) name mismatch.");
            Assert.AreEqual(derivedS.Name, derivedS.VTable[1].NameContext, "Derived VTable entry 1 (new) should point to DerivedS's method.");
        }
    }
}
