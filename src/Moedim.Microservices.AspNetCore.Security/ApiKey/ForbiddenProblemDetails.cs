using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Authentication;

public class ForbiddenProblemDetails : ProblemDetails
{
    public ForbiddenProblemDetails(string? details = null)
    {
        Title = "Forbidden";
        Detail = details;
        Status = 403;
        Type = "https://httpstatuses.com/403";
    }
}
