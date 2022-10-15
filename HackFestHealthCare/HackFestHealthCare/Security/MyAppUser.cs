using System;
using System.Linq;
using System.Security.Claims;

namespace HackFestHealthCare.Security
{
    public class MyAppUser : ClaimsPrincipal
    {
        public MyAppUser(ClaimsPrincipal principal) : base(principal)
        {

        }

        private string GetClaimValue(string key)
        {
            var claim = this.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }

        public String UserName
        {
            get
            {
                if (this.FindFirst(ClaimTypes.GivenName) == null)
                    return String.Empty;

                return this.FindFirst(ClaimTypes.GivenName).Value;
            }
        }

        public string UserId
        {
            get
            {
                if (this.FindFirst(ClaimTypes.NameIdentifier) == null)
                    return String.Empty;

                return GetClaimValue(ClaimTypes.NameIdentifier);
            }
        }

        public String Name
        {
            get
            {
                if (this.FindFirst(ClaimTypes.Name) == null)
                    return String.Empty;

                return this.FindFirst(ClaimTypes.Name).Value;
            }
        }
        public string Email
        {
            get
            {
                if (this.FindFirst(ClaimTypes.Email) == null)
                    return String.Empty;

                return this.FindFirst(ClaimTypes.Email).Value;
            }
        }
        public bool IsAdmin
        {
            get
            {
                if (this.FindFirst(ClaimTypes.Role) == null)
                    return false;

                return this.FindAll(ClaimTypes.Role).Any(c =>
                {
                    if ("client_admin".Equals(c.Value, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                    else
                        return false;
                });
            }
        }
        public bool IsSupport
        {
            get
            {
                if (this.FindFirst(ClaimTypes.Role) == null)
                    return false;

                return this.FindAll(ClaimTypes.Role).Any(c =>
                {
                    if ("Power_User".Equals(c.Value, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                    else
                        return false;
                });
            }
        }

        public bool IsCustomer
        {
            get
            {
                if (this.FindFirst(ClaimTypes.Role) == null)
                    return false;

                return this.FindAll(ClaimTypes.Role).Any(c =>
                {
                    if ("client_user".Equals(c.Value, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                    else
                        return false;
                });
            }
        }
    }
}
