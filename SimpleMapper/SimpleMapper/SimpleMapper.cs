using System.Collections;
using SimpleMapper.Interface;
using SimpleMapper.Util;

namespace SimpleMapper;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
    List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> sources);
}

public class SimpleMapper : IMapper
{
    private readonly Dictionary<(Type, Type), IMapAction> _mappings;

    public SimpleMapper(Dictionary<(Type, Type), IMapAction> mappings)
    {
        _mappings = mappings;
    }

    public TDestination Map<TDestination>(object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        // Se for uma lista, trata como lista
        if (source is IEnumerable<object> sourceList)
        {
            var sourceType = source.GetType().GenericTypeArguments[0];
            var destType = typeof(TDestination).GenericTypeArguments[0];
            var key = (sourceType, destType);

            if (_mappings.TryGetValue(key, out var listAction))
            {
                var destinationList = (IList)Activator
                    .CreateInstance(typeof(List<>).MakeGenericType(destType))!;
                
                foreach (var item in sourceList)
                {
                    if (item is null) continue;
                    var mappedItem = listAction.Map(item);
                    destinationList.Add(mappedItem);
                }

                return (TDestination)(object)destinationList;
            }
        }

        // Tenta encontrar o mapeamento direto
        var directKey = (source.GetType(), typeof(TDestination));
        if (_mappings.TryGetValue(directKey, out var directAction))
        {
            return (TDestination)directAction.Map(source);
        }

        // Tenta encontrar o mapeamento reverso
        var reverseKey = (typeof(TDestination), source.GetType());
        if (_mappings.TryGetValue(reverseKey, out var reverseAction))
        {
            return (TDestination)reverseAction.Map(source);
        }

        throw new InvalidOperationException(
            $"Mapeamento não encontrado para {source.GetType().Name} → {typeof(TDestination).Name}");
    }

    public List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> sources)
    {
        ArgumentNullException.ThrowIfNull(sources, nameof(sources));

        var key = TypeResolver.CreateMapKey<TSource, TDestination>();

        if (!_mappings.TryGetValue(key, out var action))
        {
            throw new InvalidOperationException(
                $"Mapeamento não encontrado para {typeof(TSource).Name} → {typeof(TDestination).Name}");
        }

        var destinationList = new List<TDestination>();
        foreach (var source in sources.Where(s => s != null))
        {
            if (source is null) continue;
            
            var mappedItem = (TDestination)action.Map(source);
            destinationList.Add(mappedItem);
        }

        return destinationList;
    }
}