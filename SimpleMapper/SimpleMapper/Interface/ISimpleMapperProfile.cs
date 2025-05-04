using SimpleMapper.Configuration;

namespace SimpleMapper.Interface;

public interface ISimpleMapperProfile
{
    void Configure(SimpleMapperConfiguration config);
}
