using System.Collections;
using System.Text;

namespace Unicorn.Algo.Indicators.Utility;

public sealed class RollingWindow<T> : IList<T>
{
    private readonly T[] _window;
    private int _start;

    private int _version;

    public RollingWindow(int windowSize)
    {
        if (windowSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(windowSize), "windowSize must be at least 1");
        }

        WindowSize = windowSize;
        _window = new T[windowSize];
    }

    public int Count { get; private set; }

    public int WindowSize { get; }

    public bool IsReadOnly => false;

    #region Custom

    public bool IsReady => Count == WindowSize;

    public List<T> Source => _window.ToList();

    #endregion

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    "index must be greater than zero and less than the size of the collection");
            }

            return _window[WrapIndex(_start + index)];
        }
        set
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    "index must be greater than zero and less than the size of the collection");
            }

            _window[WrapIndex(_start + index)] = value;
        }
    }

    public void Add(T item)
    {
        if (Count < WindowSize)
        {
            Count++;
        }
        else
        {
            _start = WrapIndex(_start + 1);
        }

        this[Count - 1] = item;

        _version++;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index),
                "index must be greater than zero and less than or equal to the size of the collection");
        }

        if (Count >= WindowSize)
        {
            throw new InvalidOperationException("Unable to insert item; rolling window is full");
        }

        // Increase count first to
        // prevent out of range indexes
        Count++;

        if (index <= WindowSize / 2)
        {
            // Shift left to make room
            _start = WrapIndex(_start - 1);

            for (var i = 0; i < index; i++)
            {
                this[i] = this[i + 1];
            }
        }
        else
        {
            // Shift right to make room
            for (var i = Count - 1; i > index; i--)
            {
                this[i] = this[i - 1];
            }
        }

        // Insert the item and
        // increment the version
        this[index] = item;
        _version++;
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);

        if (index < 0)
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index)
    {
        if (index <= WindowSize / 2)
        {
            // Shift right to fill space
            for (var i = index; i > 0; i--)
            {
                this[i] = this[i - 1];
            }

            this[0] = default!;
            _start++;
        }
        else
        {
            // Shift left to fill space
            for (var i = index; i < Count - 1; i++)
            {
                this[i] = this[i + 1];
            }

            this[Count - 1] = default!;
        }

        Count--;
        _version++;
    }

    public int IndexOf(T item)
    {
        var i = 0;

        foreach (var obj in this)
        {
            if (Equals(obj, item))
            {
                return i;
            }

            i++;
        }

        return -1;
    }

    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    public void Clear()
    {
        for (var i = 0; i < WindowSize; i++)
        {
            _window[i] = default!;
        }

        Count = 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        int i;

        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex must be greater than zero");
        }

        if (Count > array.Length - arrayIndex)
        {
            throw new ArgumentException("Not enough available space in array to copy elements from rolling window");
        }

        i = 0;

        foreach (var item in this)
        {
            array[i] = item;
            i++;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        var version = _version;
        var count = 0;

        for (var i = _start; i < WindowSize; i++)
        {
            if (version != _version)
            {
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute");
            }

            if (count >= Count)
            {
                break;
            }

            count++;

            yield return _window[i];
        }

        for (var i = 0; i < _start; i++)
        {
            if (version != _version)
            {
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute");
            }

            if (count >= Count)
            {
                break;
            }

            count++;

            yield return _window[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private int WrapIndex(int index)
    {
        return (int)Euclidean.Mod(index, WindowSize);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var value in _window)
        {
            sb.Append($"{value} ");
        }

        return sb.ToString();
    }
}
