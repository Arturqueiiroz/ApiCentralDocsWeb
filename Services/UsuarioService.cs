using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Model;
using ApiCentralDocsWeb.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace ApiCentralDocsWeb.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;
        private readonly TokenService _TokenService;

        public UsuarioService(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _TokenService = tokenService;
        }

        public async Task<List<Usuario>> GetAllUsuarios()
        {
            return await _context.Usuarios
                .Include(u => u.Documentos)
                .ToListAsync();
        }

        public async Task<dynamic> GetUsuarioById(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return new
                {
                    Erro = true,
                    Mensagem = $"Usuário com id {id} não encontrado"
                };
            }

            return new
            {
                Erro = false,
                Usuario = usuario
            };
        }

        public async Task<dynamic> CriarUsuario(CriarUsuarioDTO dados)
        {
            if (dados.Senha != dados.ConfirmarSenha)
            {
                return new
                {
                    Erro = true,
                    Mensagem = "As senhas não coincidem"
                };
            }
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CPF == dados.CPF);

            if (usuarioExistente != null)
            {
                return new { Erro = true, Mensagem = "CPF já cadastrado" };
            }

            var usuario = new Usuario
            {
                Nome = dados.Nome,
                CPF = dados.CPF,
                Email = dados.Email,
                Senha = CryptoService.EncryptPassword(dados.Senha)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new { Erro = false, Usuario = usuario };
        }

        public async Task<dynamic> AtualizarUsuario(int id, AtualizarUsuarioDTO dados)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return new { Erro = true, Mensagem = "Usuário não encontrado" };

            usuario.Nome = dados.Nome;
            usuario.Email = dados.Email;

            if (!string.IsNullOrEmpty(dados.Senha))
            {
                usuario.Senha = CryptoService.EncryptPassword(dados.Senha); 
            }

            await _context.SaveChangesAsync();

            return new { Erro = false, Usuario = usuario };
        }

        public async Task<dynamic> DeletarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return new { Erro = true, Mensagem = "Usuário não encontrado" };

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return new { Erro = false };
        }

        public async Task<dynamic> Login(LoginDTO dados)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Documentos)
                    .ThenInclude(d => d.TipoDocumento)
                .FirstOrDefaultAsync(u => u.Email == dados.Email);

            if (usuario == null)
            {
                return new { Erro = true, Mensagem = "Email ou senha inválidos" };
            }

            bool senhaValida = CryptoService.VerifyPassword(dados.Senha, usuario.Senha);

            if (!senhaValida)
            {
                return new { Erro = true, Mensagem = "Email ou senha inválidos" };
            }

            var token = _TokenService.GerarToken(usuario);
            return new
            {
                Erro = false,
                Token = token,
                Usuario = new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    Documentos = usuario.Documentos.Select(d => new
                    {
                        d.Id,
                        d.Numero,
                        d.OrgaoEmissor,
                        d.CidadeEmissao,
                        Tipo = d.TipoDocumento.Nome
                    })
                }
            };
        }
    }
}


