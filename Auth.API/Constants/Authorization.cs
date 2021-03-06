namespace Auth.API.Constants;

public class Authorization
{
    public enum Roles
    {
        Admin,
        PremiumUser,
        NonPremiumUser
    }
    public const string DefaultUserName = "user";
    public const string DefaultEmail = "user@secureapi.com";
    public const string DefaultPassword = "Passw0rd.";
    public const Roles DefaultRole = Roles.NonPremiumUser;
}
