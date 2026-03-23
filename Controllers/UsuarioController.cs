using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Model;
using ApiCentralDocsWeb.Model.DTO;
using ApiCentralDocsWeb.Services;
using Microsoft.IdentityModel.Tokens;

namespace ApiCentralDocsWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsuarios()
        {
            List<Usuario> usuarios = await _usuarioService.GetAllUsuarios();
            return Ok(usuarios);
        }

        [HttpGet("GetById{id}")]
        public async Task<IActionResult> GetUsuarioById([FromRoute] int id)
        {
            dynamic usuario = await _usuarioService.GetUsuarioById(id);

            if (usuario.Usuario == null)

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
                return BadRequest(ModelState);
            var resultado = await _usuarioService.CriarUsuario(dadosUsuario);
            if (resultado.Erro)
                return BadRequest(resultado);
            return Ok(resultado);
        }
        [HttpPut("AtualizarUsuario/{id}")]
        public async Task<IActionResult> AtualizarUsuario(int id, AtualizarUsuarioDTO dadosUsuario)
        {
            var resultado = await _usuarioService.AtualizarUsuario(id, dadosUsuario);

            if (resultado.Erro)

                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpDelete("DeletarUsuario/{id}")]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            var resultado = await _usuarioService.DeletarUsuario(id);

            if (resultado.Erro)
                return BadRequest(resultado);

            return Ok("Usuário deletado com sucesso");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dados)
        {
            var resultado = await _usuarioService.Login(dados);

            if (resultado.Erro)
                return Unauthorized(resultado);

            return Ok(resultado);
        }
    }
}