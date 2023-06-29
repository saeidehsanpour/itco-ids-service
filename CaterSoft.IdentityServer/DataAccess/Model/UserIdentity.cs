using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace CaterSoft.IdentityServer.DataAccess.Model
{
    public class UserIdentity : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
    }

}
