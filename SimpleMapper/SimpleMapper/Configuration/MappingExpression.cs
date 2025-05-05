using System.Linq.Expressions;

namespace SimpleMapper.Configuration;

public class MappingExpression<TSource, TDestination>
{
    private readonly List<string> _configuredProperties = new();
    private readonly List<Action<TSource, TDestination>> _propertyMappings = new();

    /// <summary>
    /// Configura explicitamente o mapeamento de uma propriedade de destino 
    /// com base em uma propriedade de origem.
    /// </summary>
    public void ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Func<TSource, TMember> valueResolver)
    {
        _propertyMappings.Add((source, destination) =>
        {
            if (destinationMember.Body is MemberExpression memberExpression)
            {
                var propertyInfo = typeof(TDestination).GetProperty(memberExpression.Member.Name);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    var value = valueResolver(source);
                    propertyInfo.SetValue(destination, value);
                }
            }
            else
            {
                throw new InvalidOperationException("A expressão fornecida não é válida para uma propriedade.");
            }
        });
    }

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
        foreach (var mapping in _propertyMappings)
        {
            mapping(source, destination);
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