using System;
using System.Collections;
using System.Collections.Generic;

namespace Jovton;

public sealed class FastArrayList<T> : IEnumerable<T>, IList<T>, IList, IReadOnlyList<T>
{
    private readonly FastArrayListNode<Memory<T>> _head = new();
    private FastArrayListNode<Memory<T>>? _tail;
    private int _size = 0;

    public FastArrayList(T[] values)
    {
        _head.Value = values;
        _tail = _head;
        _size = values.Length;
    }

    public T this[int index]
    {
        get
        {
            if (index > _size - 1)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == _size)
            {

            }
            else
            {
                var currentNode = _head;

                if (index < currentNode.Value.Length)
                {
                    return currentNode.Value.Span[index];
                }

                var i = currentNode.Value.Length;
                currentNode = currentNode.Next;

                while (currentNode is not null && i <= index)
                {
                    if (i == index && currentNode is not null)
                    {
                        return currentNode.Value.Span[0];
                    }

                    if (currentNode is not null)
                    {
                        currentNode = currentNode.Next;
                    }
                    i++;
                }
            }

            return default!;
        }

        set
        {
            var currentNode = _head;

            if (index < currentNode.Value.Length)
            {
                currentNode.Value.Span[index] = value;
            }
            else if (index > _size)
            {
                throw new IndexOutOfRangeException();
            }

            var i = currentNode.Value.Length;

            while (i < index)
            {
                currentNode = currentNode!.Next;
                i++;
            }

            if (index < _size)
            {
                currentNode!.Value = new[] { value };
            }
            else
            {
                currentNode!.Next = new FastArrayListNode<Memory<T>>(new[] { value });
                _tail = currentNode;
            }

            _size++;
        }
    }

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (value is null) ? throw new ArgumentNullException(nameof(value)) : (T)value;
    }

    public int Count => _size;

    bool ICollection<T>.IsReadOnly => false;

    bool IList.IsReadOnly => false;

    bool IList.IsFixedSize => false;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    public IEnumerator<T> GetEnumerator()
    {
        return new FastArrayListEnumerator<T>(_head);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new FastArrayListEnumerator<T>(_head);
    }

    public void Add(T item)
    {
        Insert(_size, item);
    }

    public int Add(object? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        Insert(_size, (T)value);
        return _size - 1;
    }

    public void Clear()
    {
        var currentNode = _head;
        _tail = _head;

        while (currentNode is not null)
        {
            var next = currentNode.Next;
            currentNode.Value.Span.Clear();
            currentNode.Next = null;
            currentNode = next;
        }

        _size = 0;
    }

    public bool Contains(T item)
    {
        return IndexOf(item) > -1;
    }

    public bool Contains(object? value)
    {
        if (value is null)
        {
            return false;
        }

        return IndexOf((T)value) > -1;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (arrayIndex > _size - 1)
        {
            throw new IndexOutOfRangeException();
        }

        var counter = 0;
        var currentNode = _head;

        while (currentNode is not null)
        {
            if (arrayIndex < counter + currentNode.Value.Length)
            {
                for (var i = 0; i < currentNode.Value.Length; i++)
                {
                    array[counter + i] = currentNode.Value.Span[i];
                }
            }

            counter += currentNode.Value.Length;
            currentNode = currentNode.Next;
        }
    }

    public void CopyTo(Array array, int index)
    {
        if (index > _size - 1)
        {
            throw new IndexOutOfRangeException();
        }

        var counter = 0;
        var currentNode = _head;

        while (currentNode is not null)
        {
            if (index < counter + currentNode.Value.Length)
            {
                for (var i = 0; i < currentNode.Value.Length; i++)
                {
                    array.SetValue(currentNode.Value.Span[i], counter + i);
                }
            }

            counter += currentNode.Value.Length;
            currentNode = currentNode.Next;
        }
    }

    public int IndexOf(T item)
    {
        var index = 0;
        var currentNode = _head;

        while (currentNode is not null)
        {
            for (int i = 0; i < currentNode!.Value.Length; i += 5)
            {
                if (currentNode!.Value.Span[i]?.Equals(item) ?? false)
                {
                    index += i;
                    return index;
                }
                else if (i + 1 < currentNode.Value.Length && (currentNode!.Value.Span[i + 1]?.Equals(item) ?? false))
                {
                    index += i + 1;
                    return index;
                }
                else if (i + 2 < currentNode.Value.Length && (currentNode!.Value.Span[i + 2]?.Equals(item) ?? false))
                {
                    index += i + 2;
                    return index;
                }
                else if (i + 3 < currentNode.Value.Length && (currentNode!.Value.Span[i + 3]?.Equals(item) ?? false))
                {
                    index += i + 3;
                    return index;
                }
                else if (i + 4 < currentNode.Value.Length && (currentNode!.Value.Span[i + 4]?.Equals(item) ?? false))
                {
                    index += i + 4;
                    return index;
                }
                else
                {
                    index += currentNode!.Value.Length;
                }
            }

            currentNode = currentNode.Next;
        }

        return -1;
    }

    public int IndexOf(object? value)
    {
        if (value is null)
        {
            return -1;
        }

        return IndexOf((T)value);
    }

    public void Insert(int index, T item)
    {
        if (index > _size)
        {
            throw new IndexOutOfRangeException();
        }

        if (index == _size)
        {
            _tail!.Next = new FastArrayListNode<Memory<T>>(new[] { item });
            _tail = _tail.Next;
        }
        else
        {
            var existingNode = NodeAtIndex(index);

            var startPos = index - _size + existingNode.Value.Length;

            if (startPos > 0 && startPos < existingNode.Value.Length)
            {
                var lastNode = existingNode.Next;

                var n1v = existingNode.Value[..index];
                var n2v = existingNode.Value[index..];

                var newNode = new FastArrayListNode<Memory<T>>(new[] { item });
                existingNode.Value = n1v;
                existingNode.Next = newNode;

                var nextNode = new FastArrayListNode<Memory<T>>(n2v); ;
                nextNode.Next = lastNode;

                newNode.Next = nextNode;

                if (lastNode is not null && lastNode.Next is null)
                {
                    _tail = lastNode;
                }
            }
            else if (startPos == 0)
            {
                var newNode = new FastArrayListNode<Memory<T>>(existingNode.Value)
                {
                    Next = existingNode.Next
                };

                if (newNode.Next is null)
                {
                    _tail = newNode;
                }

                existingNode.Value = new Memory<T>(new[] { item });
                existingNode.Next = newNode;
            }
        }

        _size++;
    }

    private FastArrayListNode<Memory<T>> NodeAtIndex(int index)
    {
        var counter = -1;
        var currentNode = _head;

        while (currentNode is not null)
        {
            for (var i = 0; i < currentNode.Value.Length; i++)
            {
                counter += 1;

                if (counter == index)
                {
                    return currentNode;
                }
            }

            currentNode = currentNode.Next;
        }

        return null!;
    }

    public void Insert(int index, object? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        Insert(index, (T)value);
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();

        if (_size > 0)
        {
            _size--;
            return true;
        }

        return false;
    }

    public void Remove(object? value)
    {
        throw new NotImplementedException();

        if (_size > 0)
        {
            _size--;
        }
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
        if (_size > 0)
        {
            _size--;
        }
    }
}
