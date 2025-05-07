using System;

namespace SimpleMapper.Map;

public abstract class MapDefinition
{
    public abstract object Map(object source);
}

public class MapDefinition<TSource, TDestination> : MapDefinition
{
    private readonly Dictionary<string, Func<TSource, object>> _memberMappings = new();
    private bool _hasReverse;

    public MapDefinition<TSource, TDestination> ForMember(
        string destProp,
        Func<TSource, object> valueFunc)
    {
        _memberMappings[destProp] = valueFunc;
        return this;
    }

    public MapDefinition<TSource, TDestination> ReverseMap()
    {
        _hasReverse = true;
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

    public MapDefinition<TDestination, TSource> GetReverseMap()
    {
        if (!_hasReverse)
            throw new InvalidOperationException("Reverse map not enabled.");

        var reverse = new MapDefinition<TDestination, TSource>();
        foreach (var destProp in typeof(TDestination).GetProperties())
        {
            var sourceProp = typeof(TSource).GetProperty(destProp.Name);
            if (sourceProp != null)
            {
                reverse.ForMember(sourceProp.Name, d => destProp.GetValue(d)
                    ?? throw new InvalidOperationException(
                        $"Cannot get value for property {destProp.Name}"));
            }
        }
        return reverse;
    }
}

