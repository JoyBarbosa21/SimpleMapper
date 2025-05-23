using SimpleMapper.Configuration;
using SimpleMapper.Example.Dto;
using SimpleMapper.Example.Model;
using SimpleMapper.Interface;

namespace SimpleMapper.Example.MapperHandle;

public class AlunoMapperProfile : ISimpleMapperProfile
{
    public void Configure(SimpleMapperConfiguration config)
    {
        config.CreateMap<Aluno, AlunoDto>()
            .ForMember(dest => dest.Nome, orig => orig.PrimeiroNome)
            .ForMember(dest => dest.Email, orig => orig.Contato)
            .ReverseMap();
            
    }
    
}
