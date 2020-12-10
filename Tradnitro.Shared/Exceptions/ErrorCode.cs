namespace Tradnitro.Shared.Exceptions
{
    public enum ErrorCode
    {
        Unknown                              = 0,
        AccessDenied                         = 100,
        AccessDenied_UnauthenticatedUser     = 101,
        AccessDenied_InsufficientPermissions = 102,
        NotFound                             = 200,
        NotFound_User                        = 201,
        BadSetup                             = 300,
        ForbidDuplicates                     = 400
    }
}