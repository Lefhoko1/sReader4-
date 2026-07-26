using System;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels.Home
{
    /// <summary>
    /// Shared backing for the role-specific home screens. Loads the signed-in
    /// user (for the greeting) and exposes sign-out. Each role subclass adds the
    /// title and tagline that describe what that role does, keeping the loading
    /// logic in one place (MVVM, no duplication across the three homes).
    /// </summary>
    public abstract class RoleHomeViewModel : ViewModelBase
    {
        readonly IUserService users;
        readonly Func<Task> signOut;

        User user;

        public string DisplayName { get; private set; } = "";
        public string Username { get; private set; } = "";
        public string Email => user?.Email ?? "";
        public string AvatarUrl { get; private set; } = "";
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// The single short name to greet the user by: their username when set,
        /// otherwise just their first name — never the full "name surname".
        /// </summary>
        public string ShortName =>
            !string.IsNullOrWhiteSpace(Username) ? Username.Trim() : FirstName();

        public string Greeting =>
            string.IsNullOrEmpty(ShortName) ? "Welcome" : $"Welcome, {ShortName}";

        /// <summary>Short heading naming the workspace, e.g. "Student dashboard".</summary>
        public abstract string Title { get; }

        /// <summary>One line describing what this role can do here.</summary>
        public abstract string Tagline { get; }

        protected RoleHomeViewModel(IUserService users = null, Func<Task> signOut = null)
        {
            this.users = users;
            this.signOut = signOut;
        }

        public async Task<Result> LoadAsync()
        {
            if (users == null)
            {
                // Placeholder mode: show a sample name so the layout is visible
                // in-editor before services are wired.
                DisplayName = "Reader";
                IsLoaded = true;
                Raise(nameof(IsLoaded));
                return PlaceholderOk();
            }

            IsBusy = true;
            var result = await users.GetCurrentUserAsync();
            IsBusy = false;

            if (result.IsFailure)
            {
                ErrorMessage = result.Error;
                return Result.Fail(result.Error);
            }

            user = result.Value;
            DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? "" : user.DisplayName.Trim();
            Username = string.IsNullOrWhiteSpace(user.Username) ? "" : user.Username.Trim();

            // Fetch the latest profile picture so the avatar reflects updates.
            var profile = await users.GetProfileAsync(user.Id);
            AvatarUrl = profile.IsSuccess ? (profile.Value?.ProfileImageUrl ?? "") : "";

            IsLoaded = true;
            ErrorMessage = "";
            Raise(nameof(IsLoaded));
            return Result.Ok();
        }

        public async Task SignOutAsync()
        {
            if (signOut != null) await signOut();
        }

        string FirstName()
        {
            var name = (DisplayName ?? "").Trim();
            if (name.Length == 0) return "";
            var space = name.IndexOf(' ');
            return space > 0 ? name.Substring(0, space) : name;
        }
    }
}
