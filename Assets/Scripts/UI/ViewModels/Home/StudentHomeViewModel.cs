using System;
using System.Threading.Tasks;
using SReader.Domains.Identity.Services;

namespace SReader.UI.ViewModels.Home
{
    /// <summary>Home for students — reading, assignments and progress.</summary>
    public sealed class StudentHomeViewModel : RoleHomeViewModel
    {
        public StudentHomeViewModel(IUserService users = null, Func<Task> signOut = null)
            : base(users, signOut) { }

        public override string Title => "Student dashboard";
        public override string Tagline => "Pick up where you left off and keep your streak going.";
    }
}
