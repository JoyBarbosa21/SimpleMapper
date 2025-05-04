using SimpleMapper.Example.Dto;
using SimpleMapper.Example.Model;
using SimpleMapper.Interface;

namespace SimpleMapper.Example;

public class App
{
    private readonly ISimpleMapper _mapper;

    public App(ISimpleMapper mapper)
    {
        _mapper = mapper;
    }
    
    public void Run()
    {
        var professor = new Professor { Nome = "Barbosa", Email = "Barbosa@email.com" };
        
        //Mapper sem ForMember   
        var dtoProfessor = _mapper.Map<ProfessorDto>(professor);
        
        Console.WriteLine($"DTO: Nome = {dtoProfessor.Nome}, Email = {dtoProfessor.Email}");
        
        var aluno = new Aluno { PrimeiroNome = "William", Contato = "Barbosa@email.com" };
        
        //Mapper com ForMember
        var dtoAluno = _mapper.Map<AlunoDto>(aluno);

        Console.WriteLine($"DTO: Nome = {dtoAluno.Nome}, Email = {dtoAluno.Email}");

        

    }
}
