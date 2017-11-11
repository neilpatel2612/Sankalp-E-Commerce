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
    public class ColorCodeController : BaseAdminController
    {
        #region Fields

        private readonly ISKL_ColorCodeService _colorCodeService;
        private readonly ILanguageService _languageService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Constructors

        public ColorCodeController(ISKL_ColorCodeService colorCodeService,
            ILanguageService languageService,
            IPermissionService permissionService,
            IWorkContext workContext,
            ICacheManager cacheManager,
            ILocalizationService localizationService)
        {
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
            var model = new SKL_ColorCodeListModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, SKL_ColorCodeListModel model)
        {
            var colorCode = _colorCodeService.GetAllColorCode(model.SearchColorCodeName,
                command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = colorCode.Select(x =>
                {
                    var colorCodeModel = x.ToModel();
                    return colorCodeModel;
                }),
                Total = colorCode.TotalCount
            };
            return Json(gridModel);
        }
        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            var model = new SKL_ColorCodeModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(SKL_ColorCodeModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var colorCode = model.ToEntity();
                _colorCodeService.InsertColorCode(colorCode);

                SuccessNotification(_localizationService.GetResource("Admin.Sankalp.ColorCode.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();
                    return RedirectToAction("Edit", new { id = colorCode.Id });
                }
                return RedirectToAction("List");
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var colorCode = _colorCodeService.GetColorCodeById(id);
            if (colorCode == null)
                return RedirectToAction("List");

            var model = colorCode.ToModel();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(SKL_ColorCodeModel model, bool continueEditing)
        {
            var colorCode = _colorCodeService.GetColorCodeById(model.Id);
            if (colorCode == null)
                //No colorCode found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                colorCode = model.ToEntity(colorCode);
                _colorCodeService.UpdateColorCode(colorCode);

                SuccessNotification(_localizationService.GetResource("Admin.Sankalp.ColorCode.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();
                    return RedirectToAction("Edit", new { id = colorCode.Id });
                }
                return RedirectToAction("List");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var colorCode = _colorCodeService.GetColorCodeById(id);
            if (colorCode == null)
                //No colorCode found with the specified id
                return RedirectToAction("List");

            _colorCodeService.DeleteColorCode(colorCode);

            SuccessNotification(_localizationService.GetResource("Admin.Sankalp.ColorCode.Deleted"));
            return RedirectToAction("List");
        }
        #endregion
    }
}