using System;
using System.Collections.Generic;
using CLanguage.Compiler;
using CLanguage.Interpreter;
using CLanguage.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class VTableTypeTests
    {
        EmitContext _c = new ExecutableContext (new Executable (MachineInfo.Windows32), new Report (new Report.TextWriterPrinter (Console.Out)));

        static CStructField MakeField (string name, CType type)
        {
            return new CStructField { Name = name, MemberType = type };
        }

        static CStructMethod MakeVirtualMethod (string name, CFunctionType sig)
        {
            return new CStructMethod { Name = name, MemberType = sig, IsVirtual = true };
        }

        static CStructMethod MakeOverrideMethod (string name, CFunctionType sig)
        {
            return new CStructMethod { Name = name, MemberType = sig, IsOverride = true };
        }

        static CFunctionType MakeMethodSig (CStructType declaringType)
        {
            return new CFunctionType (CBasicType.SignedInt, isInstance: true, declaringType: declaringType);
        }

        // ---- Non-polymorphic types: zero-cost guarantee ----

        [TestMethod]
        public void NonPolymorphicStructIsNotPolymorphic ()
        {
            var s = new CStructType ("Plain");
            s.Members.Add (MakeField ("x", CBasicType.SignedInt));
            s.Members.Add (MakeField ("y", CBasicType.SignedInt));
            Assert.IsFalse (s.IsPolymorphic);
            Assert.IsFalse (s.HasVTable);
        }

        [TestMethod]
        public void NonPolymorphicNumValuesUnchanged ()
        {
            var s = new CStructType ("Plain");
            s.Members.Add (MakeField ("x", CBasicType.SignedInt));
            s.Members.Add (MakeField ("y", CBasicType.SignedInt));
            Assert.AreEqual (2, s.NumValues);
        }

        [TestMethod]
        public void NonPolymorphicByteSizeUnchanged ()
        {
            var s = new CStructType ("Plain");
            s.Members.Add (MakeField ("x", CBasicType.SignedInt));
            s.Members.Add (MakeField ("y", CBasicType.SignedInt));
            Assert.AreEqual (8, s.GetByteSize (_c)); // 2 * 4 bytes
        }

        [TestMethod]
        public void NonPolymorphicFieldOffsetUnchanged ()
        {
            var s = new CStructType ("Plain");
            var fx = MakeField ("x", CBasicType.SignedInt);
            var fy = MakeField ("y", CBasicType.SignedInt);
            s.Members.Add (fx);
            s.Members.Add (fy);
            Assert.AreEqual (0, s.GetFieldValueOffset (fx, _c));
            Assert.AreEqual (1, s.GetFieldValueOffset (fy, _c));
        }

        // ---- IsPolymorphic ----

        [TestMethod]
        public void TypeWithVTableIsPolymorphic ()
        {
            var s = new CStructType ("Base");
            var method = MakeVirtualMethod ("foo", MakeMethodSig (s));
            s.Members.Add (method);
            s.BuildVTable ();
            Assert.IsTrue (s.HasVTable);
            Assert.IsTrue (s.IsPolymorphic);
        }

        [TestMethod]
        public void DerivedFromPolymorphicIsPolymorphic ()
        {
            var baseType = new CStructType ("Base");
            var method = MakeVirtualMethod ("foo", MakeMethodSig (baseType));
            baseType.Members.Add (method);
            baseType.BuildVTable ();

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            derived.Members.Add (MakeField ("z", CBasicType.SignedInt));
            derived.BuildVTable ();

            Assert.IsTrue (derived.IsPolymorphic);
        }

        [TestMethod]
        public void NonVirtualDerivedIsNotPolymorphic ()
        {
            var baseType = new CStructType ("Base");
            baseType.Members.Add (MakeField ("x", CBasicType.SignedInt));

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            derived.Members.Add (MakeField ("y", CBasicType.SignedInt));

            Assert.IsFalse (derived.IsPolymorphic);
            Assert.IsNull (derived.BaseType.VTable);
        }

        // ---- NumValues with polymorphism ----

        [TestMethod]
        public void PolymorphicNumValuesIncludesVptr ()
        {
            var s = new CStructType ("Base");
            s.Members.Add (MakeField ("x", CBasicType.SignedInt));
            var method = MakeVirtualMethod ("foo", MakeMethodSig (s));
            s.Members.Add (method);
            s.BuildVTable ();

            // 1 (vptr) + 1 (x) = 2
            Assert.AreEqual (2, s.NumValues);
        }

        [TestMethod]
        public void DerivedNumValuesIncludesBaseFields ()
        {
            var baseType = new CStructType ("Base");
            baseType.Members.Add (MakeField ("x", CBasicType.SignedInt));
            var method = MakeVirtualMethod ("foo", MakeMethodSig (baseType));
            baseType.Members.Add (method);
            baseType.BuildVTable ();

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            derived.Members.Add (MakeField ("y", CBasicType.SignedInt));
            derived.BuildVTable ();

            // 1 (vptr) + 1 (base x) + 1 (own y) = 3
            Assert.AreEqual (3, derived.NumValues);
        }

        // ---- GetByteSize with polymorphism ----

        [TestMethod]
        public void PolymorphicByteSizeIncludesVptr ()
        {
            var s = new CStructType ("Base");
            s.Members.Add (MakeField ("x", CBasicType.SignedInt));
            var method = MakeVirtualMethod ("foo", MakeMethodSig (s));
            s.Members.Add (method);
            s.BuildVTable ();

            // 4 (vptr) + 4 (x) = 8
            Assert.AreEqual (8, s.GetByteSize (_c));
        }

        [TestMethod]
        public void DerivedByteSizeIncludesBaseFields ()
        {
            var baseType = new CStructType ("Base");
            baseType.Members.Add (MakeField ("x", CBasicType.SignedInt));
            var method = MakeVirtualMethod ("foo", MakeMethodSig (baseType));
            baseType.Members.Add (method);
            baseType.BuildVTable ();

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            derived.Members.Add (MakeField ("y", CBasicType.SignedInt));
            derived.BuildVTable ();

            // 4 (vptr) + 4 (base x) + 4 (own y) = 12
            Assert.AreEqual (12, derived.GetByteSize (_c));
        }

        // ---- GetFieldValueOffset with polymorphism ----

        [TestMethod]
        public void PolymorphicFieldOffsetSkipsVptr ()
        {
            var s = new CStructType ("Base");
            var fx = MakeField ("x", CBasicType.SignedInt);
            s.Members.Add (fx);
            var method = MakeVirtualMethod ("foo", MakeMethodSig (s));
            s.Members.Add (method);
            s.BuildVTable ();

            // vptr at 0, x at 1
            Assert.AreEqual (1, s.GetFieldValueOffset (fx, _c));
        }

        [TestMethod]
        public void DerivedFieldOffsetIncludesBaseFields ()
        {
            var baseType = new CStructType ("Base");
            var fx = MakeField ("x", CBasicType.SignedInt);
            baseType.Members.Add (fx);
            var method = MakeVirtualMethod ("foo", MakeMethodSig (baseType));
            baseType.Members.Add (method);
            baseType.BuildVTable ();

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            var fy = MakeField ("y", CBasicType.SignedInt);
            derived.Members.Add (fy);
            derived.BuildVTable ();

            // vptr(0), base x(1), own y(2)
            Assert.AreEqual (1, derived.GetFieldValueOffset (fx, _c));
            Assert.AreEqual (2, derived.GetFieldValueOffset (fy, _c));
        }

        // ---- VTable construction ----

        [TestMethod]
        public void BuildVTableCreatesSlots ()
        {
            var s = new CStructType ("Base");
            var sig = MakeMethodSig (s);
            var m1 = MakeVirtualMethod ("foo", sig);
            var m2 = MakeVirtualMethod ("bar", sig);
            s.Members.Add (m1);
            s.Members.Add (m2);
            s.BuildVTable ();

            Assert.IsNotNull (s.VTable);
            Assert.AreEqual (2, s.VTable!.Count);
            Assert.AreEqual ("foo", s.VTable[0].MethodName);
            Assert.AreEqual ("bar", s.VTable[1].MethodName);
            Assert.AreEqual (0, m1.VTableSlotIndex);
            Assert.AreEqual (1, m2.VTableSlotIndex);
        }

        [TestMethod]
        public void BuildVTableInheritsBaseSlots ()
        {
            var baseType = new CStructType ("Base");
            var baseSig = MakeMethodSig (baseType);
            var baseFoo = MakeVirtualMethod ("foo", baseSig);
            baseType.Members.Add (baseFoo);
            baseType.BuildVTable ();

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            var derivedSig = MakeMethodSig (derived);
            var derivedBar = MakeVirtualMethod ("bar", derivedSig);
            derived.Members.Add (derivedBar);
            derived.BuildVTable ();

            Assert.IsNotNull (derived.VTable);
            Assert.AreEqual (2, derived.VTable!.Count);
            Assert.AreEqual ("foo", derived.VTable[0].MethodName);
            Assert.AreEqual ("bar", derived.VTable[1].MethodName);
            Assert.AreEqual (0, derived.VTable[0].SlotIndex);
            Assert.AreEqual (1, derived.VTable[1].SlotIndex);
        }

        [TestMethod]
        public void BuildVTableOverridesBaseSlot ()
        {
            var baseType = new CStructType ("Base");
            var baseSig = MakeMethodSig (baseType);
            var baseFoo = MakeVirtualMethod ("foo", baseSig);
            baseType.Members.Add (baseFoo);
            baseType.BuildVTable ();

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            var derivedSig = MakeMethodSig (derived);
            // Virtual method with same name overrides implicitly
            var derivedFoo = MakeVirtualMethod ("foo", derivedSig);
            derived.Members.Add (derivedFoo);
            derived.BuildVTable ();

            Assert.IsNotNull (derived.VTable);
            Assert.AreEqual (1, derived.VTable!.Count);
            Assert.AreEqual ("foo", derived.VTable[0].MethodName);
            Assert.AreSame (derived, derived.VTable[0].DeclaringType);
            Assert.AreEqual (0, derivedFoo.VTableSlotIndex);
        }

        [TestMethod]
        public void BuildVTableExplicitOverride ()
        {
            var baseType = new CStructType ("Base");
            var baseSig = MakeMethodSig (baseType);
            var baseFoo = MakeVirtualMethod ("foo", baseSig);
            baseType.Members.Add (baseFoo);
            baseType.BuildVTable ();

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            var derivedSig = MakeMethodSig (derived);
            var derivedFoo = MakeOverrideMethod ("foo", derivedSig);
            derived.Members.Add (derivedFoo);
            derived.BuildVTable ();

            Assert.AreEqual (1, derived.VTable!.Count);
            Assert.AreSame (derived, derived.VTable[0].DeclaringType);
            Assert.AreEqual (0, derivedFoo.VTableSlotIndex);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void BuildVTableOverrideWithoutBaseThrows ()
        {
            var s = new CStructType ("Bad");
            var sig = MakeMethodSig (s);
            var method = MakeOverrideMethod ("nonexistent", sig);
            s.Members.Add (method);
            s.BuildVTable ();
        }

        [TestMethod]
        public void BuildVTableWithNoVirtualMethodsProducesNull ()
        {
            var s = new CStructType ("Plain");
            s.Members.Add (MakeField ("x", CBasicType.SignedInt));
            s.BuildVTable ();
            Assert.IsNull (s.VTable);
            Assert.IsFalse (s.HasVTable);
            Assert.IsFalse (s.IsPolymorphic);
        }

        // ---- GetOwnFieldsNumValues / GetOwnFieldsByteSize ----

        [TestMethod]
        public void GetOwnFieldsNumValuesExcludesMethods ()
        {
            var s = new CStructType ("S");
            s.Members.Add (MakeField ("x", CBasicType.SignedInt));
            s.Members.Add (new CStructMethod { Name = "foo", MemberType = MakeMethodSig (s) });
            s.Members.Add (MakeField ("y", CBasicType.SignedInt));
            Assert.AreEqual (2, s.GetOwnFieldsNumValues ());
        }

        [TestMethod]
        public void GetOwnFieldsByteSizeExcludesMethods ()
        {
            var s = new CStructType ("S");
            s.Members.Add (MakeField ("x", CBasicType.SignedInt));
            s.Members.Add (new CStructMethod { Name = "foo", MemberType = MakeMethodSig (s) });
            s.Members.Add (MakeField ("y", CBasicType.SignedInt));
            Assert.AreEqual (8, s.GetOwnFieldsByteSize (_c)); // 2 * 4 bytes
        }

        // ---- VTableEntry ----

        [TestMethod]
        public void VTableEntryToString ()
        {
            var s = new CStructType ("Base");
            var sig = MakeMethodSig (s);
            var entry = new VTableEntry (0, "foo", sig, s);
            Assert.IsTrue (entry.ToString ().Contains ("foo"));
            Assert.IsTrue (entry.ToString ().Contains ("0"));
        }

        // ---- CStructMethod flags ----

        [TestMethod]
        public void CStructMethodDefaultFlags ()
        {
            var method = new CStructMethod ();
            Assert.IsFalse (method.IsVirtual);
            Assert.IsFalse (method.IsOverride);
            Assert.IsFalse (method.IsPureVirtual);
            Assert.IsNull (method.VTableSlotIndex);
        }

        // ---- BaseType property ----

        [TestMethod]
        public void BaseTypeDefaultsToNull ()
        {
            var s = new CStructType ("S");
            Assert.IsNull (s.BaseType);
        }

        [TestMethod]
        public void BaseTypeCanBeSet ()
        {
            var baseType = new CStructType ("Base");
            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            Assert.AreSame (baseType, derived.BaseType);
        }

        // ---- Non-polymorphic base type (no virtual methods) ----

        [TestMethod]
        public void NonPolymorphicBaseNumValues ()
        {
            var baseType = new CStructType ("Base");
            baseType.Members.Add (MakeField ("x", CBasicType.SignedInt));

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            derived.Members.Add (MakeField ("y", CBasicType.SignedInt));

            // No vtable, no vptr: base x(1) + own y(1) = 2
            Assert.AreEqual (2, derived.NumValues);
        }

        [TestMethod]
        public void NonPolymorphicBaseByteSize ()
        {
            var baseType = new CStructType ("Base");
            baseType.Members.Add (MakeField ("x", CBasicType.SignedInt));

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            derived.Members.Add (MakeField ("y", CBasicType.SignedInt));

            // No vptr: 4 (base x) + 4 (own y) = 8
            Assert.AreEqual (8, derived.GetByteSize (_c));
        }

        [TestMethod]
        public void NonPolymorphicBaseFieldOffset ()
        {
            var baseType = new CStructType ("Base");
            var fx = MakeField ("x", CBasicType.SignedInt);
            baseType.Members.Add (fx);

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            var fy = MakeField ("y", CBasicType.SignedInt);
            derived.Members.Add (fy);

            // No vptr: base x(0), own y(1)
            Assert.AreEqual (0, derived.GetFieldValueOffset (fx, _c));
            Assert.AreEqual (1, derived.GetFieldValueOffset (fy, _c));
        }
    }
}
