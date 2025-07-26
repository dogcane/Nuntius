namespace Nuntius.Core.Templates.Entities;

public record TemplateContext(string Language, string? Scope = null)
{
    public TemplateContext(): this(string.Empty, null) { }    
}
