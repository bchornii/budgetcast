namespace BudgetCast.Gateways.Bff.Services.Session;

public interface ISessionTrackerService
{
    /// <summary>
    /// Revokes a user session.
    /// </summary>
    /// <param name="uuid">Identifier to revoke session based on</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RevokeAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies if a session is active.
    /// </summary>
    /// <param name="uuid">Identifier to verify upon</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsActiveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies if a session has been revoked.
    /// </summary>
    /// <param name="uuid">Identifier to verify upon</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsRevokedAsync(string uuid, CancellationToken cancellationToken = default);
}