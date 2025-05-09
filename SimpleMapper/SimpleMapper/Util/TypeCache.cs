using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SimpleMapper.Util;

public class TypeCache
{
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

    public static string GetName<T>() => typeof(T).Name;
}

public static class TypeCache<T>
{
    public static readonly bool IsList = typeof(T).IsGenericType && 
                                       typeof(T).GetGenericTypeDefinition() == typeof(List<>);

    private static readonly Func<List<T>> ListFactory;

    static TypeCache()
    {
        var ctor = Expression.New(typeof(List<T>));
        var lambda = Expression.Lambda<Func<List<T>>>(ctor);
        ListFactory = lambda.Compile();
    }

    public static List<T> CreateList() => ListFactory();
}