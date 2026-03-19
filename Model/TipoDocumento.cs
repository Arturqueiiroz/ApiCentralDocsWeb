using System.ComponentModel.DataAnnotations;

namespace ApiCentralDocsWeb.Model
{
    public class TipoDocumento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do tipo de documento é obrigatório")]
        public string Nome { get; set; } = string.Empty;
        public List<Documento> Documentos { get; set; } = new();
    }
}