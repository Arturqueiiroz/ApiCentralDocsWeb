namespace ApiCentralDocsWeb.Model.DTO
{
    public class DocumentoDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string OrgaoEmissor { get; set; } = string.Empty;
        public string CidadeEmissao { get; set; } = string.Empty;
        public string? Usuario { get; set; }
        public string? Tipo { get; set; }
    }
}