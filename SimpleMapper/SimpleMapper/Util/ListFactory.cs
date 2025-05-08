using System.Linq.Expressions;

namespace SimpleMapper.Util;

public static class ListFactory<T>
{
    private static readonly Func<object> CreateDelegate = InitializeDelegate();

    private static Func<object> InitializeDelegate()
    {
        var type = typeof(T);
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
        {
            throw new InvalidOperationException($"Type {type.Name} is not a List<T>");
        }

        var elementType = type.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var newExpr = Expression.New(listType);
        var lambda = Expression.Lambda<Func<object>>(newExpr);
        return lambda.Compile();
    }

    public static object Create() => CreateDelegate();
    public static List<T> CreateList() => new();
}