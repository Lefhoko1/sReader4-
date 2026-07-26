using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Bindings;
using SReader.UI.ViewModels.Home;

namespace SReader.UI.Views.Home
{
    /// <summary>View for GuardianHomeReal.uxml.</summary>
    public sealed class GuardianHomeController : RoleHomeController<GuardianHomeViewModel>
    {
        protected override GuardianHomeViewModel CreatePlaceholderViewModel() => new GuardianHomeViewModel();

        protected override void BindRoleActions(VisualElement root)
        {
            UIPageBinder.Bind(root, "my-students-button",  () => Debug.Log("My students clicked"),  this);
            UIPageBinder.Bind(root, "progress-button",     () => Debug.Log("Progress clicked"),     this);
            UIPageBinder.Bind(root, "link-student-button", () => Debug.Log("Link a student clicked"), this);
        }
    }
}
