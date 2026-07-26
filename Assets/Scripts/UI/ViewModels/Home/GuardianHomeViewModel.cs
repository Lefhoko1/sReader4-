using System;
using System.Threading.Tasks;
using SReader.Domains.Identity.Services;

namespace SReader.UI.ViewModels.Home
{
    /// <summary>Home for guardians — overseeing the students in their care.</summary>
    public sealed class GuardianHomeViewModel : RoleHomeViewModel
    {
        public GuardianHomeViewModel(IUserService users = null, Func<Task> signOut = null)
            : base(users, signOut) { }

        public override string Title => "Guardian dashboard";
        public override string Tagline => "Follow your students' progress and stay in the loop.";
    }
}
