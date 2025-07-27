using Nuntius.Core.Common.Entities;
using Nuntius.Core.Messages;
using Resulz;

namespace Nuntius.Core.Delivery.Entities;

public partial class Sender : Element<string>
{
	#region Properties
	public virtual string Name { get; protected set; } = string.Empty;
	public virtual string EngineId { get; protected set; } = string.Empty;
	public virtual string Settings { get; protected set; } = string.Empty;
	public virtual MessageType MessageType { get; protected set; } = MessageType.Text;
	#endregion

	#region Ctor
	protected Sender() : base() { }

	protected Sender(string id, string name, string engineId, string settings, MessageType messageType) : base(id)
		=> (Name, EngineId, Settings, MessageType) = (name.ToUpper(), engineId.ToUpper(), settings, messageType);
	#endregion

	#region Factory Method
	public static OperationResult<Sender> Create(string id, string name, string engineId, string settings, MessageType messageType)
		=> Validate(id, name, engineId, settings, messageType)
			.IfSuccessThenReturn(() => new Sender(id, name, engineId, settings, messageType));
	#endregion

	#region Methods
	public virtual OperationResult Update(string name, string engineId, string settings)
		=> ValidateEnable()
			.Then(() => Validate(this.Id!, name, engineId, settings, this.MessageType))
			.IfSuccess(res => (Name, EngineId, Settings) = (name.ToUpper(), engineId.ToUpper(), settings));
	#endregion
}
