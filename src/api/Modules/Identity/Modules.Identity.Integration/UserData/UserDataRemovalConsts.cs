namespace Modules.Identity.Integration.UserData;

public static class UserDataRemovalConsts
{
    public static readonly string IntegrationStreamName = 
        $"{typeof(UserDataRemovalConsts).Namespace!}-integration-stream";
}