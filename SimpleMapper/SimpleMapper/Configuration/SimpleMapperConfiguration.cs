using SimpleMapper.Interface;

namespace SimpleMapper.Configuration;

public class SimpleMapperConfiguration
{
    internal Dictionary<(Type, Type), IMapAction> Mappings { get; } = new();

    public void AddProfile<TProfile>() where TProfile : ISimpleMapperProfile, new()
    {
        var profile = new TProfile();
        profile.Configure(this);
    }

    public MapBuilder<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        var builder = new MapBuilder<TSource, TDestination>();
        Mappings[(typeof(TSource), typeof(TDestination))] = builder;
        return builder;
    }
}
