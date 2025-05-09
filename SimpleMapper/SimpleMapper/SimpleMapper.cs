using System.Collections;
using SimpleMapper.Interface;

namespace SimpleMapper;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
}

public class SimpleMapper : IMapper
{
    private readonly Dictionary<(Type, Type), IMapAction> _mappings;

    public SimpleMapper(Dictionary<(Type, Type), IMapAction> mappings)
    {
        _mappings = mappings;
    }

    public TDestination Map<TDestination>(object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        // Se for uma lista, cria uma lista do tipo correto
        if (source is IEnumerable enumerable)
        {
            var resultList = new List<object>();
            var destType = typeof(TDestination);

            // Se o destino não é uma lista, não podemos mapear
            if (!IsListType(destType))
            {
                throw new InvalidOperationException(
                    $"Não é possível mapear uma lista para um tipo não-lista: {destType.Name}");
            }

            // Obtém o tipo dos elementos da lista
            var elementType = destType.GetGenericArguments()[0];
            
            foreach (var item in enumerable)
            {
                if (item is null) continue;

                var key = (item.GetType(), elementType);
                if (!_mappings.TryGetValue(key, out var action))
                {
                    throw new InvalidOperationException(
                        $"Mapeamento não encontrado para {item.GetType().Name} → {elementType.Name}");
                }

                resultList.Add(action.Map(item));
            }

            // Converte a lista para o tipo correto
            var typedList = resultList.Select(x => Convert.ChangeType(x, elementType)).ToList();
            
            var listType = typeof(List<>).MakeGenericType(elementType);

            var destinationList = (IList)Activator.CreateInstance(listType)!;

            foreach (var item in typedList)
            {
                destinationList.Add(item);
            }

            return (TDestination)destinationList!;
        }

        
        // Mapeamento direto
        var directKey = (source.GetType(), typeof(TDestination));
        if (_mappings.TryGetValue(directKey, out var directAction))
        {
            return (TDestination)directAction.Map(source);
        }

        //Mapeamento reverso
        var reverseKey = (typeof(TDestination), source.GetType());
        if (_mappings.TryGetValue(reverseKey, out var reverseAction))
        {
            return (TDestination)reverseAction.Map(source);
        }

        throw new InvalidOperationException(
            $"Mapeamento não encontrado para {source.GetType().Name} → {typeof(TDestination).Name}");
    }

    private static bool IsListType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
}