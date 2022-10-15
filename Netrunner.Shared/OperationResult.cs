using System.Collections.Generic;
using System.Linq;

namespace Netrunner.Shared;

public class OperationResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<Error>? Errors { get; init; }

    public static OperationResult Create(bool succeeded, params string[] errors)
    {
        return new()
        {
            Succeeded = succeeded,
            Errors = errors.Select(s => new Error { Description = s })
        };
    }
}