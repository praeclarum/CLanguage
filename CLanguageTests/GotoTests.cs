using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class GotoTests : TestsBase
    {
        [TestMethod]
        public void ForwardGoto ()
        {
            Run (@"
void main()
{
    int x = 0;
    goto skip;
    x = 42;
skip:
    assertAreEqual(0, x);
}
");
        }

        [TestMethod]
        public void BackwardGoto ()
        {
            Run (@"
void main()
{
    int x = 0;
loop:
    x = x + 1;
    if (x < 5)
        goto loop;
    assertAreEqual(5, x);
}
");
        }

        [TestMethod]
        public void GotoSkipsMultipleStatements ()
        {
            Run (@"
void main()
{
    int a = 1;
    goto end;
    a = 2;
    a = 3;
    a = 4;
end:
    assertAreEqual(1, a);
}
");
        }

        [TestMethod]
        public void MultipleLabels ()
        {
            Run (@"
void main()
{
    int x = 0;
    goto second;
first:
    x = x + 10;
    goto done;
second:
    x = x + 1;
    goto first;
done:
    assertAreEqual(11, x);
}
");
        }

        [TestMethod]
        public void GotoWithinNestedBlocks ()
        {
            Run (@"
void main()
{
    int x = 0;
    if (1) {
        goto skip;
    }
    x = 42;
skip:
    assertAreEqual(0, x);
}
");
        }

        [TestMethod]
        public void LabelBeforeReturn ()
        {
            Run (@"
void main()
{
    int x = 1;
    goto end;
    x = 2;
end:
    return;
}
");
        }

        [TestMethod]
        public void GotoUndefinedLabel ()
        {
            Run (@"
void main()
{
    goto missing;
}
", 9999);
        }

        [TestMethod]
        public void DuplicateLabel ()
        {
            Run (@"
void main()
{
    int x = 0;
dup:
    x = 1;
dup:
    x = 2;
}
", 140);
        }

        [TestMethod]
        public void GotoInLoopBreakout ()
        {
            Run (@"
void main()
{
    int sum = 0;
    int i = 0;
    while (i < 100) {
        sum = sum + i;
        i = i + 1;
        if (i == 5)
            goto done;
    }
done:
    assertAreEqual(10, sum);
}
");
        }
    }
}
