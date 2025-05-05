using System;
using SimpleMapper.Configuration;

namespace SimpleMapper;

public class MappingExpressionWithReverse<TSource, TDestination>
{
    private readonly SimpleMapperConfiguration _config;
    private readonly MappingExpression<TSource, TDestination> _mappingExpression;

    public MappingExpressionWithReverse(SimpleMapperConfiguration config, MappingExpression<TSource, TDestination> mappingExpression)
    {
        _config = config;
        _mappingExpression = mappingExpression;
    }

    /// <summary>
    /// Configura o mapeamento inverso entre os tipos de origem e destino.
    /// </summary>
    public void ReverseMap()
    {
        var reverseMappingExpression = new MappingExpression<TDestination, TSource>();

        foreach (var mapping in _mappingExpression.GetMappings())
        {
            reverseMappingExpression.AddMapping((source, destination) =>
            {
                mapping((TSource)destination, (TDestination)source);
            });
        }

        var sourceProperties = typeof(TDestination).GetProperties();
        var destinationProperties = typeof(TSource).GetProperties();

        foreach (var destProp in destinationProperties)
        {
            if (destProp.CanWrite && !reverseMappingExpression.IsConfigured(destProp.Name))
            {
                var sourceProp = sourceProperties.FirstOrDefault(sp =>
                    sp.Name == destProp.Name && sp.PropertyType == destProp.PropertyType);

                if (sourceProp != null)
                {
                    reverseMappingExpression.AddMapping((source, destination) =>
                    {
                        var value = sourceProp.GetValue(source);
                        destProp.SetValue(destination, value);
                    });
                }
            }
        }

        _config.AddMapping<TDestination, TSource>((source, destination) =>
        {
            reverseMappingExpression.Apply((TDestination)source, (TSource)destination);
        });
    }
}
