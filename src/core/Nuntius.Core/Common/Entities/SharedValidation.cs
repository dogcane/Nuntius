using Resulz.Validation;

namespace Nuntius.Core.Common.Entities;

public static class SharedValidation
{
    public static ValueChecker<string?> ValidId(this ValueChecker<string?> checker)
        => checker
            .Required()
            .StringLength(50)
            .StringMatch("^[a-zA-Z0-9_-]{1,50}$");

}
