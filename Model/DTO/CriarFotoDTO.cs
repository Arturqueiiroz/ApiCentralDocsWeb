namespace ApiCentralDocsWeb.Model.DTO
{
    public class CriarFotoDTO
    {
        public string Url { get; set; } = null!;
        public int DocumentoId { get; set; }
        public string? Descricao { get; set; }
    }
}