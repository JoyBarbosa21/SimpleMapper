using System.Linq.Expressions;

namespace SimpleMapper.Util;

public static class PropertyCache<T>
{
    private static readonly Dictionary<string, Func<T, object>> Getters;
    private static readonly Dictionary<string, Action<T, object>> Setters;
    public static readonly string Name;

    static PropertyCache()
    {
        Getters = new Dictionary<string, Func<T, object>>();
        Setters = new Dictionary<string, Action<T, object>>();
        Name = typeof(T).Name;
        
        InitializeAccessors();
    }

    private static void InitializeAccessors()
    {
        var param = Expression.Parameter(typeof(T), "x");
        var valueParam = Expression.Parameter(typeof(object), "value");

        foreach (var prop in typeof(T).GetProperties())
        {
            var getter = Expression.Lambda<Func<T, object>>(
                Expression.Convert(Expression.Property(param, prop), typeof(object)),
                param).Compile();

            var setter = Expression.Lambda<Action<T, object>>(
                Expression.Assign(
                    Expression.Property(param, prop),
                    Expression.Convert(valueParam, prop.PropertyType)),
                param,
                valueParam).Compile();

            Getters[prop.Name] = getter;
            Setters[prop.Name] = setter;
        }
    }

    public static IEnumerable<string> GetPropertyNames() => Getters.Keys;

    public static Func<T, object> GetGetter(string propertyName) =>
        Getters.TryGetValue(propertyName, out var getter) 
            ? getter 
            : throw new KeyNotFoundException($"Getter for property '{propertyName}' not found.");

    public static Action<T, object> GetSetter(string propertyName) =>
        Setters.TryGetValue(propertyName, out var setter) 
            ? setter 
            : throw new KeyNotFoundException($"Setter for property '{propertyName}' not found.");
}

public static class PropertyCache<TSource, TDestination>
{
    private static readonly HashSet<string> CommonProperties;

    static PropertyCache()
    {
        CommonProperties = new HashSet<string>(
        PropertyCache<TSource>.GetPropertyNames()
            .Intersect(PropertyCache<TDestination>.GetPropertyNames()));
    }

    public static IEnumerable<string> GetCommonProperties() => CommonProperties;
}
