using Nuntius.Core.Common;
using Nuntius.Core.Delivery;
using Nuntius.Core.Fetching;
using Nuntius.Core.Templates.Entities;
using Resulz;
using Resulz.Validation;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Nuntius.Core.Messages;

public class Message : Entity<int>
{
    #region Properties
    public virtual string? BuilderCode { get; protected set; } = null;
    public virtual string? TemplateCode { get; protected set; } = null;
    public virtual string SenderCode { get; protected set; } = string.Empty;
    public virtual string From { get; protected set; } = string.Empty;
    public virtual MessageRecipients Recipients { get; protected set; } = new();
    public virtual string Payload { get; protected set; } = string.Empty;
    public virtual JsonElement PayloadHeader => JsonDocument.Parse(Payload).RootElement.GetProperty("header");
    public virtual JsonElement PayloadBody => JsonDocument.Parse(Payload).RootElement.GetProperty("body");
    public virtual RenderedMessage? RenderedMessage { get; protected set; } = null;
    public virtual MessageType MessageType { get; protected set; } = MessageType.Email;
    public virtual MessageStatus Status { get; protected set; } = MessageStatus.New;
    public virtual DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;
    public virtual DateTime? CompiledOn { get; protected set; }
    public virtual DateTime? GeneratedOn { get; protected set; }
    public virtual DateTime? SentOn { get; protected set; }
    public virtual int Retries { get; protected set; } = 0;
    #endregion

    #region Ctor
    protected Message() : base() { }
    protected Message(string? builderCode, string? templateCode, string senderCode, string from, MessageRecipients recipients, MessageType messageType)
        : this()
    {
        BuilderCode = builderCode;
        TemplateCode = templateCode;
        SenderCode = senderCode;
        From = from;
        Recipients = recipients;
        MessageType = messageType;
        Status = MessageStatus.New;
        CreatedOn = DateTime.UtcNow;
    }
    #endregion

    #region Factory Method
    public static OperationResult<Message> Create(DataFetcher builder, Template template, Sender sender, string from, MessageRecipients recipients, MessageType messageType, string headerPayload)
        => OperationResult
            .MakeSuccess()
            .With(builder, nameof(builder)).ValidateModel()
            .With(template, nameof(template)).ValidateTemplate()
            .With(sender, nameof(sender)).ValidateSender()
            .With(from, nameof(from)).ValidateFrom()
            .With(recipients, nameof(recipients)).ValidateRecipients()            
            .With(headerPayload, nameof(headerPayload)).ValidatePayload()
            .Result
            .IfSuccessThenReturn(() => {
                var message = new Message(builder.Id, template.Id, sender.Id, from, recipients, messageType);
                var jsonPayload = JsonNode.Parse("{}")!;
                jsonPayload["mode"] = nameof(MessageCreationMode.FromBuilder);
                jsonPayload["header"] = JsonNode.Parse(headerPayload);
                message.Payload = jsonPayload.ToString();
                return message;
            });

    public static OperationResult<Message> CreateAfterBuild(Template template, Sender sender, string from, MessageRecipients recipients, MessageType messageType, string bodyPayload)
        => OperationResult
            .MakeSuccess()
            .With(template, nameof(template)).ValidateTemplate()
            .With(sender, nameof(sender)).ValidateSender()
            .With(from, nameof(from)).ValidateFrom()
            .With(recipients, nameof(recipients)).ValidateRecipients()
            .With(bodyPayload, nameof(bodyPayload)).ValidatePayload()
            .Result
            .IfSuccessThenReturn(() =>
            {
                var message = new Message(null, template.Id, sender.Id, from, recipients, messageType);
                var jsonPayload = JsonNode.Parse("{}")!;
                jsonPayload["mode"] = nameof(MessageCreationMode.FromTemplate);
                jsonPayload["header"] = JsonNode.Parse("{}");
                jsonPayload["body"] = JsonNode.Parse(bodyPayload);
                message.Payload = jsonPayload.ToString();
                message.Status = MessageStatus.Built;
                message.CompiledOn = DateTime.UtcNow;
                return message;
            });

    public static OperationResult<Message> CreateAfterRender(Sender sender, string from, MessageRecipients recipients, MessageType messageType, RenderedMessage renderedMessage)
        => OperationResult
            .MakeSuccess()
            .With(sender, nameof(sender)).ValidateSender()
            .With(from, nameof(from)).ValidateFrom()
            .With(recipients, nameof(recipients)).ValidateRecipients()
            .With(renderedMessage, nameof(renderedMessage)).ValidateGeneratedMessage()
            .Result
            .IfSuccessThenReturn(() =>
            {
                var message = new Message(null, null, sender.Id, from, recipients, messageType);
                var jsonPayload = JsonNode.Parse("{}")!;
                jsonPayload["mode"] = nameof(MessageCreationMode.FromRenderedMessage);
                jsonPayload["header"] = JsonNode.Parse("{}");
                jsonPayload["body"] = JsonNode.Parse("{}");
                message.Payload = jsonPayload.ToString();
                message.Status = MessageStatus.Rendered;
                message.RenderedMessage = renderedMessage;
                message.CompiledOn = DateTime.UtcNow;
                message.GeneratedOn = DateTime.UtcNow;
                return message;
            });

    #endregion

    #region Methods
    public virtual OperationResult SetAsBuilt(string bodyPayload)
        => OperationResult
            .MakeSuccess()
            .With(bodyPayload, nameof(bodyPayload)).ValidatePayload()
            .With(Status, nameof(Status)).Condition(x => x == MessageStatus.New, "MESSAGE_ALREADY_COMPILED")
            .Result
            .IfSuccess(res => {
                var jsonPayload = JsonNode.Parse(Payload)!;
                jsonPayload["body"] = JsonNode.Parse(bodyPayload);
                Payload = jsonPayload.ToString();
                Status = MessageStatus.Built;
                CompiledOn = DateTime.UtcNow;
            });

    public virtual OperationResult SetAsRendered(RenderedMessage renderedMessage)
        => OperationResult
            .MakeSuccess()
            .With(renderedMessage, nameof(renderedMessage)).ValidateGeneratedMessage()
            .With(Status, nameof(Status)).Condition(x => x == MessageStatus.Built, "MESSAGE_NOT_COMPILED")
            .Result
            .IfSuccess(res =>
            {
                RenderedMessage = renderedMessage;
                Status = MessageStatus.Rendered;
                GeneratedOn = DateTime.UtcNow;
            });


    public virtual OperationResult MarkRetryFailure()
        => OperationResult
            .MakeSuccess()
            .With(Status, nameof(Status)).Into([MessageStatus.Rendered, MessageStatus.Fault], "MESSAGE_NOT_SENDABLE")
            .With(Retries, nameof(Retries)).LessThenOrEqual(3, "MESSAGE_RETRIES_EXCEEDED")
            .Result
            .IfSuccess(res =>
            {
                Status = MessageStatus.Fault;
                Retries++;
            });

    public virtual OperationResult SetAsSent()
        => OperationResult
            .MakeSuccess()
            .With(Status, nameof(Status)).Into([MessageStatus.Rendered, MessageStatus.Fault], "MESSAGE_NOT_SENDABLE")
            .Result
            .IfSuccess(res =>
            {
                Retries = 0;
                Status = MessageStatus.Sent;
                SentOn = DateTime.UtcNow;
            });

    #endregion
}
