using System.Collections;
using SimpleMapper.Configuration;
using SimpleMapper.Interface;

namespace SimpleMapper;

public class SimpleMapper : ISimpleMapper
{
    private readonly SimpleMapperConfiguration _config;

    public SimpleMapper(SimpleMapperConfiguration config)
    {
        _config = config;
    }

    public TDestination Map<TDestination>(object source)
        where TDestination : new()
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var sourceType = source.GetType();

        if (typeof(IEnumerable).IsAssignableFrom(sourceType) 
        && typeof(IEnumerable).IsAssignableFrom(typeof(TDestination)))
        {
            return (TDestination)MapList((IEnumerable)source, 
                typeof(TDestination).GetGenericArguments()[0]);
        }

        if (_config.TryGetMapping(sourceType, typeof(TDestination), out var mapping) && mapping != null)
    {
            var destination = new TDestination();
            mapping(source, destination);
            return destination;
        }

        throw new InvalidOperationException($"No mapping exists for {sourceType} -> {typeof(TDestination)}");
    }

    private object MapList(IEnumerable source, Type destinationItemType)
    {
        var listType = typeof(List<>).MakeGenericType(destinationItemType);
        var destinationList = (IList)Activator.CreateInstance(listType)!;

        foreach (var item in source)
        {
            var destinationItem = Activator.CreateInstance(destinationItemType)!;

            if (_config.TryGetMapping(item.GetType(), 
            destinationItemType, out var mapping) && mapping != null)
            {
                mapping(item, destinationItem);
                destinationList.Add(destinationItem);
            }
            else
            {
                throw new InvalidOperationException($"No mapping exists for {item.GetType()} -> {destinationItemType}");
            }
        }

        return destinationList;
    }
}