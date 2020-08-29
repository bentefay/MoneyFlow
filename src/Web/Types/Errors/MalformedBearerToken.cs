using System.Linq;
using Web.Utils.Extensions;

namespace Web.Types.Errors
{
    public class MalformedBearerToken : IError, IParseAuthorizationErrors
    {
        public string AuthorizationHeader { get; }

        public MalformedBearerToken(string authorizationHeader)
        {
            AuthorizationHeader = authorizationHeader
                .Split(" ")
                .Select(token => ReplaceAfter(10, token))
                .Join(" ");
        }

        private static string ReplaceAfter(int count, string token)
        {
            return token.Length > count ? token.Substring(0, count) + new string('*', token.Length - count) : token;
        }

        public string GetDescription() => $"Expected header 'Authorization: Bearer <Authorization>' but got 'Authorization: {AuthorizationHeader}'";
    }
}