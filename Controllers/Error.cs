using Microsoft.AspNetCore.Mvc;

namespace Workout.Controllers;

public class ErrorsController : ControllerBase
{
    [HttpGet("/error")]
    private IActionResult Error()
    {
        return Problem();
    }
}