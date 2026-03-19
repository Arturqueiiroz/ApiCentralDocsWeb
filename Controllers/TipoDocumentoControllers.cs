using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Model;

namespace ApiCentralDocsWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoDocumentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TipoDocumentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tipos = await _context.TiposDocumento.ToListAsync();
            return Ok(tipos);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] TipoDocumento tipo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.TiposDocumento.Add(tipo);
            await _context.SaveChangesAsync();

            return Created("", tipo);
        }
    }
}