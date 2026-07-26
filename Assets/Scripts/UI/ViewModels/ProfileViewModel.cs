using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Education.Services;
using SReader.Domains.Files.Services;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Services;
using SReader.Domains.Locations.Models;
using SReader.Domains.Locations.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    /// <summary>
    /// Backs the signed-in landing/profile screen. Loads the current user,
    /// their profile and saved location, and pushes edits back to Supabase
    /// through the Identity and Locations services.
    /// </summary>
    public sealed class ProfileViewModel : ViewModelBase
    {
        readonly IUserService users;
        readonly ILocationService locations;
        readonly System.Func<Task> signOut;
        readonly IImagePicker imagePicker;
        readonly IImageUploader imageUploader;

        const string AvatarBucket = "avatars";

        User user;
        UserProfile profile;
        UserLocation location;

        // ── Read-only display ──
        public string Email => user?.Email ?? "";
        public string Role => user != null ? user.Role.ToString() : "";
        public bool IsLoaded { get; private set; }

        /// <summary>Current profile-picture URL (empty until one is uploaded).</summary>
        public string AvatarUrl => profile?.ProfileImageUrl ?? "";

        // ── Editable account ──
        public string DisplayName { get; set; } = "";
        public string Username { get; set; } = "";

        // ── Editable profile ──
        public string Bio { get; set; } = "";
        public string Country { get; set; } = "";
        public string Timezone { get; set; } = "";

        // ── Editable location ──
        public string LocationCountry { get; set; } = "";
        public string Province { get; set; } = "";
        public string City { get; set; } = "";
        public string Address { get; set; } = "";

        public string StatusMessage { get; private set; } = "";

        /// <param name="users">Null when no AppCompositionRoot exists yet — the screen then shows placeholder data.</param>
        public ProfileViewModel(IUserService users = null, ILocationService locations = null, System.Func<Task> signOut = null,
            IImagePicker imagePicker = null, IImageUploader imageUploader = null)
        {
            this.users = users;
            this.locations = locations;
            this.signOut = signOut;
            this.imagePicker = imagePicker;
            this.imageUploader = imageUploader;
        }

        /// <summary>
        /// Picks a new profile picture, uploads it, and saves the URL on the
        /// user's profile so it's fetched everywhere next time. Returns the URL.
        /// </summary>
        public async Task<Result<string>> ChangeProfilePictureAsync()
        {
            if (imagePicker == null || imageUploader == null || users == null)
                return Result.Fail<string>("Photo upload isn't available here.");
            if (user == null)
                return Result.Fail<string>("Profile is still loading — try again in a moment.");

            var picked = await imagePicker.PickImageAsync();
            if (picked.Cancelled) return Result.Fail<string>("");
            if (!picked.Success) { ErrorMessage = picked.Error; return Result.Fail<string>(picked.Error); }

            IsBusy = true;
            var upload = await imageUploader.UploadAsync(AvatarBucket, picked.Data, picked.Extension, "avatar");
            if (upload.IsFailure) { IsBusy = false; ErrorMessage = upload.Error; return upload; }

            profile = profile ?? new UserProfile { UserId = user.Id };
            profile.UserId = user.Id;
            profile.ProfileImageUrl = upload.Value;
            var save = await users.UpdateProfileAsync(profile);
            IsBusy = false;

            if (save.IsFailure) { ErrorMessage = save.Error; return Result.Fail<string>(save.Error); }
            Raise(nameof(AvatarUrl));
            return upload;
        }

        public async Task<Result> LoadAsync()
        {
            if (users == null)
            {
                // Placeholder mode: show sample data so the layout is visible
                // in-editor before services are wired.
                DisplayName = "Reader";
                Bio = "Tell others a little about yourself.";
                IsLoaded = true;
                Raise(nameof(IsLoaded));
                return PlaceholderOk();
            }

            IsBusy = true;
            StatusMessage = "";

            var userResult = await users.GetCurrentUserAsync();
            if (userResult.IsFailure)
            {
                IsBusy = false;
                ErrorMessage = userResult.Error;
                return Result.Fail(userResult.Error);
            }

            user = userResult.Value;
            DisplayName = user.DisplayName ?? "";
            Username = user.Username ?? "";

            var profileResult = await users.GetProfileAsync(user.Id);
            profile = profileResult.IsSuccess ? profileResult.Value : new UserProfile { UserId = user.Id };
            Bio = profile.Bio ?? "";
            Country = profile.Country ?? "";
            Timezone = profile.Timezone ?? "";

            if (locations != null)
            {
                var locResult = await locations.GetMyLocationAsync();
                location = locResult.IsSuccess ? locResult.Value : new UserLocation { UserId = user.Id };
                LocationCountry = location.Country ?? "";
                Province = location.Province ?? "";
                City = location.City ?? "";
                Address = location.Address ?? "";
            }

            IsBusy = false;
            IsLoaded = true;
            ErrorMessage = "";
            Raise(nameof(IsLoaded));
            return Result.Ok();
        }

        public async Task<Result> SaveAsync()
        {
            if (users == null)
            {
                StatusMessage = "Saved (placeholder mode).";
                Raise(nameof(StatusMessage));
                return PlaceholderOk();
            }
            if (user == null)
                return Fail("Profile is still loading — try again in a moment.");

            IsBusy = true;
            StatusMessage = "";
            ErrorMessage = "";

            // 1 · Account (display name / username)
            user.DisplayName = DisplayName?.Trim();
            user.Username = Username?.Trim();
            var userResult = await users.UpdateUserAsync(user);
            if (userResult.IsFailure) return Done(Result.Fail(userResult.Error), userResult.Error);

            // 2 · Profile
            profile = profile ?? new UserProfile { UserId = user.Id };
            profile.UserId = user.Id;
            profile.Bio = Bio?.Trim();
            profile.Country = Country?.Trim();
            profile.Timezone = Timezone?.Trim();
            var profileResult = await users.UpdateProfileAsync(profile);
            if (profileResult.IsFailure) return Done(Result.Fail(profileResult.Error), profileResult.Error);

            // 3 · Location
            if (locations != null)
            {
                location = location ?? new UserLocation { UserId = user.Id };
                location.Country = LocationCountry?.Trim();
                location.Province = Province?.Trim();
                location.City = City?.Trim();
                location.Address = Address?.Trim();
                var locResult = await locations.UpdateMyLocationAsync(location);
                if (locResult.IsFailure) return Done(Result.Fail(locResult.Error), locResult.Error);
            }

            StatusMessage = "Profile synced.";
            Raise(nameof(StatusMessage));
            return Done(Result.Ok(), "");
        }

        public async Task SignOutAsync()
        {
            if (signOut != null) await signOut();
        }

        Result Done(Result result, string error)
        {
            IsBusy = false;
            ErrorMessage = error;
            return result;
        }

        Result Fail(string message)
        {
            ErrorMessage = message;
            return Result.Fail(message);
        }
    }
}
