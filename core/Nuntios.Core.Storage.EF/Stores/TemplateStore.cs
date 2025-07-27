using Nuntius.Core.Templates.Entities;
using Nuntius.Core.Templates.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class TemplateStore(NuntiusDbContext context) : EfStore<Template, string>(context), ITemplateStore
{
}