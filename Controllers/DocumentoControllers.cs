using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Model;
using ApiCentralDocsWeb.Model.DTO;


namespace ApiCentralDocsWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var documentos = await _context.Documentos
                .Include(d => d.Usuario)
                .Include(d => d.TipoDocumento)
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
            var documento = await _context.Documentos
                .Include(d => d.Usuario)
                .Include(d => d.TipoDocumento)
                .FirstOrDefaultAsync(d => d.Id == id);

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

            var usuario = await _context.Usuarios.FindAsync(dados.UsuarioId);
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
                UsuarioId = dados.UsuarioId,
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
            var documento = await _context.Documentos.FindAsync(id);

            if (documento == null)
                return NotFound("Documento não encontrado");

            _context.Documentos.Remove(documento);
            await _context.SaveChangesAsync();

            return Ok("Documento deletado com sucesso");
        }
    }
}