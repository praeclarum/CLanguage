using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NUnit.Framework
{
	public class Assert
	{
		public static void That (bool condition)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.IsTrue (condition);
		}

		public static void That (object actual, IResolveConstraint constraint)
		{
			constraint.Run (actual, "");
		}

		public static void That (object actual, IResolveConstraint constraint, string message)
		{
			constraint.Run (actual, message);
		}

		public static void AreEqual (int expected, int actual)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.AreEqual (expected, actual);
		}

		public static void AreEqual (string expected, string actual)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.AreEqual (expected, actual);
		}

		public static void AreEqual (object expected, object actual)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.AreEqual (expected, actual);
		}

		public static void Fail (string message)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.Fail (message);
		}

		public static void IsInstanceOf<T> (object value)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.IsInstanceOfType (value, typeof (T));
		}

		public static void IsTrue (bool value)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.IsTrue (value);
		}

		public static void IsNull (object value)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.IsNull (value);
		}
	}

	public static class Is
	{
		public static IResolveConstraint EqualTo (object expected)
		{
			return new EqualConstraint (expected);
		}
		public static IResolveConstraint Null ()
		{
			return new NullConstraint ();
		}
		public static class Not
		{
			public static IResolveConstraint EqualTo (object expected)
			{
				return new NotConstraint (new EqualConstraint (expected));
			}
			public static IResolveConstraint Null ()
			{
				return new NotConstraint (new NullConstraint ());
			}
		}

		public static IResolveConstraint InstanceOf<T> ()
		{
			return new InstanceOfConstraint<T> ();
		}

		public static IResolveConstraint True { get { return new EqualConstraint (true); } }
	}

	public interface IResolveConstraint
	{
		void Run (object actual, string message);
	}

	public abstract class Constraint : IResolveConstraint
	{
		public abstract void Run (object actual, string message);
	}

	public class InstanceOfConstraint<T> : Constraint
	{
		public override void Run (object actual, string message)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.IsInstanceOfType (actual, typeof(T), message);
		}
	}

	public class EqualConstraint : Constraint
	{
		object expected;
		public EqualConstraint (object expected)
		{
			this.expected = expected;
		}
		public override void Run (object actual, string message)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.AreEqual (expected, actual, message);
		}
	}

	public class NullConstraint : Constraint
	{
		public override void Run (object actual, string message)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.IsNull (actual, message);
		}
	}

	public class NotConstraint : Constraint
	{
		Constraint constraint;
		public NotConstraint (Constraint constraint)
		{
			this.constraint = constraint;
		}
		public override void Run (object actual, string message)
		{
			Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.Fail (message);
		}
	}
}

