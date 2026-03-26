using ApiCentralDocsWeb.Model;

namespace ApiCentralDocsWeb.Interfaces
{
    // Cada service tem sua interface para facilitar a manutenção e a testabilidade do código, além de seguir o princípio de inversão de dependência.
    // A interface ITokenService define o contrato para a geração de tokens JWT, permitindo que a implementação possa ser facilmente substituída ou mockada em testes.
    public interface ITokenService
    {
       string GerarToken(Usuario usuario);

    }
}
