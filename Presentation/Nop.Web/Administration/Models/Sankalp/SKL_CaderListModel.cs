using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Sankalp
{
    public partial class SKL_CaderListModel : BaseNopModel
    {
        public SKL_CaderListModel()
        {

        }
        [NopResourceDisplayName("Admin.Sankalp.Cader.List.SearchCaderName")]
        public string SearchCaderName { get; set; }
    }
}