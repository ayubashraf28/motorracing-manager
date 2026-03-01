using System;
using System.Collections.Generic;

namespace MotorracingManager.Domain.Definitions
{
    public sealed class ValidationResult
    {
        public ValidationResult(IReadOnlyList<string> errors)
        {
            Errors = errors == null ? Array.Empty<string>() : Guard.AgainstNullOrCopy(errors, nameof(errors));
        }

        public bool IsValid => Errors.Count == 0;

        public IReadOnlyList<string> Errors { get; }
    }
}
