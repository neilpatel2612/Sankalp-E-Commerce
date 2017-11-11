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
    [Validator(typeof(SKL_ColorCodeValidator))]
    public partial class SKL_ColorCodeModel : BaseNopEntityModel
    {
        public SKL_ColorCodeModel()
        {

        }
        [NopResourceDisplayName("Admin.Sankalp.ColorCode.Fields.Name")]
        public string ColorCode { get; set; }
    }
}