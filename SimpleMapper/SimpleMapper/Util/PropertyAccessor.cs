using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SimpleMapper.Util;

public static class PropertyAccessor<T>
{
    private static readonly ConcurrentDictionary<string, Func<T, object?>> Getters = new();
    private static readonly ConcurrentDictionary<string, Action<T, object?>> Setters = new();
    private static readonly HashSet<string> PropertyNames = new();

    public static Func<T, object?>? GetGetter(string propertyName)
    {
        try
        {
            return Getters.GetOrAdd(propertyName, name =>
            {
                var param = Expression.Parameter(typeof(T), "x");
                var property = Expression.PropertyOrField(param, name);
                var convert = Expression.Convert(property, typeof(object));
                return Expression.Lambda<Func<T, object?>>(convert, param).Compile();
            });
        }
        catch
        {
            return null;
        }
    }

    public static Action<T, object?>? GetSetter(string propertyName)
    {
        try
        {
            return Setters.GetOrAdd(propertyName, name =>
            {
                var param = Expression.Parameter(typeof(T), "x");
                var value = Expression.Parameter(typeof(object), "value");
                var property = Expression.PropertyOrField(param, name);
                var convert = Expression.Convert(value, property.Type);
                var assign = Expression.Assign(property, convert);
                return Expression.Lambda<Action<T, object?>>(assign, param, value).Compile();
            });
        }
        catch
        {
            return null;
        }
    }

    public static IEnumerable<string> GetPropertyNames()
    {
        if (!PropertyNames.Any())
        {
            var param = Expression.Parameter(typeof(T), "x");
            foreach (var prop in typeof(T).GetProperties())
            {
                try
                {
                    Expression.PropertyOrField(param, prop.Name);
                    PropertyNames.Add(prop.Name);
                }
                catch
                {
                    // Ignora propriedades que não podem ser acessadas
                }
            }
        }
        return PropertyNames;
    }
}
