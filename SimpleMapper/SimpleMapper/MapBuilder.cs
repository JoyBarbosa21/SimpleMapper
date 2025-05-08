using System.Linq.Expressions;
using SimpleMapper.Interface;
using SimpleMapper.Util;

namespace SimpleMapper;

public class MapBuilder<TSource, TDestination> : IMapAction
{
    private readonly List<Action<TSource, TDestination>> _mappings = new();
    private readonly List<Action<TDestination, TSource>> _reverseMappings = new();
    private readonly Dictionary<string, string> _propertyMaps = new();
    private bool _hasReverse;

    public MapBuilder()
    {
        // Configura mapeamentos automáticos no construtor usando cache de propriedades
        ConfigureAutomaticMappings();
    }

    private void ConfigureAutomaticMappings()
    {
        var properties = PropertyCache<TSource, TDestination>.GetCommonProperties();
        
        foreach (var prop in properties)
        {
            var getter = PropertyCache<TSource>.GetGetter(prop);
            var setter = PropertyCache<TDestination>.GetSetter(prop);

            if (getter != null && setter != null)
            {
                _mappings.Add((source, dest) =>
                {
                    var value = getter(source);
                    setter(dest, value);
                });
            }
        }
    }
    
    public MapBuilder<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Expression<Func<TSource, TMember>> sourceMember)
    {
        if (destinationMember.Body is MemberExpression destMember && 
            sourceMember.Body is MemberExpression srcMember)
        {
            var destPropName = destMember.Member.Name;
            var sourcePropName = srcMember.Member.Name;

            var getter = PropertyCache<TSource>.GetGetter(sourcePropName);
            var setter = PropertyCache<TDestination>.GetSetter(destPropName);

            if (getter != null && setter != null)
            {
                _mappings.Add((source, dest) =>
                {
                    var value = getter(source);
                    setter(dest, value);
                });

                _propertyMaps[sourcePropName] = destPropName;
            }
        }

        return this;
    }

    public MapBuilder<TSource, TDestination> ReverseMap()
    {
        _hasReverse = true;

        foreach (var mapping in _propertyMaps)
        {
            var destGetter = PropertyAccessor<TDestination>.GetGetter(mapping.Value);
            var sourceSetter = PropertyAccessor<TSource>.GetSetter(mapping.Key);

            if (destGetter != null && sourceSetter != null)
            {
                _reverseMappings.Add((source, dest) =>
                {
                    var value = destGetter(source);
                    sourceSetter(dest, value);
                });
            }
        }

        // Adiciona mapeamentos automáticos para propriedades com mesmo nome
        var sourceProps = PropertyAccessor<TSource>.GetPropertyNames();
        var destProps = PropertyAccessor<TDestination>.GetPropertyNames();

        foreach (var propName in sourceProps.Intersect(destProps))
        {
            if (!_propertyMaps.ContainsKey(propName))
            {
                var getter = PropertyAccessor<TSource>.GetGetter(propName);
                var setter = PropertyAccessor<TDestination>.GetSetter(propName);

                if (getter != null && setter != null)
                {
                    _mappings.Add((source, dest) =>
                    {
                        var value = getter(source);
                        setter(dest, value);
                    });

                    var reverseGetter = PropertyAccessor<TDestination>.GetGetter(propName);
                    var reverseSetter = PropertyAccessor<TSource>.GetSetter(propName);

                    if (reverseGetter != null && reverseSetter != null)
                    {
                        _reverseMappings.Add((source, dest) =>
                        {
                            var value = reverseGetter(source);
                            reverseSetter(dest, value);
                        });
                    }
                }
            }
        }

        return this;
    }

    public object Map(object source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source is TDestination destSource)
        {
            return MapReverse(destSource);
        }
        
        if (source is TSource typedSource)
        {
            var destination = TypeFactory<TDestination>.Create();
            
            foreach (var mapping in _mappings)
            {
                mapping(typedSource, destination);
            }

            return destination!;
        }

        throw new InvalidOperationException(
            $"O objeto fonte deve ser do tipo {TypeCache.GetName<TSource>()} ou {TypeCache.GetName<TDestination>()}");
    }

    public object MapReverse(object source)
    {
        if (!_hasReverse)
            throw new InvalidOperationException("ReverseMap não foi configurado.");

        if (source is not TDestination typedSource)
            throw new InvalidOperationException(
                $"O objeto fonte deve ser do tipo {TypeCache.GetName<TDestination>()}");

        var destination = TypeFactory<TSource>.Create();

        foreach (var mapping in _reverseMappings)
        {
            mapping(typedSource, destination);
        }

        return destination!;
    } 
}
