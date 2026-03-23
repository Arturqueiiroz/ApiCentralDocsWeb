using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Model;
using ApiCentralDocsWeb.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace ApiCentralDocsWeb.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
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
                Senha = dados.Senha
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
            usuario.Senha = dados.Senha;

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
                .FirstOrDefaultAsync(u => u.Email == dados.Email && u.Senha == dados.Senha);

            if (usuario == null)
            {
                return new { Erro = true, Mensagem = "Email ou senha inválidos" };
            }

            return new
            {
                Erro = false,
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


