using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleMapper.Configuration;

public class SimpleMapperConfiguration
{
    private readonly Dictionary<(Type Source, Type Destination), Action<object, object>> _mappings = new();

    /// <summary>
    /// Cria um mapeamento entre dois tipos, com suporte a configurações explícitas e mapeamento automático.
    /// </summary>
    public MappingExpressionWithReverse<TSource, TDestination> CreateMap<TSource, TDestination>(Action<MappingExpression<TSource, TDestination>>? config = null)
    {
        var mappingExpression = new MappingExpression<TSource, TDestination>();

        config?.Invoke(mappingExpression);

        _mappings[(typeof(TSource), typeof(TDestination))] = (source, destination) =>
        {
            mappingExpression.Apply((TSource)source, (TDestination)destination);

            var sourceProperties = typeof(TSource).GetProperties();
            var destinationProperties = typeof(TDestination).GetProperties();

            foreach (var destProp in destinationProperties)
            {
                if (destProp.CanWrite && !mappingExpression.IsConfigured(destProp.Name))
                {
                    var sourceProp = sourceProperties.FirstOrDefault(sp =>
                        sp.Name == destProp.Name && sp.PropertyType == destProp.PropertyType);

                    if (sourceProp != null)
                    {
                        mappingExpression.AddMapping((source, destination) =>
                        {
                            var value = sourceProp.GetValue(source);
                            destProp.SetValue(destination, value);
                        });
                    }
                    else
                    {
                        Console.WriteLine($"No matching source property found for {destProp.Name}");
                    }
                }
            }
        };

        // Retorna um objeto que permite configurar o ReverseMap
        return new MappingExpressionWithReverse<TSource, TDestination>(this, mappingExpression);
    }

    /// <summary>
    /// Realiza o mapeamento de um objeto de origem para um objeto de destino.
    /// </summary>
    public TDestination Map<TSource, TDestination>(TSource source)
        where TDestination : new()
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var destination = new TDestination();
        if (_mappings.TryGetValue((typeof(TSource), typeof(TDestination)), out var mapping))
        {
            mapping(source, destination);
        }
        else
        {
            throw new InvalidOperationException($"No mapping exists for {typeof(TSource)} -> {typeof(TDestination)}");
        }

        return destination;
    }

    /// <summary>
    /// Adiciona um mapeamento ao dicionário de mapeamentos.
    /// </summary>
    public void AddMapping<TSource, TDestination>(Action<object, object> mapping)
    {
        _mappings[(typeof(TSource), typeof(TDestination))] = mapping;
    }

    /// <summary>
    /// Tenta obter um mapeamento registrado entre dois tipos.
    /// </summary>
    public bool TryGetMapping(Type sourceType, Type destinationType, out Action<object, object>? mapping)
    {
        return _mappings.TryGetValue((sourceType, destinationType), out mapping);
    }

}
