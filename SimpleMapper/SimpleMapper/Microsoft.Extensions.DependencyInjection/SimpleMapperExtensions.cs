using SimpleMapper.Configuration;
using SimpleMapper.Interface;

namespace Microsoft.Extensions.DependencyInjection;
public static class SimpleMapperExtensions
{
     public static IServiceCollection AddSimpleMapper(this IServiceCollection services, Action<SimpleMapperConfiguration> configure)
    {
        var configuration = new SimpleMapperConfiguration();

        configure(configuration);
 
        services.AddSingleton<ISimpleMapper>(new SimpleMapper.SimpleMapper(configuration));


        return services;
    }

    public static void AddProfile<TProfile>(this SimpleMapperConfiguration configuration)
        where TProfile : ISimpleMapperProfile, new()
    {
        var profile = new TProfile();
     
        profile.Configure(configuration);
    }
}
