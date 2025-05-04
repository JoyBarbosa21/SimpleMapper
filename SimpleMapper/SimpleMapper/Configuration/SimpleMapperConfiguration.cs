using System;

namespace SimpleMapper.Configuration;

public class SimpleMapperConfiguration
{
    internal readonly Dictionary<(Type dest, Type orig), Delegate> _mapeamentos = new();

    public MapperBuilder<TOrigem, TDestino> CreateMapper<TDestino, TOrigem>()
        where TDestino : class, new()
        where TOrigem : class
    {
        var builder = new MapperBuilder<TOrigem, TDestino>(null!);
        _mapeamentos[(typeof(TDestino), typeof(TOrigem))] = builder.BuildFunctionOnly();
        return builder;
    }

    internal Func<object, object> GetMapper(Type dest, Type orig)
    {
        if (_mapeamentos.TryGetValue((dest, orig), out var map))
        {
            return obj => map.DynamicInvoke(obj)!;
        }
        throw new InvalidOperationException("Mapper não configurado.");
    }
}
