using System.ComponentModel.DataAnnotations;

namespace ApiCentralDocsWeb.Model.DTO
{
    public class CriarDocumentoDTO
    {
        [Required]
        public string Numero { get; set; } = string.Empty;
        [Required]
        public string OrgaoEmissor { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public string CidadeEmissao { get; set; } = string.Empty;
        [Required]
        public int UsuarioId { get; set; }
        [Required]
        public int TipoDocumentoId { get; set; }
    }
}
