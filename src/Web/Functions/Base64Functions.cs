using System;
using System.Text;
using LanguageExt;
using Web.Types.Errors;

namespace Web.Functions
{
    public static class Base64Functions
    {
        public static Either<Base64DecodeError, string> DecodeBase64ToString(string base64Json)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64Json));
            }
            catch (Exception e)
            {
                {
                    return new Base64DecodeError(base64Json, e);
                }
            }
        }
    }
}