using SimpleMapper.Configuration;
using SimpleMapper.Interface;

namespace Microsoft.Extensions.DependencyInjection;
public static class SimpleMapperExtensions
{
    public static IServiceCollection AddSimpleMapper(this IServiceCollection services, Action<SimpleMapperConfigurationBuilder> configAction)
    {
        var builder = new SimpleMapperConfigurationBuilder();
        configAction(builder);

        var config = builder.Build();
        services.AddSingleton(config);
        services.AddSingleton<ISimpleMapper, SimpleMapper.SimpleMapper>();
        return services;
    }
}
