namespace ApiCentralDocsWeb.Model
{
    public class Foto
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public int DocumentoId { get; set; }
        public Documento Documento { get; set; } = null!;
        public string? Descricao { get; set; }
    }
}
