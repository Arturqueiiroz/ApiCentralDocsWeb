using System.ComponentModel.DataAnnotations;

namespace ApiCentralDocsWeb.Model
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required (ErrorMessage = "Nome é um campo obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "CPF é um campo obrigatório")]
        [StringLength(11, ErrorMessage = "CPF deve conter exatamente 11 caracteres")]
        public string CPF { get; set; } = string.Empty;

        [Required (ErrorMessage = "Email é um campo obrigatório")]
        public string Email { get; set; } = string.Empty;
        [Required (ErrorMessage = "Senha é um campo obrigatório")]
        public string Senha { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }

        public Usuario()
        {
            DataCriacao = DateTime.UtcNow;
            Ativo = true;
        }
    }
}
