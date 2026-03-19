using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Model;
using ApiCentralDocsWeb.Model.DTO;

namespace ApiCentralDocsWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _context.Usuarios.Include(usuario => usuario.Documentos).ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("GetById{id}")]
        public async Task<IActionResult> GetUsuarioById([FromRoute] int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)

            {
                return BadRequest(new
                {
                    Erro = true,
                    Mensagem = $"Usuário com id {id} não encontrado"
                });
            }

            return Ok(usuario);
        }

        [HttpPost("CriarUsuario")]
        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioDTO dadosUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(usuario => usuario.CPF == dadosUsuario.CPF);

            if (usuarioExistente != null)
            {
                return BadRequest($"Já existe um usuário com CPF {dadosUsuario.CPF}");
            }

            Usuario usuario = new Usuario
            {
                Nome = dadosUsuario.Nome,
                CPF = dadosUsuario.CPF,
                Email = dadosUsuario.Email,
                Senha = dadosUsuario.Senha
            };

            _context.Usuarios.Add(usuario);

            int resultado = await _context.SaveChangesAsync();

            if (resultado > 0)
                return Created($"Usuario {usuario.Nome} criado com sucesso!", usuario);

            return BadRequest("Erro ao criar usuário");
        }

        [HttpPut("AtualizarUsuario/{id}")]
        public async Task<IActionResult> AtualizarUsuario([FromRoute] int id, [FromBody] AtualizarUsuarioDTO dadosUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return BadRequest(new
                {
                    Erro = true,
                    Mensagem = $"Usuário com id {id} não encontrado"
                });
            }
            usuario.Nome = dadosUsuario.Nome;
            usuario.Email = dadosUsuario.Email;
            usuario.Senha = dadosUsuario.Senha;
            _context.Usuarios.Update(usuario);
            int resultado = await _context.SaveChangesAsync();
            if (resultado > 0)
                return Ok(new
                {
                    Mensagem = $"Usuário com id {id} foi atualizado com sucesso",
                    UsuarioAtualizado = usuario
                });
            return BadRequest("Erro ao atualizar usuário");
        }

        [HttpDelete("DeletarUsuario/{id}")]
        public async Task<IActionResult> DeletarUsuario([FromRoute] int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return BadRequest(new
                {
                    Erro = true,
                    Mensagem = $"Usuário com id {id} não encontrado"
                });
            }

            _context.Usuarios.Remove(usuario);

            int resultado = await _context.SaveChangesAsync();

            if (resultado > 0)
                return Ok(new
                {
                    Mensagem = $"Usuário com id {id} foi deletado com sucesso"
                });

            return BadRequest("Erro ao deletar usuário");
        }
    }
}