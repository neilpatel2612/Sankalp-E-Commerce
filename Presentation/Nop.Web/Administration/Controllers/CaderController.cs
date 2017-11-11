using Nop.Admin.Models.Sankalp;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Localization;
using Nop.Services.Sankalp;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    public class CaderController : BaseAdminController
    {
        #region Fields

        private readonly ISKL_CaderService _caderService;
        private readonly ISKL_ColorCodeService _colorCodeService;
        private readonly ILanguageService _languageService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Constructors

        public CaderController(ISKL_CaderService caderService,
            ISKL_ColorCodeService colorCodeService,
            ILanguageService languageService,
            IPermissionService permissionService,
            IWorkContext workContext,
            ICacheManager cacheManager,
            ILocalizationService localizationService)
        {
            this._caderService = caderService;
            this._colorCodeService = colorCodeService;
            this._languageService = languageService;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
        }
        #endregion
        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var model = new SKL_CaderListModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, SKL_CaderListModel model)
        {
            var cader = _caderService.GetAllCader(model.SearchCaderName,
                command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = cader.Select(x =>
                {
                    var caderModel = x.ToModel();
                    return caderModel;
                }),
                Total = cader.TotalCount
            };
            return Json(gridModel);
        }
        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            var model = new SKL_CaderModel();
            PrepareColorCodeList(model);
            return View(model);
        }
        
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(SKL_CaderModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var cader = model.ToEntity();
                _caderService.InsertCader(cader);

                SuccessNotification(_localizationService.GetResource("Admin.Sankalp.Cader.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();
                    return RedirectToAction("Edit", new { id = cader.Id });
                }
                return RedirectToAction("List");
            }
            
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var cader = _caderService.GetCaderById(id);
            if (cader == null)
                return RedirectToAction("List");

            var model = cader.ToModel();
            PrepareColorCodeList(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(SKL_CaderModel model, bool continueEditing)
        {
            var cader = _caderService.GetCaderById(model.Id);
            if (cader == null)
                //No cader found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                cader = model.ToEntity(cader);
                _caderService.UpdateCader(cader);
                
                SuccessNotification(_localizationService.GetResource("Admin.Sankalp.Cader.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();
                    return RedirectToAction("Edit", new { id = cader.Id });
                }
                return RedirectToAction("List");
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var cader = _caderService.GetCaderById(id);
            if (cader == null)
                //No cader found with the specified id
                return RedirectToAction("List");

            _caderService.DeleteCader(cader);

            SuccessNotification(_localizationService.GetResource("Admin.Sankalp.Cader.Deleted"));
            return RedirectToAction("List");
        }

        private void PrepareColorCodeList(SKL_CaderModel model)
        {
            var templates = _colorCodeService.GetAllColorCode();
            foreach (var template in templates)
            {
                model.AvailableColorCodes.Add(new SelectListItem
                {
                    Text = template.ColorCode,
                    Value = template.Id.ToString()
                });
            }
        }
        #endregion
    }
}