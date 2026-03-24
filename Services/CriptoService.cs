using BCrypt.Net;
using System.Web.Helpers;

namespace ApiCentralDocsWeb.Services
{
    public class CryptoService
    {
  
        public static string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

      
        public static bool VerifyPassword(string password, string hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }
    }
}