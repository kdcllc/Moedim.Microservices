﻿using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Authentication;

public class UnauthorizedProblemDetails : ProblemDetails
{
    public UnauthorizedProblemDetails(string? details = null)
    {
        Title = "Unauthorized";
        Detail = details;
        Status = 401;
        Type = "https://httpstatuses.com/401";
    }
}
