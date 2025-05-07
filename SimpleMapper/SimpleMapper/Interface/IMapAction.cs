namespace SimpleMapper.Interface;

public interface IMapAction{
        object Map(object source);
        object MapReverse(object source);
}