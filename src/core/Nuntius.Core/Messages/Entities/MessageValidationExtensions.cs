using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Fetching.Entities;
using Nuntius.Core.Templates.Entities;
using Resulz.Validation;
using System.Text.Json;

namespace Nuntius.Core.Messages.Entities;

public static class MessageValidationExtensions
{
    public static ValueChecker<DataFetcher?> ValidateModel(this ValueChecker<DataFetcher?> checker) => checker.Required();
    public static ValueChecker<Template?> ValidateTemplate(this ValueChecker<Template?> checker) => checker.Required();
    public static ValueChecker<Sender?> ValidateSender(this ValueChecker<Sender?> checker) => checker.Required();
    public static ValueChecker<string?> ValidateFrom(this ValueChecker<string?> checker) => checker.Required().StringLength(150);    
    public static ValueChecker<MessageRecipients?> ValidateRecipients(this ValueChecker<MessageRecipients?> checker)
    {
        checker.Required();
        if (checker.Value != null)
        {
            checker
                .With(checker.Value.To, $"Recipients.{nameof(checker.Value.To)}").Required().StringLength(150)
                .With(checker.Value.Cc, $"Recipients.{nameof(checker.Value.Cc)}").Condition(x => x is not null && x!.All(xs => xs.Length <= 150), "CC_NOT_VALID")
                .With(checker.Value.Bcc, $"Recipients.{nameof(checker.Value.Bcc)}").Condition(x => x is not null && x!.All(xs => xs.Length <= 150), "BCC_NOT_VALID");
        }
        return checker;
    }
    public static ValueChecker<string?> ValidatePayload(this ValueChecker<string?> checker) => checker.Required().Condition(payload =>
    {
        try { JsonDocument.Parse(payload ?? ""); return true; }
        catch { return false;}
    }, "INVALID_PAYLOAD");
    public static ValueChecker<RenderedMessage?> ValidateGeneratedMessage(this ValueChecker<RenderedMessage?> checker)
    {
        checker.Required();
        if (checker.Value != null)
        {
            checker
                .With(checker.Value.Subject, $"GeneratedMessage.{nameof(checker.Value.Subject)}").Required().StringLength(100)
                .With(checker.Value.Content, $"GeneratedMessage.{nameof(checker.Value.Content)}").Required();
        }
        return checker;
    }

    //TODO VALIDAZIONE MITTENTE E RECIPIENTI IN BASE AL MESSAGGIO
}
