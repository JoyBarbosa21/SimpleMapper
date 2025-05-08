using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SimpleMapper.Util;

public class TypeCache
{
    private static readonly ConcurrentDictionary<Type, string> TypeNames = new();
    private static readonly ConcurrentDictionary<Type, Func<IList>> ListFactories = new();
    private readonly ConcurrentDictionary<Type, Type> _elementTypes = new();

    public Type GetElementType<T>() where T : IEnumerable
    {
        var type = typeof(T);
        return _elementTypes.GetOrAdd(type, _ =>
        {
            var elementType = type.IsArray ? type.GetElementType()! 
                : type.GenericTypeArguments[0];
            return elementType;
        });
    }

    public string GetTypeName<T>() => typeof(T).Name;

    public IList CreateList<T>()
    {
        var type = typeof(T);
        var factory = ListFactories.GetOrAdd(type, t =>
        {
            var listType = typeof(List<>).MakeGenericType(t);
            var newExpr = Expression.New(listType);
            return Expression.Lambda<Func<IList>>(newExpr).Compile();
        });

        return factory();
    }

    public static string GetName<T>() => typeof(T).Name;
}

public static class TypeCache<T>
{
    public static readonly Type Type = typeof(T);
    public static readonly string Name = typeof(T).Name;
    public static readonly bool IsList = typeof(T).IsGenericType && 
                                       typeof(T).GetGenericTypeDefinition() == typeof(List<>);
    public static readonly Type ElementType = IsList ? typeof(T).GetGenericArguments()[0] : typeof(T);

    private static readonly Func<List<T>> ListFactory;

    static TypeCache()
    {
        var ctor = Expression.New(typeof(List<T>));
        var lambda = Expression.Lambda<Func<List<T>>>(ctor);
        ListFactory = lambda.Compile();
    }

    public static List<T> CreateList() => ListFactory();
}