using Nuntius.Core.Common;
using Nuntius.Core.Messages;
using Resulz;

namespace Nuntius.Core.Delivery;

public partial class Sender : Element<string>
{
	#region Properties
	public virtual string Name { get; protected set; } = string.Empty;
	public virtual string EngineName { get; protected set; } = string.Empty;
	public virtual string Settings { get; protected set; } = string.Empty;
	public virtual MessageType MessageType { get; protected set; } = MessageType.Text;
	#endregion

	#region Ctor
	protected Sender() : base() { }

	protected Sender(string id, string name, string engineName, string settings, MessageType messageType) : base(id)
		=> (Name, EngineName, Settings, MessageType) = (name.ToUpper(), engineName.ToUpper(), settings, messageType);
	#endregion

	#region Factory Method
	public static OperationResult<Sender> Create(string id, string name, string engineName, string settings, MessageType messageType)
		=> Validate(id, name, engineName, settings, messageType)
			.IfSuccessThenReturn(() => new Sender(id, name, engineName, settings, messageType));
	#endregion

	#region Methods
	public virtual OperationResult Update(string name, string engineName, string settings)
		=> Validate(this.Id!, name, engineName, settings, this.MessageType)
			.IfSuccess(res => (Name, EngineName, Settings) = (name.ToUpper(), engineName.ToUpper(), settings));
	#endregion
}
