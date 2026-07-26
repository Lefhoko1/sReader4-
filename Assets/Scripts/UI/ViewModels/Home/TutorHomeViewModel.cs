using System;
using System.Threading.Tasks;
using SReader.Domains.Identity.Services;

namespace SReader.UI.ViewModels.Home
{
    /// <summary>Home for tutors — managing learners, assignments and sessions.</summary>
    public sealed class TutorHomeViewModel : RoleHomeViewModel
    {
        public TutorHomeViewModel(IUserService users = null, Func<Task> signOut = null)
            : base(users, signOut) { }

        public override string Title => "Tutor dashboard";
        public override string Tagline => "Set work, review submissions and guide your learners.";
    }
}
