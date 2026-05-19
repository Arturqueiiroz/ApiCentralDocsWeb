using System.ComponentModel.DataAnnotations;

namespace ApiCentralDocsWeb.Model.DTO
{
    public class CriarDocumentoDTO
    {
        [Required(ErrorMessage = "Número do documento é obrigatório")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "Órgão emissor é obrigatório")]
        public string OrgaoEmissor { get; set; } = string.Empty;

        public DateTime DataEmissao { get; set; }

        public string CidadeEmissao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo do documento é obrigatório")]
        public int TipoDocumentoId { get; set; }
    }
}