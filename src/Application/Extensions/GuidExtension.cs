using System.Security.Cryptography;

namespace Application.Extensions
{
    public static class GuidExtension
    {
        public static Guid GetGuid(this string prop)
        {
            Guid.TryParse(prop, out Guid guid);
            return guid;
        }
    }
}
