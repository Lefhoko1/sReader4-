using UnityEngine;
using SReader.Domains.Identity.Models;

namespace SReader.UI.Navigation
{
    public class NavigationManager : MonoBehaviour
    {
        [Header("UI Pages")]
        [SerializeField] private GameObject landingPage;
        [SerializeField] private GameObject loginPage;
        [SerializeField] private GameObject aboutPage;
        [SerializeField] private GameObject contactPage;
        [SerializeField] private GameObject registerPage;
        [SerializeField] private GameObject forgotPasswordPage;
        [SerializeField] private GameObject otpPage;
        [SerializeField] private GameObject resetPasswordPage;
        [SerializeField] private GameObject profilePage;

        [Header("Role-specific home pages (post-login landing)")]
        [SerializeField] private GameObject studentHomePage;
        [SerializeField] private GameObject guardianHomePage;
        [SerializeField] private GameObject tutorHomePage;

        [Header("Game")]
        [SerializeField] private GameObject trekPage;   // Story Trek scene wrapper (TrekScenePage)

        private void Start()
        {
            ShowLanding();
        }

        public void ShowLanding()        => ShowPage(landingPage, "Landing");
        public void ShowLogin()          => ShowPage(loginPage, "Login");
        public void ShowRegister()       => ShowPage(registerPage, "Register");
        public void ShowAbout()          => ShowPage(aboutPage, "About");
        public void ShowContact()        => ShowPage(contactPage, "Contact");
        public void ShowForgotPassword() => ShowPage(forgotPasswordPage, "Forgot Password");
        public void ShowOTP()            => ShowPage(otpPage, "OTP");
        public void ShowResetPassword()  => ShowPage(resetPasswordPage, "Reset Password");
        public void ShowProfile()        => ShowPage(profilePage, "Profile");
        public void ShowStudentHome()    => ShowPage(studentHomePage, "Student Home");
        public void ShowGuardianHome()   => ShowPage(guardianHomePage, "Guardian Home");
        public void ShowTutorHome()      => ShowPage(tutorHomePage, "Tutor Home");
        public void ShowTrek()           => ShowPage(trekPage, "Story Trek");

        // A page created in code rather than placed in the scene (e.g. the 3D
        // Reading House wrapper). It takes part in the same one-page-at-a-time
        // rule: showing it hides every other page, and showing any other page
        // hides it again (which lets it clean up in OnDisable).
        GameObject runtimePage;

        /// <summary>Show a page that was created at runtime (not assigned in the Inspector).</summary>
        public void ShowRuntimePage(GameObject page, string pageName)
        {
            if (page == null)
            {
                Debug.LogError($"[NavigationManager] Runtime page '{pageName}' is null — staying on the current page.", this);
                return;
            }
            SetAllPagesInactive();
            runtimePage = page;
            page.SetActive(true);
        }

        /// <summary>
        /// Post-login landing: each role lands on the home built for what they do.
        /// Teachers reuse the tutor home; administrators fall back to the profile
        /// until a dedicated admin console exists.
        /// </summary>
        public void ShowHomeForRole(UserRole role)
        {
            switch (role)
            {
                case UserRole.Guardian:      ShowGuardianHome(); break;
                case UserRole.Tutor:         ShowTutorHome();    break;
                case UserRole.Teacher:       ShowTutorHome();    break;
                case UserRole.Administrator: ShowProfile();      break;
                default:                     ShowStudentHome();  break;
            }
        }

        private void ShowPage(GameObject page, string pageName)
        {
            // Refuse to navigate to an unassigned page: hiding everything and
            // showing nothing looks like a dead button with no explanation.
            if (page == null)
            {
                Debug.LogError($"[NavigationManager] The '{pageName}' page is not assigned in the Inspector — staying on the current page.", this);
                return;
            }

            SetAllPagesInactive();
            page.SetActive(true);
        }

        private void SetAllPagesInactive()
        {
            SetPageActive(landingPage, false);
            SetPageActive(loginPage, false);
            SetPageActive(aboutPage, false);
            SetPageActive(contactPage, false);
            SetPageActive(registerPage, false);
            SetPageActive(forgotPasswordPage, false);
            SetPageActive(otpPage, false);
            SetPageActive(resetPasswordPage, false);
            SetPageActive(profilePage, false);
            SetPageActive(studentHomePage, false);
            SetPageActive(guardianHomePage, false);
            SetPageActive(tutorHomePage, false);
            SetPageActive(trekPage, false);
            SetPageActive(runtimePage, false);
        }

        private void SetPageActive(GameObject page, bool isActive)
        {
            if (page != null)
            {
                page.SetActive(isActive);
            }
        }
    }
}
