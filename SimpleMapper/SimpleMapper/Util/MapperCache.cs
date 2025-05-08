using System;
using System.Linq.Expressions;

namespace SimpleMapper.Util;

public static class MapperCache<TSource, TDestination>
{
    public static readonly Type SourceType = typeof(TSource);
    public static readonly Type DestType = typeof(TDestination);
    public static readonly string SourceName = typeof(TSource).Name;
    public static readonly string DestName = typeof(TDestination).Name;
    
    private static readonly Func<List<TDestination>> ListFactory;

    static MapperCache()
    {
        var newExpr = Expression.New(typeof(List<TDestination>));
        var lambda = Expression.Lambda<Func<List<TDestination>>>(newExpr);
        ListFactory = lambda.Compile();
    }

    public static List<TDestination> CreateList() => ListFactory();
}
