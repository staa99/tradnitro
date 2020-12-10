using System;
using System.Net;
using System.Security.Claims;
using Tradnitro.Shared.Enums;


namespace Tradnitro.Shared.Audit
{
    public interface IAuditSecurityData
    {
        int CompanyId { get; }
        bool ChangedPassword { get; }
        string Email { get; }
        string GivenName { get; }
        IPAddress IpAddress { get; }
        string Phone { get; }
        string Purpose { get; set; }
        string RequesterName { get; }
        string UserAgent { get; }
        int UserId { get; }
        DateTime UtcTime { get; }
        bool VerifiedEmail { get; }

        void RefreshPrincipal(ClaimsPrincipal principal);
    }
}