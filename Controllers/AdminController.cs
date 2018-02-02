using Moov2.Orchard.FindReplace.Models;
using Moov2.Orchard.FindReplace.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;
using System.Linq;
using System.Web.Mvc;

namespace Moov2.Orchard.FindReplace.Controllers {
    [OrchardFeature("Moov2.Orchard.FindReplace")]
    public class AdminController : Controller {
        #region Dependencies

        public Localizer T { get; set; }

        private readonly IContentManager _contentManager;
        private readonly IFindReplaceService _findReplaceService;
        private readonly IOrchardServices _orchardServices;

        dynamic Shape { get; set; }

        #endregion

        #region Constructor

        public AdminController(IContentManager contentManager, IFindReplaceService findReplaceService, IOrchardServices orchardServices, IShapeFactory shapeFactory) {
            _contentManager = contentManager;
            _findReplaceService = findReplaceService;
            _orchardServices = orchardServices;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        #endregion

        #region Actions

        public ActionResult Index() {
            return View();
        }

        public ActionResult Preview(string find)
        {
            var contentItems = _findReplaceService.GetContentItems(find).OrderBy(x => x.As<ITitleAspect>().Title);

            var model = new PreviewModel
            {
                Find = find,
                ContentItems = contentItems,
                Display = Shape.List()
            };
            
            model.Display.AddRange(contentItems.Select(ci => _contentManager.BuildDisplay(ci, "SummaryAdmin")));

            return View(model);
        }

        [HttpPost]
        public ActionResult Replace(ReplaceModel model) {
            _findReplaceService.Replace(model.ItemIds, model.Find, model.Replace);

            _orchardServices.Notifier.Add(NotifyType.Information, T("Successfully updated {0} content item(s).", model.ItemIds.Count));

            return RedirectToAction("Index");
        }

        #endregion
    }
}