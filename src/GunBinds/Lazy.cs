using System;

namespace GunBinds;

public sealed class Lazy<T>
{
    private readonly Func<T> _factory;

    private bool _isInitialized;

    private T _value;

    public Lazy(Func<T> factory)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        _factory = factory;
    }

    public T Value
    {
        get
        {
            if (_isInitialized)
            {
                return _value;
            }

            _value = _factory();
            _isInitialized = true;
            return _value;
        }
    }
}
