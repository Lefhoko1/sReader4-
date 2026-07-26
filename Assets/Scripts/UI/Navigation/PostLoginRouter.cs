using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Services;

namespace SReader.UI.Navigation
{
    /// <summary>
    /// Decides where a freshly signed-in user lands. Looks up the current
    /// user's role through the Identity service, then asks the
    /// NavigationManager for the matching role-specific home. Used by both the
    /// login screen and the silent session-restore on app start so the routing
    /// rule lives in exactly one place.
    /// </summary>
    public sealed class PostLoginRouter
    {
        readonly NavigationManager navigation;
        readonly IUserService users;

        public PostLoginRouter(NavigationManager navigation, IUserService users)
        {
            this.navigation = Guard.NotNull(navigation, nameof(navigation));
            this.users      = Guard.NotNull(users, nameof(users));
        }

        public async Task RouteAsync()
        {
            // If the role can't be loaded (offline, transient error) fall back to
            // the student home rather than trapping the user on the auth screen.
            var role = UserRole.Student;
            var result = await users.GetCurrentUserAsync();
            if (result.IsSuccess && result.Value != null)
                role = result.Value.Role;

            navigation.ShowHomeForRole(role);
        }
    }
}
