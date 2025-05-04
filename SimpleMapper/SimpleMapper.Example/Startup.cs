using Microsoft.Extensions.DependencyInjection;
using SimpleMapper.Example.MapperHandle;

namespace SimpleMapper.Example;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSimpleMapper(c =>
        {
            c.AddProfile<AlunoMapperProfile>();
            c.AddProfile<ProfessorMapperProfile>();
        });

        services.AddTransient<App>();
    }
}
