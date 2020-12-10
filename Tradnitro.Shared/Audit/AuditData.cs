using System;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Tradnitro.Shared.Enums;


namespace Tradnitro.Shared.Audit
{
    public class AuditData : IAuditSecurityData
    {
        public AuditData(IHttpContextAccessor httpAccessor)
        {
            var http = httpAccessor.HttpContext;
            UtcTime = DateTime.UtcNow;
            if (http == null)
            {
                // this is not a HTTP request
                RequesterName = "SYSTEM";
            }
            else
            {
                IpAddress = http.Connection.RemoteIpAddress;
                UserAgent = http.Request.Headers["User-Agent"].ToString();
                RequesterName = http.User.Identity!.Name!;

                GivenName = http.User.FindFirstValue(ClaimTypes.GivenName);
                UserId = Convert.ToInt32(http.User.FindFirstValue(ApplicationClaimTypes.Id));
                VerifiedEmail = Convert.ToBoolean(http.User.FindFirstValue(ApplicationClaimTypes.VerifiedEmail));
                ChangedPassword = Convert.ToBoolean(http.User.FindFirstValue(ApplicationClaimTypes.ChangedPassword));
                Email = http.User.FindFirstValue(ClaimTypes.Email);
                Phone = http.User.FindFirstValue(ClaimTypes.MobilePhone);
                CompanyId = Convert.ToInt32(http.User.FindFirstValue(ApplicationClaimTypes.CompanyId));
            }
        }


        public int CompanyId { get; }
        public bool ChangedPassword { get; private set; }
        public string Email { get; private set; }
        public string GivenName { get; private set; }

        public IPAddress IpAddress { get; }
        public string Phone { get; private set; }

        public string Purpose { get; set; }


        public void RefreshPrincipal(ClaimsPrincipal principal)
        {
            RequesterName = principal.Identity.Name;
            GivenName = principal.FindFirstValue(ClaimTypes.GivenName);
            UserId = Convert.ToInt32(principal.FindFirstValue(ApplicationClaimTypes.Id));
            VerifiedEmail = Convert.ToBoolean(principal.FindFirstValue(ApplicationClaimTypes.VerifiedEmail));
            ChangedPassword = Convert.ToBoolean(principal.FindFirstValue(ApplicationClaimTypes.ChangedPassword));
            Email = principal.FindFirstValue(ClaimTypes.Email);
            Phone = principal.FindFirstValue(ClaimTypes.MobilePhone);
        }


        public string RequesterName { get; private set; }
        public string UserAgent { get; }
        public int UserId { get; private set; }
        public DateTime UtcTime { get; }
        public bool VerifiedEmail { get; private set; }
    }
}