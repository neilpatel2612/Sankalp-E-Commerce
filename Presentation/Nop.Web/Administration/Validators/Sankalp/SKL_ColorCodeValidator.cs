using Nop.Admin.Models.Sankalp;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Sankalp
{
    public partial class SKL_ColorCodeValidator : BaseNopValidator<SKL_ColorCodeModel>
    {
        public SKL_ColorCodeValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            //RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
            //RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems"));

            //SetStringPropertiesMaxLength<Category>(dbContext);
        }
    }
}