using SimpleMapper.Configuration;
using SimpleMapper.Example.Dto;
using SimpleMapper.Example.Model;
using SimpleMapper.Interface;

namespace SimpleMapper.Example.MapperHandle;

public class AlunoMapperProfile : ISimpleMapperProfile
{
    public void Configure(SimpleMapperConfiguration config)
    {
        config.CreateMapper<AlunoDto, Aluno>()
            .ForMember((dest, val) => dest.Nome = val, orig => orig.PrimeiroNome)
            .ForMember((dest, val) => dest.Email = val, orig => orig.Contato);
    }
    
}
