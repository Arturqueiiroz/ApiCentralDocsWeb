using ApiCentralDocsWeb.Data;
using ApiCentralDocsWeb.Model;
using ApiCentralDocsWeb.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace ApiCentralDocsWeb.Services
{
    public class FotoService
    {
        private readonly AppDbContext _context;

        public FotoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FotoDTO>> GetAll()
        {
            return await _context.Fotos
                .Select(f => new FotoDTO
                {
                    Id = f.Id,
                    Url = f.Url,
                    Descricao = f.Descricao,
                    DocumentoId = f.DocumentoId
                })
                .ToListAsync();
        }

        public async Task<dynamic> Criar(CriarFotoDTO dados)
        {
            var documento = await _context.Documentos.FindAsync(dados.DocumentoId);

            if (documento == null)
                return new { Erro = true, Mensagem = "Documento não encontrado" };

            var foto = new Foto
            {
                Url = dados.Url,
                DocumentoId = dados.DocumentoId,
                Descricao = dados.Descricao
            };

            _context.Fotos.Add(foto);
            await _context.SaveChangesAsync();

            return new { Erro = false, Foto = foto };
        }

        public async Task<dynamic> Deletar(int id)
        {
            var foto = await _context.Fotos.FindAsync(id);

            if (foto == null)
                return new { Erro = true, Mensagem = "Foto não encontrada" };

            _context.Fotos.Remove(foto);
            await _context.SaveChangesAsync();

            return new { Erro = false };
        }
    }
}