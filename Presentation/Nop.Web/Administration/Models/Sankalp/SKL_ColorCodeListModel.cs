using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Sankalp
{
    public partial class SKL_ColorCodeListModel : BaseNopModel
    {
        public SKL_ColorCodeListModel()
        {

        }
        [NopResourceDisplayName("Admin.Sankalp.ColorCode.List.SearchColorCodeName")]
        public string SearchColorCodeName { get; set; }
    }
}