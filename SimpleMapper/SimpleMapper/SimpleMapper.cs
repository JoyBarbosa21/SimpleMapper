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

    public TDestino Map<TDestino>(object origem) where TDestino : new()
    {
        var func = _config.GetMapper(typeof(TDestino), origem.GetType());
        return (TDestino)func(origem);
    }
}