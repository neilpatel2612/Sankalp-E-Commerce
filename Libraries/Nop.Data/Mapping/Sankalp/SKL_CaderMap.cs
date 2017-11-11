using Nop.Core.Domain.Sankalp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Sankalp
{
    public partial class SKL_CaderMap : NopEntityTypeConfiguration<SKL_Cader>
    {
        public SKL_CaderMap()
        {
            this.ToTable("SKL_Cader");
            this.HasKey(c => c.Id);
        }
    }
}
