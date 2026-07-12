using Microsoft.AspNetCore.Mvc;
using SmartWork.Shared.Results;

namespace SmartWork.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, Func<T, IActionResult> onSuccess)
    {
        if (result.Succeeded) return onSuccess(result.Value!);
        return new BadRequestObjectResult(new { errors = result.Errors });
    }

    public static IActionResult ToActionResult(this Result result)
    {
        return result.Succeeded ? new OkResult() : new BadRequestObjectResult(new { errors = result.Errors });
    }
}
