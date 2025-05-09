using System;

namespace SimpleMapper.Map;

public abstract class MapDefinition
{
    public abstract object Map(object source);
}

public class MapDefinition<TSource, TDestination> : MapDefinition
{
    private readonly Dictionary<string, Func<TSource, object>> _memberMappings = new();

    public MapDefinition<TSource, TDestination> ForMember(
        string destProp,
        Func<TSource, object> valueFunc)
    {
        _memberMappings[destProp] = valueFunc;
        return this;
    }

    public MapDefinition<TSource, TDestination> ReverseMap()
    {
        return this;
    }

    public override object Map(object source)
    {
        if (source is not TSource typedSrc)
            throw new InvalidCastException();

        var dest = Activator.CreateInstance<TDestination>();

        foreach (var prop in typeof(TDestination).GetProperties())
        {
            if (_memberMappings.TryGetValue(prop.Name, out var func))
            {
                prop.SetValue(dest, func(typedSrc));
            }
            else
            {
                var srcProp = typeof(TSource).GetProperty(prop.Name);
                if (srcProp != null)
                {
                    prop.SetValue(dest, srcProp.GetValue(typedSrc));
                }
            }
        }

        return dest!;
    }
}

