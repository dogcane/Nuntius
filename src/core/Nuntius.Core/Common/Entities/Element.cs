using Resulz;

namespace Nuntius.Core.Common.Entities;

public abstract partial class Element<TKey> : Entity<TKey>
    where TKey : notnull
{
    #region Properties
    public virtual ElementStatus Status { get; protected set; } = ElementStatus.Enabled;
    #endregion

    #region Ctor
    protected Element() : base() { }

    protected Element(TKey id) : base(id) { }
    #endregion

    #region Methods

    public virtual OperationResult Enable()
        => ValidateStatus(Status, ElementStatus.Enabled)
            .IfSuccess(_ => Status = ElementStatus.Enabled);

    public virtual OperationResult Disable()
        => ValidateStatus(Status, ElementStatus.Disabled)
            .IfSuccess(_ => Status = ElementStatus.Disabled);

    public virtual OperationResult Archive()
        => ValidateStatus(Status, ElementStatus.Archived)
            .IfSuccess(_ => Status = ElementStatus.Archived);
    #endregion
}