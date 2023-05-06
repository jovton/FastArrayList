namespace Jovton.FastArrayList.Tests;

public class FastArrayListTests
{
    [Fact]
    public void TestBytes()
    {
        var list = new FastArrayList<byte>(new byte[] {1,2,3,4,5});
        Assert.Collection(list, 
            (n) => n.Equals(1),
            (n) => n.Equals(2),
            (n) => n.Equals(3),
            (n) => n.Equals(4),
            (n) => n.Equals(5));
    }

    [Fact]
    public void TestStrings()
    {
        var list = new FastArrayList<string>(new string[] { "a", "c", "b", "rt", "zx" });
        Assert.Collection(list,
            (n) => n.Equals("a"),
            (n) => n.Equals("c"),
            (n) => n.Equals("b"),
            (n) => n.Equals("rt"),
            (n) => n.Equals("zx"));
    }

    [Fact]
    public void TestEnumerator1()
    {
        var arr = new byte[] { 1, 2, 3, 4, 5 };
        var list = new FastArrayList<byte>(arr);

        var i = 0;
        foreach(var item in list)
        {
            Assert.Equal(arr[i], item);
            i++;
        }
    }

    [Fact]
    public void TestEnumerator2()
    {
        var arr = new byte[] { 7, 3, 1, 9, 5 };
        var list = new FastArrayList<byte>(arr)
        {
            20
        };

        var i = 0;
        foreach (var item in list)
        {
            Assert.Equal(list[i], item);
            i++;
        }
    }

    [Fact]
    public void TestCopyToArray1()
    {
        var arr = new byte[] { 7, 3, 1, 9, 5 };
        var list = new FastArrayList<byte>(arr)
        {
            20,
            14
        };

        var copy = new byte[list.Count];
        list.CopyTo(copy, 0);
        
        var i = 0;
        foreach (var item in copy)
        {
            Assert.Equal(list[i], item);
            i++;
        }
    }

    [Fact]
    public void TestAdd1()
    {
        var arr = new byte[] { 1, 2, 3, 4, 5 };
        var list = new FastArrayList<byte>(arr)
        {
            6
        };
        Assert.Equal(6, list[5]);
    }

    [Fact]
    public void TestAdd2()
    {
        var arr = new byte[] { 1, 2, 3, 4, 5 };
        var list = new FastArrayList<byte>(arr)
        {
            6,
            7
        };
        Assert.Equal(7, list[6]);
        Assert.Equal(6, list[5]);
    }

    [Fact]
    public void TestIndexOf1()
    {
        var arr = new int[] { 2, 4, 1, 5, 3 };
        var list = new FastArrayList<int>(arr)
        {
            -4
        };

        Assert.Equal(5, list.IndexOf(-4));
        Assert.Equal(4, list.IndexOf(3));
    }

    [Fact]
    public void TestInsert1()
    {
        var arr = new int[] { 2, 4, 1, 5, 3 };
        var list = new FastArrayList<int>(arr);

        list.Insert(1, -4);

        Assert.Equal(1, list.IndexOf(-4));
        Assert.Equal(5, list.IndexOf(3));
    }

    [Fact]
    public void TestInsert2()
    {
        var arr = new int[] { 2, 4, 1, 5, 3 };
        var list = new FastArrayList<int>(arr);

        list.Insert(0, -4);

        Assert.Equal(0, list.IndexOf(-4));
        Assert.Equal(1, list.IndexOf(2));
        Assert.Equal(5, list.IndexOf(3));
    }

    [Fact]
    public void TestInsert3()
    {
        var arr = new int[] { 2, 4, 1, 5, 3 };
        var list = new FastArrayList<int>(arr);

        list.Insert(5, -7);
        list.Add(38);

        Assert.Equal(5, list.IndexOf(-7));
        Assert.Equal(2, list.IndexOf(1));
        Assert.Equal(0, list.IndexOf(2));
        Assert.Equal(6, list.IndexOf(38));
    }

    [Fact]
    public void TestInsert4()
    {
        var arr = new int[] { 2, 4, 1, 5, 3 };
        var list = new FastArrayList<int>(arr);

        list.Insert(4, -17);

        Assert.Equal(4, list.IndexOf(-17));
        Assert.Equal(5, list.IndexOf(3));
    }

    [Fact]
    public void TestAddRange()
    {
        var arr = new int[] { 2, 4, 1, 5, 3 };
        var list = new FastArrayList<int>(arr);

        list.AddRange(new int[] { 60, 13, 203 });

        Assert.Equal(5, list.IndexOf(60));
        Assert.Equal(7, list.IndexOf(203));
    }

    [Fact]
    public void BigArray()
    {

        var arr = new int[9999999];
        for (var i=0; i<9999999;i++)
        {
            arr[i] = i+1;
        }

        var list = new FastArrayList<int>(arr);

        Assert.Equal(9999999, list.Count);
        Assert.Equal(9999998, list.IndexOf(9999999));
    }
}