namespace Tradnitro.Shared.Enums
{
    public static class ApplicationClaimTypes
    {
        public const string Blocked         = nameof(Blocked);
        public const string ChangedPassword = nameof(ChangedPassword);
        public const string Id              = nameof(Id);
        public const string MFAExpiry       = nameof(MFAExpiry);
        public const string Permission      = nameof(Permission);
        public const string UserType        = nameof(UserType);
        public const string VerifiedEmail   = nameof(VerifiedEmail);
        public const string CompanyId = nameof(CompanyId);
    }
    public enum FieldType
    {
        Text,
        Numeric,
        Date
    }
}