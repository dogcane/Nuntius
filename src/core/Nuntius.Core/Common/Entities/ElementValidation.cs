using Resulz;
using Resulz.Validation;

namespace Nuntius.Core.Common.Entities;

public abstract partial class Element<TKey> : Entity<TKey>
{
    protected OperationResult ValidateEnable()
        => OperationResult.MakeSuccess()
            .With(Status, nameof(Status)).EqualTo(ElementStatus.Enabled, "ELEMENT_NOT_ENABLED")
            .Result;

    protected static OperationResult ValidateStatus(ElementStatus currentStatus, ElementStatus newStatus)
        => OperationResult.MakeSuccess()
            .With(newStatus, nameof(newStatus)).Condition(val => val != currentStatus, "CANNOT_CHANGE_STATUS")
            .With(currentStatus, nameof(currentStatus)).Condition(val => val != ElementStatus.Archived, "CANNOT_CHANGE_FROM_ARCHIVED")
            .Result;
}