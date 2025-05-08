using System.Linq.Expressions;

namespace SimpleMapper.Util;

public static class TypeHelper<T>
{
    public static readonly string Name = typeof(T).Name;
    public static readonly Type Type = typeof(T);
}

public static class ListHelper<T>
{
    private static readonly Func<List<T>> ListFactory;

    static ListHelper()
    {
        var newExpr = Expression.New(typeof(List<T>));
        var lambda = Expression.Lambda<Func<List<T>>>(newExpr);
        ListFactory = lambda.Compile();
    }

    public static List<T> CreateList() => ListFactory();
}