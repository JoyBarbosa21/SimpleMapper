using SimpleMapper.Example.Dto;
using SimpleMapper.Example.Model;
using SimpleMapper.Interface;

namespace SimpleMapper.Example;

public class App
{
    private readonly IMapper _mapper;

    public App(IMapper mapper)
    {
        _mapper = mapper;
    }
    
    public void Run()
    {
        var professor = new Professor { Nome = "Barbosa", Email = "Barbosa@email.com" };
        
        //Mapper sem ForMember   
        var dtoProfessor = _mapper.Map<ProfessorDto>(professor);
        
        Console.WriteLine($"Mapper sem ForMember: Nome = {dtoProfessor.Nome}, Email = {dtoProfessor.Email}");
        
        //Mapper com ForMember
        var aluno = new Aluno { PrimeiroNome = "William", Contato = "Barbosa@email.com" };
        
        var dtoAluno = _mapper.Map<AlunoDto>(aluno);

        Console.WriteLine($"Mapper com ForMember: Nome = {dtoAluno.Nome}, Email = {dtoAluno.Email}");

        //Mapper Lista
        var professores = new List<Professor>
        {
            new Professor { Nome = "Joao", Email = "Joao@email.com" },
            new Professor { Nome = "Lucas", Email = "Lucas@email.com" },
            new Professor { Nome = "Maria", Email = "Maria@email.com" }
        };

        var dtoProfessores = _mapper.Map<List<ProfessorDto>>(professores);

        foreach (var dto in dtoProfessores)
        {
            Console.WriteLine($"Mapper Lista: Nome = {dto.Nome}, Email = {dto.Email}");
        }
        
        //Mapper Reverse
        var alunoDto = new AlunoDto { Nome = "Fulano", Email = "Fulano@email.com" };
        
        var alunoModel = _mapper.Map<Aluno>(alunoDto);

        Console.WriteLine($"Mapper Reverse: Nome = {alunoModel.PrimeiroNome}, Email = {alunoModel.Contato}");



    }
}
