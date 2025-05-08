using System;

namespace SimpleMapper.Util;

public static class TypeInfo<T>
{
    public static readonly Type Type = typeof(T);
    public static readonly string Name = typeof(T).Name;
}
