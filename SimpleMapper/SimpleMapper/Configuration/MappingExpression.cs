using System.Linq.Expressions;
using SimpleMapper.Util;

namespace SimpleMapper.Configuration;

public class MappingExpression<TSource, TDestination>
{
    private readonly List<string> _configuredProperties = new();
    private readonly List<Action<TSource, TDestination>> _propertyMappings = new();
    private readonly Dictionary<string, string> _propertyNameMaps = new();

    public class PropertyMapping
    {
        public string? SourceProperty { get; set; }
        public string? DestinationProperty { get; set; }
    }

    /// <summary>
    /// Configura explicitamente o mapeamento de uma propriedade de destino 
    /// com base em uma propriedade de origem.
    /// </summary>
    public void ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Func<TSource, TMember> valueResolver)
    {
        if (destinationMember.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            _configuredProperties.Add(propertyName);

            // Store the mapping information
            _propertyNameMaps[GetSourcePropertyName(valueResolver)] = propertyName;

            Console.WriteLine($"ForMember registrado: {propertyName}");

            var setter = PropertyAccessor<TDestination>.GetSetter(propertyName);

            if (setter == null)
            {
                throw new InvalidOperationException($"Setter não encontrado para a propriedade '{propertyName}' no tipo '{typeof(TDestination)}'.");
            }

            _propertyMappings.Add((source, destination) =>
            {
                var value = valueResolver(source);
                setter(destination, value);
            });
        }
        else
        {
            throw new InvalidOperationException("A expressão fornecida não é válida para uma propriedade.");
        }
    }

    private string GetSourcePropertyName<TMember>(Func<TSource, TMember> valueResolver)
    {
        // Extract the property name from the resolver expression
        var expression = valueResolver.Target?.ToString() ?? "";
        var parts = expression.Split('.');
        return parts.Length > 0 ? parts[^1] : "";
    }

    public Dictionary<string, string> GetPropertyMappings() => _propertyNameMaps;

    /// <summary>
    /// Verifica se uma propriedade foi configurada explicitamente.
    /// </summary>
    /// <param name="propertyName">O nome da propriedade de destino.</param>
    /// <returns>True se a propriedade foi configurada explicitamente; caso contrário, False.</returns>
    public bool IsConfigured(string propertyName)
    {
        return _configuredProperties.Contains(propertyName);
    }

    /// <summary>
    /// Aplica todas as configurações explícitas ao objeto de destino.
    /// </summary>
    public void Apply(TSource source, TDestination destination)
    {
        // Aplica mapeamentos configurados
        foreach (var mapping in _propertyMappings)
        {
            Console.WriteLine($"Aplicando mapeamento para {typeof(TSource)} -> {typeof(TDestination)}");
            mapping(source, destination);
        }

        // Aplica mapeamentos automáticos para propriedades não configuradas
        var sourceProps = PropertyAccessor<TSource>.GetPropertyNames();
        var destProps = PropertyAccessor<TDestination>.GetPropertyNames();

        foreach (var propName in destProps)
        {
            if (!_configuredProperties.Contains(propName))
            {
                var getter = PropertyAccessor<TSource>.GetGetter(propName);
                var setter = PropertyAccessor<TDestination>.GetSetter(propName);

                if (getter != null && setter != null)
                {
                    var value = getter(source);
                    setter(destination, value);
                }
            }
        }
    }

    /// <summary>
    /// Retorna todas as configurações de mapeamento registradas.
    /// </summary>
    /// <returns>Uma lista de ações de mapeamento.</returns>
    public IEnumerable<Action<TSource, TDestination>> GetMappings()
    {
        return _propertyMappings;
    }

    /// <summary>
    /// Adiciona uma configuração de mapeamento diretamente.
    /// </summary>
    /// <param name="mapping">A ação de mapeamento a ser adicionada.</param>
    public void AddMapping(Action<TSource, TDestination> mapping)
    {
        _propertyMappings.Add(mapping);
    }
}