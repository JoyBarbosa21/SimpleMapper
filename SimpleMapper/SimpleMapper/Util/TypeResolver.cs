using System;

namespace SimpleMapper.Util;
public static class TypeResolver<T>
{
    public static Type ItemType => typeof(T);
}

public static class TypeResolver
{
    private static readonly Dictionary<object, Type> TypeCache = new();

    public static Type ResolveType<T>(T item) => TypeResolver<T>.ItemType;

    public static (Type SourceType, Type DestType) CreateMapKey<TSource, TDestination>() 
        => (TypeResolver<TSource>.ItemType, TypeResolver<TDestination>.ItemType);
        
    public static Type GetListItemType<T>() where T : IEnumerable<object>
        => TypeResolver<T>.ItemType.GenericTypeArguments[0];
}
