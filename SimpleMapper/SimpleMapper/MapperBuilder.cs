using System;

namespace SimpleMapper;

public class MapperBuilder<TOrigem, TDestino>
    where TOrigem : class
    where TDestino : class, new()
{
    private readonly List<Action<TOrigem, TDestino>> _mapeamentos = new();
    
    public MapperBuilder(TOrigem _) {}

    public MapperBuilder<TOrigem, TDestino> ForMember<TProp>(
        Action<TDestino, TProp> setDestino,
        Func<TOrigem, TProp> getOrigem)
    {
        _mapeamentos.Add((orig, dest) => setDestino(dest, getOrigem(orig)));
        return this;
    }
    
    public Func<TOrigem, TDestino> BuildFunctionOnly()
    {
        return (origem) =>
        {
            var destino = new TDestino();
            foreach (var map in _mapeamentos)
                map(origem, destino);
            return destino;
        };
    }
}
