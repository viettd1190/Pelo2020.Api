using System.Security.Principal;

namespace Pelo.Api.Services.UserServices.Auth
{
    public class JwtAuthenticationIdentity : GenericIdentity
    {
        public JwtAuthenticationIdentity(string username) : base(username)
        {
            Username = username;
        }

        public JwtAuthenticationIdentity(string username,
            int id) : base(username)
        {
            Id = id;
        }

        public int Id { get; set; }

        public string Username { get; set; }
    }
}