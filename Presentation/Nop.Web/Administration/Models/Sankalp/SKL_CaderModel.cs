using FluentValidation.Attributes;
using Nop.Admin.Validators.Sankalp;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Models.Sankalp
{
    [Validator(typeof(SKL_CaderValidator))]
    public partial class SKL_CaderModel : BaseNopEntityModel
    {
        public SKL_CaderModel()
        {
            AvailableColorCodes = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Sankalp.Cader.Fields.Name")]
        public string Cader { get; set; }
        [NopResourceDisplayName("Admin.Sankalp.ColorCode.Fields.Name")]
        public int ColorCodeId { get; set; }
        public IList<SelectListItem> AvailableColorCodes { get; set; }
    }
}