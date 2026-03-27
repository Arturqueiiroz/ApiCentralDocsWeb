using System.ComponentModel.DataAnnotations;

namespace ApiCentralDocsWeb.Model.DTO
{
    public class CriarUsuarioDTO
    {
        public string Nome { get; set; } = string.Empty;
        [Required(ErrorMessage = "CPF é um campo obrigatório")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 números")]
        public string CPF { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email é um campo obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; }

    }
}
