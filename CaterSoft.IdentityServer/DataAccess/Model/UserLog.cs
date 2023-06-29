using System;

namespace CaterSoft.IdentityServer.DataAccess.Model
{
    public class UserLog
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public string Device { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public string UserAgent { get; set; }
        public string Platform { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
        public string Title { get; set; }

    }
}
