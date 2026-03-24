using ApiCentralDocsWeb.Model.DTO;
using ApiCentralDocsWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCentralDocsWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FotoController : ControllerBase
    {
        private readonly FotoService _service;

        public FotoController(FotoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var fotos = await _service.GetAll();
            return Ok(fotos);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarFotoDTO dados)
        {
            var result = await _service.Criar(dados);

            if (result.Erro)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var result = await _service.Deletar(id);

            if (result.Erro)
                return NotFound(result);

            return Ok(result);
        }
    }
}
