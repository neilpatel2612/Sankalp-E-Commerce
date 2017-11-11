using Nop.Core.Domain.Sankalp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Sankalp
{
    public partial class SKL_ColorCodeMap : NopEntityTypeConfiguration<SKL_ColorCode>
    {
        public SKL_ColorCodeMap()
        {
            this.ToTable("SKL_ColorCode");
            this.HasKey(c => c.Id);
        }
    }
}
