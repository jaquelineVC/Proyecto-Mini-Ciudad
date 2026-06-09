using Microsoft.AspNetCore.Mvc;
using MiniCiudad.Application.DTOs;
using MiniCiudad.Application.UseCases;

namespace MiniCiudad.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentController : ControllerBase
{
    private readonly IncidentUseCase _incidentUseCase;

    public IncidentController(IncidentUseCase incidentUseCase)
    {
        _incidentUseCase = incidentUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(IncidentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ReportIncident([FromBody] IncidentRequestDto request)
    {
        try
        {
            var result = _incidentUseCase.ReportIncident(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{incidentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult RemoveIncident(Guid incidentId)
    {
        _incidentUseCase.RemoveIncident(incidentId);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IncidentResponseDto>), StatusCodes.Status200OK)]
    public IActionResult GetActiveIncidents()
    {
        var incidents = _incidentUseCase.GetActiveIncidents();
        return Ok(incidents);
    }
}