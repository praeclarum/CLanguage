using System;
using System.Collections.Generic;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class StructLayoutTests : TestsBase
    {
        [TestMethod]
        public void SimpleStructFieldOffsets ()
        {
            // struct Servo { int pin; int servoIndex; int min; int max; };
            var servo = new CStructType ("Servo");
            servo.Members.Add (new CStructField { Name = "pin", MemberType = CBasicType.SignedInt });
            servo.Members.Add (new CStructField { Name = "servoIndex", MemberType = CBasicType.UnsignedChar });
            servo.Members.Add (new CStructField { Name = "min", MemberType = CBasicType.SignedChar });
            servo.Members.Add (new CStructField { Name = "max", MemberType = CBasicType.SignedChar });
            servo.Members.Add (new CStructField { Name = "lastDegrees", MemberType = CBasicType.SignedInt });
            servo.Members.Add (new CStructField { Name = "lastMicroseconds", MemberType = CBasicType.SignedInt });

            var layout = new StructLayout (servo);
            Assert.AreEqual ("Servo", layout.Name);
            Assert.AreEqual (6, layout.NumValues);

            Assert.AreEqual (0, layout.Field ("pin").Offset);
            Assert.AreEqual (1, layout.Field ("servoIndex").Offset);
            Assert.AreEqual (2, layout.Field ("min").Offset);
            Assert.AreEqual (3, layout.Field ("max").Offset);
            Assert.AreEqual (4, layout.Field ("lastDegrees").Offset);
            Assert.AreEqual (5, layout.Field ("lastMicroseconds").Offset);
        }

        [TestMethod]
        public void FieldAccessorNumValues ()
        {
            var s = new CStructType ("S");
            s.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });
            s.Members.Add (new CStructField { Name = "arr", MemberType = new CArrayType (CBasicType.SignedInt, 5) });
            s.Members.Add (new CStructField { Name = "y", MemberType = CBasicType.Float });

            var layout = new StructLayout (s);
            Assert.AreEqual (1, layout.Field ("x").NumValues);
            Assert.AreEqual (5, layout.Field ("arr").NumValues);
            Assert.AreEqual (1, layout.Field ("y").NumValues);

            Assert.AreEqual (0, layout.Field ("x").Offset);
            Assert.AreEqual (1, layout.Field ("arr").Offset);
            Assert.AreEqual (6, layout.Field ("y").Offset);
        }

        [TestMethod]
        public void FieldAccessorGetSet ()
        {
            var s = new CStructType ("Point");
            s.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });
            s.Members.Add (new CStructField { Name = "y", MemberType = CBasicType.SignedInt });

            var layout = new StructLayout (s);
            var fx = layout.Field ("x");
            var fy = layout.Field ("y");

            var stack = new Value[10];
            int basePtr = 3; // struct starts at offset 3

            fx.Set (stack, basePtr, 42);
            fy.Set (stack, basePtr, 99);

            Assert.AreEqual (42, fx.Get (stack, basePtr).Int32Value);
            Assert.AreEqual (99, fy.Get (stack, basePtr).Int32Value);
        }

        [TestMethod]
        public void FieldAccessorGetAddress ()
        {
            var s = new CStructType ("S");
            s.Members.Add (new CStructField { Name = "a", MemberType = CBasicType.SignedInt });
            s.Members.Add (new CStructField { Name = "b", MemberType = CBasicType.SignedInt });

            var layout = new StructLayout (s);
            Assert.AreEqual (10, layout.Field ("a").GetAddress (10));
            Assert.AreEqual (11, layout.Field ("b").GetAddress (10));
        }

        [TestMethod]
        public void NestedStructFieldLayout ()
        {
            // struct Inner { int a; int b; };
            var inner = new CStructType ("Inner");
            inner.Members.Add (new CStructField { Name = "a", MemberType = CBasicType.SignedInt });
            inner.Members.Add (new CStructField { Name = "b", MemberType = CBasicType.SignedInt });

            // struct Outer { int x; Inner nested; int y; };
            var outer = new CStructType ("Outer");
            outer.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });
            outer.Members.Add (new CStructField { Name = "nested", MemberType = inner });
            outer.Members.Add (new CStructField { Name = "y", MemberType = CBasicType.SignedInt });

            var layout = new StructLayout (outer);
            Assert.AreEqual (4, layout.NumValues); // x(1) + Inner(2) + y(1)

            Assert.AreEqual (0, layout.Field ("x").Offset);
            Assert.AreEqual (1, layout.Field ("nested").Offset);
            Assert.AreEqual (2, layout.Field ("nested").NumValues);
            Assert.AreEqual (3, layout.Field ("y").Offset);

            // Get nested layout
            var nestedLayout = layout.FieldLayout ("nested");
            Assert.AreEqual ("Inner", nestedLayout.Name);
            Assert.AreEqual (2, nestedLayout.NumValues);
            Assert.AreEqual (0, nestedLayout.Field ("a").Offset);
            Assert.AreEqual (1, nestedLayout.Field ("b").Offset);
        }

        [TestMethod]
        public void NestedStructReadWrite ()
        {
            // Test reading/writing nested struct fields via FieldAccessor
            var inner = new CStructType ("Inner");
            inner.Members.Add (new CStructField { Name = "a", MemberType = CBasicType.SignedInt });
            inner.Members.Add (new CStructField { Name = "b", MemberType = CBasicType.SignedInt });

            var outer = new CStructType ("Outer");
            outer.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });
            outer.Members.Add (new CStructField { Name = "nested", MemberType = inner });
            outer.Members.Add (new CStructField { Name = "y", MemberType = CBasicType.SignedInt });

            var outerLayout = new StructLayout (outer);
            var nestedLayout = outerLayout.FieldLayout ("nested");
            var nestedField = outerLayout.Field ("nested");

            var stack = new Value[10];
            int basePtr = 0;

            // Write outer.x
            outerLayout.Field ("x").Set (stack, basePtr, 10);
            // Write outer.nested.a and outer.nested.b using nested address
            int nestedPtr = nestedField.GetAddress (basePtr);
            nestedLayout.Field ("a").Set (stack, nestedPtr, 20);
            nestedLayout.Field ("b").Set (stack, nestedPtr, 30);
            // Write outer.y
            outerLayout.Field ("y").Set (stack, basePtr, 40);

            Assert.AreEqual (10, stack[0].Int32Value); // x
            Assert.AreEqual (20, stack[1].Int32Value); // nested.a
            Assert.AreEqual (30, stack[2].Int32Value); // nested.b
            Assert.AreEqual (40, stack[3].Int32Value); // y

            // Read back via accessor
            Assert.AreEqual (20, nestedLayout.Field ("a").Get (stack, nestedPtr).Int32Value);
            Assert.AreEqual (30, nestedLayout.Field ("b").Get (stack, nestedPtr).Int32Value);
        }

        [TestMethod]
        public void FieldLayoutThrowsForNonStructField ()
        {
            var s = new CStructType ("S");
            s.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });

            var layout = new StructLayout (s);
            Assert.ThrowsException<InvalidOperationException> (() => layout.FieldLayout ("x"));
        }

        [TestMethod]
        public void FieldThrowsForUnknownField ()
        {
            var s = new CStructType ("S");
            s.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });

            var layout = new StructLayout (s);
            Assert.ThrowsException<KeyNotFoundException> (() => layout.Field ("nonexistent"));
        }

        [TestMethod]
        public void FieldLayoutThrowsForUnknownField ()
        {
            var s = new CStructType ("S");
            s.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });

            var layout = new StructLayout (s);
            Assert.ThrowsException<KeyNotFoundException> (() => layout.FieldLayout ("nonexistent"));
        }

        [TestMethod]
        public void PolymorphicStructFieldOffsetSkipsVptr ()
        {
            var s = new CStructType ("Base");
            s.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });
            var method = new CStructMethod {
                Name = "foo",
                MemberType = new CFunctionType (CBasicType.SignedInt, isInstance: true, declaringType: s),
                IsVirtual = true
            };
            s.Members.Add (method);
            s.BuildVTable ();

            var layout = new StructLayout (s);
            // vptr at 0, x at 1
            Assert.AreEqual (1, layout.Field ("x").Offset);
        }

        [TestMethod]
        public void DerivedStructFieldOffsetIncludesBase ()
        {
            var baseType = new CStructType ("Base");
            baseType.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });
            var method = new CStructMethod {
                Name = "foo",
                MemberType = new CFunctionType (CBasicType.SignedInt, isInstance: true, declaringType: baseType),
                IsVirtual = true
            };
            baseType.Members.Add (method);
            baseType.BuildVTable ();

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            derived.Members.Add (new CStructField { Name = "y", MemberType = CBasicType.SignedInt });
            derived.BuildVTable ();

            var layout = new StructLayout (derived);
            // vptr(0), base x(1), own y(2)
            Assert.AreEqual (1, layout.Field ("x").Offset);
            Assert.AreEqual (2, layout.Field ("y").Offset);
        }

        [TestMethod]
        public void NonPolymorphicDerivedFieldOffset ()
        {
            var baseType = new CStructType ("Base");
            baseType.Members.Add (new CStructField { Name = "x", MemberType = CBasicType.SignedInt });

            var derived = new CStructType ("Derived");
            derived.BaseType = baseType;
            derived.Members.Add (new CStructField { Name = "y", MemberType = CBasicType.SignedInt });

            var layout = new StructLayout (derived);
            // No vptr: base x(0), own y(1)
            Assert.AreEqual (0, layout.Field ("x").Offset);
            Assert.AreEqual (1, layout.Field ("y").Offset);
        }

        [TestMethod]
        public void StructLayoutMatchesCompiledOffsets ()
        {
            // Verify that StructLayout produces the same offsets as the compiler
            var exe = Compile (@"
struct Sensor {
    int id;
    float temperature;
    int status;
};
Sensor s;
void main() {
    s.id = 1;
    s.temperature = 36.5f;
    s.status = 2;
    assertAreEqual(1, s.id);
    assertFloatsAreEqual(36.5f, s.temperature);
    assertAreEqual(2, s.status);
}
");
            var sVar = exe.Globals.FirstOrDefault (g => g.Name == "s");
            Assert.IsNotNull (sVar);
            var sType = sVar!.VariableType as CStructType;
            Assert.IsNotNull (sType);

            var layout = new StructLayout (sType!);
            Assert.AreEqual (0, layout.Field ("id").Offset);
            Assert.AreEqual (1, layout.Field ("temperature").Offset);
            Assert.AreEqual (2, layout.Field ("status").Offset);
        }

        [TestMethod]
        public void StructLayoutUsedInInternalFunction ()
        {
            // Simulate the pattern: use StructLayout in an internal function
            var servo = new CStructType ("Servo");
            servo.Members.Add (new CStructField { Name = "pin", MemberType = CBasicType.SignedInt });
            servo.Members.Add (new CStructField { Name = "servoIndex", MemberType = CBasicType.UnsignedChar });
            servo.Members.Add (new CStructField { Name = "min", MemberType = CBasicType.SignedChar });
            servo.Members.Add (new CStructField { Name = "max", MemberType = CBasicType.SignedChar });

            var layout = new StructLayout (servo);
            var pinField = layout.Field ("pin");
            var servoIndexField = layout.Field ("servoIndex");
            var minField = layout.Field ("min");
            var maxField = layout.Field ("max");

            // Simulate stack with struct at offset 5
            var stack = new Value[20];
            int thisPtr = 5;

            // Write fields using accessors instead of magic numbers
            pinField.Set (stack, thisPtr, 13);
            servoIndexField.Set (stack, thisPtr, (byte)1);
            minField.Set (stack, thisPtr, (sbyte)10);
            maxField.Set (stack, thisPtr, unchecked((sbyte)180));

            // Verify we can read them back
            Assert.AreEqual (13, pinField.Get (stack, thisPtr).Int32Value);
            Assert.AreEqual (1, servoIndexField.Get (stack, thisPtr).UInt8Value);
            Assert.AreEqual (10, minField.Get (stack, thisPtr).Int8Value);
            Assert.AreEqual (unchecked((sbyte)180), maxField.Get (stack, thisPtr).Int8Value);

            // Verify they're at the expected stack positions
            Assert.AreEqual (13, stack[5].Int32Value);
            Assert.AreEqual (1, stack[6].UInt8Value);
            Assert.AreEqual (10, stack[7].Int8Value);
        }

        [TestMethod]
        public void ConstructorThrowsOnNull ()
        {
            Assert.ThrowsException<ArgumentNullException> (() => new StructLayout (null!));
        }
    }
}
