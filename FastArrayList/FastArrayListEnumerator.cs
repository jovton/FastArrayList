using System;
using System.Collections;
using System.Collections.Generic;

namespace Jovton;

internal class FastArrayListEnumerator<T> : IEnumerator<T>
{
    private int _index = -1;
    private FastArrayListNode<Memory<T>>? _itemsRoot;
    private FastArrayListNode<Memory<T>>? _items;

    public FastArrayListEnumerator(FastArrayListNode<Memory<T>> items)
    {
        _itemsRoot = _items;
        _items = items;
    }

    public T Current => _items is null ? throw new NullReferenceException() : _items.Value.Span[_index];

    object? IEnumerator.Current => _index >= 0 ? Current : null;

    public bool MoveNext()
    {
        if (_items is null)
        {
            return false;
        }

        if (!(_items.Value is Memory<T> memory))
        {
            return false;
        }

        if (_items.Value.Span.Length > (_index + 1))
        {
            _index++;
            return true;
        }
        else if (_items.Next is FastArrayListNode<Memory<T>> node)
        {
            _items = _items.Next;
            _index = -1;
            return MoveNext();
        }

        return false;
    }

    public void Reset()
    {
        _items = _itemsRoot;

        if (_items is null)
        {
            _index = -1;
            return;
        }

        if (!(_items.Value is Memory<T> memory))
        {
            _index = -1;
            return;
        }

        if (_items.Value.Span.Length == 0)
        {
            _index = -1;
            return;
        }

        _index = 0;
    }

    public void Dispose()
    {
        _items = null;
        _itemsRoot = null;
    }
}