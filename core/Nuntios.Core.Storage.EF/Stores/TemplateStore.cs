using Nuntius.Core.Templates.Entities;
using Nuntius.Core.Templates.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class TemplateStore : EfStore<Template, string>, ITemplateStore
{
    public TemplateStore(NuntiusDbContext context) : base(context)
    {
    }
}