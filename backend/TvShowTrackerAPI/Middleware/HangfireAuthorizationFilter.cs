using Hangfire.Dashboard;

namespace TvShowTrackerAPI.Middleware;

/// <summary>
/// Hangfire dashboard authorization filter
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}