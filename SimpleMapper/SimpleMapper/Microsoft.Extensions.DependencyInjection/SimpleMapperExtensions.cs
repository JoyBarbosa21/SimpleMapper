using SimpleMapper;
using SimpleMapper.Configuration;

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

}
