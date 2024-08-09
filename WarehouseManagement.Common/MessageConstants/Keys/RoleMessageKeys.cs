namespace WarehouseManagement.Common.MessageConstants.Keys;

public static class RoleMessageKeys
{
    public const string RoleWithThisNameDoesNotExist = "RoleWithThisNameDoesNotExist";
    public const string RoleWithThisNameAlreadyExists = "RoleWithThisNameExists";
    public const string RoleCreatedSuccessfully = "RoleAddedSuccessfully";
    public const string RoleEditedSuccessfully = "RoleEditedSuccessfully";
    public const string RoleDeletedSuccessfully = "RoleDeletedSuccessfully";
    public const string RoleAssignedToUserSuccessfully = "RoleAssignedToUserSuccessfully";
    public const string RoleCannotBeCreatedWithoutPermissions = "RoleCannotBeCreatedWithoutPermissions";
}
