namespace SimpleMapper.Interface;

public interface ISimpleMapper
{
    TDestino Map<TDestino>(object origem) where TDestino : new();
}
