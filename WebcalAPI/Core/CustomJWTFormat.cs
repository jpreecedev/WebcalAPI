using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Thinktecture.IdentityModel.Tokens;

namespace WebcalAPI.Core
{
    public class CustomJWTFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string AudiencePropertyKey = "audience";
        private static readonly string Secret = ConfigurationManager.AppSettings["secret"];
        private static readonly string Audience = ConfigurationManager.AppSettings["audience"];

        private readonly string _issuer;

        public CustomJWTFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var audienceId = data.Properties.Dictionary.ContainsKey(AudiencePropertyKey) ? data.Properties.Dictionary[AudiencePropertyKey] : null;

            if (string.IsNullOrWhiteSpace(audienceId))
            {
                throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");
            }
            
            var keyByteArray = TextEncodings.Base64Url.Decode(Secret);
            var signingKey = new HmacSigningCredentials(keyByteArray);
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;
            
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_issuer, Audience, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey));
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}