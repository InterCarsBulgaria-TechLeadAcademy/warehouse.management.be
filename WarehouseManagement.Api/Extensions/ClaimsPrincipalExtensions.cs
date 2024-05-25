namespace System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the unique identifier of the user.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance representing the user.</param>
    /// <returns>The unique identifier of the user as a string, or null if not found.</returns>
    public static string Id(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user"; //TODO: Fix when we implement Authentication
    }
}
