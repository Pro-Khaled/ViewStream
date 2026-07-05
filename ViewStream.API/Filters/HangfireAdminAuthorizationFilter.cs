using Hangfire.Dashboard;

namespace ViewStream.API.Filters
{
    /// <summary>
    /// Restricts Hangfire Dashboard access to Admin and SuperAdmin roles.
    /// </summary>
    public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            if (httpContext.User.Identity?.IsAuthenticated != true)
                return false;

            return httpContext.User.IsInRole("Admin") || httpContext.User.IsInRole("SuperAdmin");
        }
    }
}
