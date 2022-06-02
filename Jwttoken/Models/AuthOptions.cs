using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Jwttoken.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "SirPavidlo"; // издатель токена
        public const string AUDIENCE = "RandomPeople"; // потребитель токена
        const string KEY = "Encryption_key20";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
