using SimpleMapper.Configuration;
using SimpleMapper.Example.Dto;
using SimpleMapper.Example.Model;
using SimpleMapper.Interface;

namespace SimpleMapper.Example.MapperHandle;

public class ProfessorMapperProfile : ISimpleMapperProfile
{
    public void Configure(SimpleMapperConfiguration config)
    {
        config.CreateMapper<ProfessorDto, Professor>();
    }

}
