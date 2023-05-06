namespace Jovton;

public sealed class FastArrayListNode<T>
{
	public T? Value { get; set; }

	public FastArrayListNode<T>? Next { get; set; }

    public FastArrayListNode() { }

    public FastArrayListNode(T value)
    {
        Value = value;
    }

    public FastArrayListNode(T value, FastArrayListNode<T> next)
    {
        Value = value;
        Next = next;
    }
}
