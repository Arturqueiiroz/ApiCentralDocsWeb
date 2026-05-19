using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Model;
using ApiCentralDocsWeb.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApiCentralDocsWeb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

        private int? ObterUsuarioIdLogado()
        {
            var idClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("id")?.Value ??
                User.FindFirst("Id")?.Value ??
                User.FindFirst("sub")?.Value;

            if (int.TryParse(idClaim, out int usuarioId))
            {
                return usuarioId;
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioId = ObterUsuarioIdLogado();

            if (usuarioId == null)
                return Unauthorized("Usuário não identificado no token.");

            var documentos = await _context.Documentos
                .Include(d => d.Usuario)
                .Include(d => d.TipoDocumento)
                .Where(d => d.UsuarioId == usuarioId)
                .ToListAsync();

            var resultado = documentos.Select(d => new DocumentoDTO
            {
                Id = d.Id,
                Numero = d.Numero,
                OrgaoEmissor = d.OrgaoEmissor,
                CidadeEmissao = d.CidadeEmissao,
                Usuario = d.Usuario?.Nome,
                Tipo = d.TipoDocumento?.Nome
            }).ToList();

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();

            if (usuarioId == null)
                return Unauthorized("Usuário não identificado no token.");

            var documento = await _context.Documentos
                .Include(d => d.Usuario)
                .Include(d => d.TipoDocumento)
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == usuarioId);

            if (documento == null)
                return NotFound("Documento não encontrado");

            var resultado = new DocumentoDTO
            {
                Id = documento.Id,
                Numero = documento.Numero,
                OrgaoEmissor = documento.OrgaoEmissor,
                CidadeEmissao = documento.CidadeEmissao,
                Usuario = documento.Usuario?.Nome,
                Tipo = documento.TipoDocumento?.Nome
            };

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarDocumentoDTO dados)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = ObterUsuarioIdLogado();

            if (usuarioId == null)
                return Unauthorized("Usuário não identificado no token.");

            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
                return BadRequest("Usuário não encontrado");

            var tipo = await _context.TiposDocumento.FindAsync(dados.TipoDocumentoId);

            if (tipo == null)
                return BadRequest("Tipo de documento não encontrado");

            var documento = new Documento
            {
                Numero = dados.Numero,
                OrgaoEmissor = dados.OrgaoEmissor,
                DataEmissao = dados.DataEmissao,
                CidadeEmissao = dados.CidadeEmissao,

                // Agora o usuário vem do token, não do front-end
                UsuarioId = usuarioId.Value,

                TipoDocumentoId = dados.TipoDocumentoId
            };

            _context.Documentos.Add(documento);
            await _context.SaveChangesAsync();

            var resultado = new DocumentoDTO
            {
                Id = documento.Id,
                Numero = documento.Numero,
                OrgaoEmissor = documento.OrgaoEmissor,
                CidadeEmissao = documento.CidadeEmissao,
                Usuario = usuario.Nome,
                Tipo = tipo.Nome
            };

            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();

            if (usuarioId == null)
                return Unauthorized("Usuário não identificado no token.");

            var documento = await _context.Documentos
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == usuarioId);

            if (documento == null)
                return NotFound("Documento não encontrado");

            _context.Documentos.Remove(documento);
            await _context.SaveChangesAsync();

            return Ok("Documento deletado com sucesso");
        }
    }
}