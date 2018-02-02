using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Orchard.MediaLibrary {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public AdminMenu() {
            T = NullLocalizer.Instance;
        }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("Find & Replace"), "21", item => item.Add(T("Find & Replace"), "6", i => i.Action("Index", "Admin", new { area = "Moov2.Orchard.FindReplace" })
            .Permission(StandardPermissions.SiteOwner)));
        }
    }
}