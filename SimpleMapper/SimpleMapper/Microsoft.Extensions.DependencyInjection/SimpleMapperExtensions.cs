using SimpleMapper;
using SimpleMapper.Configuration;
using SimpleMapper.Interface;

namespace Microsoft.Extensions.DependencyInjection;
public static class SimpleMapperExtensions
{
     public static IServiceCollection AddSimpleMapper(this IServiceCollection services, Action<SimpleMapperConfiguration> setup)
    {
        var config = new SimpleMapperConfiguration();
        setup(config);

        var mapper = new SimpleMapper.SimpleMapper(config.Mappings);
        services.AddSingleton<IMapper>(mapper);

        return services;
    }

    public static void AddProfile<TProfile>(this SimpleMapperConfiguration configuration)
        where TProfile : ISimpleMapperProfile, new()
    {
        var profile = new TProfile();
     
        profile.Configure(configuration);
    }
}
