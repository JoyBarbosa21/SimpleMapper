using System;
using System.Linq.Expressions;

namespace SimpleMapper.Util;

public static class TypeFactory<T>
{
    private static readonly Func<T> Constructor;

    static TypeFactory()
    {
        var ctor = Expression.New(typeof(T));
        var lambda = Expression.Lambda<Func<T>>(ctor);
        Constructor = lambda.Compile();
    }

    public static T Create() => Constructor();
}