namespace ApiCentralDocsWeb.Model.DTO
{
    public class FotoDTO
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public string? Descricao { get; set; }
        public int DocumentoId { get; set; }
    }
}