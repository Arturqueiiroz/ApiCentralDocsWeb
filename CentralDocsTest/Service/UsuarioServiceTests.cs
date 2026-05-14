using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Interfaces;
using ApiCentralDocsWeb.Model;
using ApiCentralDocsWeb.Model.DTO;
using ApiCentralDocsWeb.Services;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace CentralDocsTest.Service
{
    public class UsuarioServiceTests
    {
        private AppDbContext CreateContext()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;




            return new AppDbContext(options);
        }

        private UsuarioService CreateService(AppDbContext context)
        {
            ITokenService tokenService = new FakeTokenService();
            return new UsuarioService(context, tokenService);
        }

        private class FakeTokenService : ITokenService
        {
            public string GerarToken(Usuario usuario)
            {
                return "token-fake-para-teste";
            }
        }

        private T ObterValor<T>(object objeto, string propriedade)
        {
            var prop = objeto.GetType().GetProperty(propriedade);

            if (prop == null)
                throw new Exception($"A propriedade '{propriedade}' não foi encontrada no objeto retornado.");

            return (T)prop.GetValue(objeto)!;
        }

        private Usuario CriarUsuarioMock(
            string nome = "Artur Queiroz",
            string cpf = "11111111111",
            string email = "artur@email.com",
            string senha = "123456")
        {
            return new Usuario
            {
                Nome = nome,
                CPF = cpf,
                Email = email,
                Senha = CryptoService.EncryptPassword(senha),
                DataCriacao = DateTime.UtcNow,
                Ativo = true,
                Documentos = new List<Documento>()
            };
        }

        private CriarUsuarioDTO CriarUsuarioDTOMock(
            string nome = "Artur Queiroz",
            string cpf = "11111111111",
            string email = "artur@email.com",
            string senha = "123456",
            string confirmarSenha = "123456")
        {
            return new CriarUsuarioDTO
            {
                Nome = nome,
                CPF = cpf,
                Email = email,
                Senha = senha,
                ConfirmarSenha = confirmarSenha
            };
        }

        private AtualizarUsuarioDTO AtualizarUsuarioDTOMock(
            string nome = "Nome Atualizado",
            string email = "atualizado@email.com",
            string senha = "novaSenha123")
        {
            return new AtualizarUsuarioDTO
            {
                Nome = nome,
                Email = email,
                Senha = senha
            };
        }

        private LoginDTO LoginDTOMock(
            string email = "artur@email.com",
            string senha = "123456")
        {
            return new LoginDTO
            {
                Email = email,
                Senha = senha
            };
        }

        [Fact]
        public async Task GetAllUsuarios_QuandoExistemUsuarios_DeveRetornarTodos()
        {
            AppDbContext context = CreateContext();

            context.Usuarios.AddRange(
                CriarUsuarioMock(
                    nome: "Artur Queiroz",
                    cpf: "11111111111",
                    email: "artur@email.com"
                ),
                CriarUsuarioMock(
                    nome: "Maria Silva",
                    cpf: "22222222222",
                    email: "maria@email.com"
                ),
                CriarUsuarioMock(
                    nome: "João Santos",
                    cpf: "33333333333",
                    email: "joao@email.com"
                )
            );

            await context.SaveChangesAsync();

            UsuarioService service = CreateService(context);

            List<Usuario> usuarios = await service.GetAllUsuarios();

            usuarios.Should().NotBeNull();
            usuarios.Should().HaveCount(3);
            usuarios.Should().Contain(usuario => usuario.Nome == "Artur Queiroz");
            usuarios.Should().Contain(usuario => usuario.Email == "maria@email.com");
            usuarios.Should().OnlyContain(usuario => usuario.Ativo == true);
        }

        [Fact]
        public async Task GetAllUsuarios_QuandoBancoVazio_DeveRetornarListaVazia()
        {
            AppDbContext context = CreateContext();
            UsuarioService service = CreateService(context);

            List<Usuario> usuarios = await service.GetAllUsuarios();

            usuarios.Should().NotBeNull();
            usuarios.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUsuarioById_QuandoUsuarioExiste_DeveRetornarUsuarioCorreto()
        {
            AppDbContext context = CreateContext();

            Usuario usuarioCriado = CriarUsuarioMock(
                nome: "Felipe Aragão",
                cpf: "44444444444",
                email: "felipe@email.com"
            );

            context.Usuarios.Add(usuarioCriado);
            await context.SaveChangesAsync();

            UsuarioService service = CreateService(context);

            object resultado = await service.GetUsuarioById(usuarioCriado.Id);

            ObterValor<bool>(resultado, "Erro").Should().BeFalse();

            Usuario usuario = ObterValor<Usuario>(resultado, "Usuario");

            usuario.Should().NotBeNull();
            usuario.Nome.Should().Be("Felipe Aragão");
            usuario.Email.Should().Be("felipe@email.com");
        }

        [Fact]
        public async Task GetUsuarioById_QuandoUsuarioNaoExiste_DeveRetornarErro()
        {
            AppDbContext context = CreateContext();
            UsuarioService service = CreateService(context);

            object resultado = await service.GetUsuarioById(999);

            ObterValor<bool>(resultado, "Erro").Should().BeTrue();
            ObterValor<string>(resultado, "Mensagem").Should().Be("Usuário com id 999 não encontrado");
        }

        [Fact]
        public async Task CriarUsuario_QuandoDadosValidos_DeveCriarUsuario()
        {
            AppDbContext context = CreateContext();
            UsuarioService service = CreateService(context);

            CriarUsuarioDTO dados = CriarUsuarioDTOMock(
                nome: "Artur Queiroz",
                cpf: "55555555555",
                email: "artur@email.com",
                senha: "123456",
                confirmarSenha: "123456"
            );

            object resultado = await service.CriarUsuario(dados);

            ObterValor<bool>(resultado, "Erro").Should().BeFalse();

            Usuario usuario = ObterValor<Usuario>(resultado, "Usuario");

            usuario.Should().NotBeNull();
            usuario.Nome.Should().Be("Artur Queiroz");
            usuario.CPF.Should().Be("55555555555");
            usuario.Email.Should().Be("artur@email.com");
            usuario.Ativo.Should().BeTrue();
            usuario.DataCriacao.Should().NotBe(default);

            context.Usuarios.Should().HaveCount(1);
        }

        [Fact]
        public async Task CriarUsuario_QuandoSenhasNaoCoincidem_DeveRetornarErro()
        {
            AppDbContext context = CreateContext();
            UsuarioService service = CreateService(context);

            CriarUsuarioDTO dados = CriarUsuarioDTOMock(
                nome: "Artur Queiroz",
                cpf: "66666666666",
                email: "artur@email.com",
                senha: "123456",
                confirmarSenha: "senhaDiferente"
            );

            object resultado = await service.CriarUsuario(dados);

            ObterValor<bool>(resultado, "Erro").Should().BeTrue();
            ObterValor<string>(resultado, "Mensagem").Should().Be("As senhas não coincidem");

            context.Usuarios.Should().BeEmpty();
        }

        [Fact]
        public async Task CriarUsuario_QuandoCPFJaExiste_DeveRetornarErro()
        {
            AppDbContext context = CreateContext();

            Usuario usuarioExistente = CriarUsuarioMock(
                nome: "Usuário Existente",
                cpf: "77777777777",
                email: "existente@email.com"
            );

            context.Usuarios.Add(usuarioExistente);
            await context.SaveChangesAsync();

            UsuarioService service = CreateService(context);

            CriarUsuarioDTO dados = CriarUsuarioDTOMock(
                nome: "Novo Usuário",
                cpf: "77777777777",
                email: "novo@email.com",
                senha: "123456",
                confirmarSenha: "123456"
            );

            object resultado = await service.CriarUsuario(dados);

            ObterValor<bool>(resultado, "Erro").Should().BeTrue();
            ObterValor<string>(resultado, "Mensagem").Should().Be("CPF já cadastrado");

            context.Usuarios.Should().HaveCount(1);
        }

        [Fact]
        public async Task AtualizarUsuario_QuandoUsuarioExiste_DeveAtualizarDados()
        {
            AppDbContext context = CreateContext();

            Usuario usuarioCriado = CriarUsuarioMock(
                nome: "Nome Antigo",
                cpf: "88888888888",
                email: "antigo@email.com"
            );

            context.Usuarios.Add(usuarioCriado);
            await context.SaveChangesAsync();

            UsuarioService service = CreateService(context);

            AtualizarUsuarioDTO dados = AtualizarUsuarioDTOMock(
                nome: "Nome Atualizado",
                email: "atualizado@email.com",
                senha: "novaSenha123"
            );

            object resultado = await service.AtualizarUsuario(usuarioCriado.Id, dados);

            ObterValor<bool>(resultado, "Erro").Should().BeFalse();

            Usuario usuario = ObterValor<Usuario>(resultado, "Usuario");

            usuario.Should().NotBeNull();
            usuario.Nome.Should().Be("Nome Atualizado");
            usuario.Email.Should().Be("atualizado@email.com");
        }

        [Fact]
        public async Task AtualizarUsuario_QuandoUsuarioNaoExiste_DeveRetornarErro()
        {
            AppDbContext context = CreateContext();
            UsuarioService service = CreateService(context);

            AtualizarUsuarioDTO dados = AtualizarUsuarioDTOMock(
                nome: "Nome Atualizado",
                email: "atualizado@email.com",
                senha: "123456"
            );

            object resultado = await service.AtualizarUsuario(999, dados);

            ObterValor<bool>(resultado, "Erro").Should().BeTrue();
            ObterValor<string>(resultado, "Mensagem").Should().Be("Usuário não encontrado");
        }

        [Fact]
        public async Task DeletarUsuario_QuandoUsuarioExiste_DeveRemoverUsuario()
        {
            AppDbContext context = CreateContext();

            Usuario usuarioCriado = CriarUsuarioMock(
                nome: "Usuário Para Deletar",
                cpf: "99999999999",
                email: "deletar@email.com"
            );

            context.Usuarios.Add(usuarioCriado);
            await context.SaveChangesAsync();

            UsuarioService service = CreateService(context);

            object resultado = await service.DeletarUsuario(usuarioCriado.Id);

            ObterValor<bool>(resultado, "Erro").Should().BeFalse();

            context.Usuarios.Should().BeEmpty();
        }

        [Fact]
        public async Task DeletarUsuario_QuandoUsuarioNaoExiste_DeveRetornarErro()
        {
            AppDbContext context = CreateContext();
            UsuarioService service = CreateService(context);

            object resultado = await service.DeletarUsuario(999);

            ObterValor<bool>(resultado, "Erro").Should().BeTrue();
            ObterValor<string>(resultado, "Mensagem").Should().Be("Usuário não encontrado");
        }

        [Fact]
        public async Task Login_QuandoEmailNaoExiste_DeveRetornarErro()
        {
            AppDbContext context = CreateContext();
            UsuarioService service = CreateService(context);

            LoginDTO dados = LoginDTOMock(
                email: "naoexiste@email.com",
                senha: "123456"
            );

            object resultado = await service.Login(dados);

            ObterValor<bool>(resultado, "Erro").Should().BeTrue();
            ObterValor<string>(resultado, "Mensagem").Should().Be("Email ou senha inválidos");
        }

        [Fact]
        public async Task Login_QuandoSenhaIncorreta_DeveRetornarErro()
        {
            AppDbContext context = CreateContext();

            Usuario usuario = CriarUsuarioMock(
                nome: "Artur Queiroz",
                cpf: "10101010101",
                email: "artur@email.com",
                senha: "senhaCorreta"
            );

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            UsuarioService service = CreateService(context);

            LoginDTO dados = LoginDTOMock(
                email: "artur@email.com",
                senha: "senhaErrada"
            );

            object resultado = await service.Login(dados);

            ObterValor<bool>(resultado, "Erro").Should().BeTrue();
            ObterValor<string>(resultado, "Mensagem").Should().Be("Email ou senha inválidos");
        }

        [Fact]
        public async Task Login_QuandoDadosValidos_DeveRetornarTokenEUsuario()
        {
            AppDbContext context = CreateContext();

            Usuario usuario = CriarUsuarioMock(
                nome: "Artur Queiroz",
                cpf: "11111111111",
                email: "artur@email.com",
                senha: "123456"
            );

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            UsuarioService service = CreateService(context);

            LoginDTO dados = LoginDTOMock(
                email: "artur@email.com",
                senha: "123456"
            );

            object resultado = await service.Login(dados);

            ObterValor<bool>(resultado, "Erro").Should().BeFalse();
            ObterValor<string>(resultado, "Token").Should().Be("token-fake-para-teste");

            object usuarioRetornado = ObterValor<object>(resultado, "Usuario");

            usuarioRetornado.Should().NotBeNull();

            ObterValor<int>(usuarioRetornado, "Id").Should().Be(usuario.Id);
            ObterValor<string>(usuarioRetornado, "Nome").Should().Be("Artur Queiroz");
            ObterValor<string>(usuarioRetornado, "Email").Should().Be("artur@email.com");
        }
    }
}