using Microsoft.AspNetCore.Mvc;
using MiniCiudad.Application.DTOs;
using MiniCiudad.Application.UseCases;
using MiniCiudad.Domain.Interfaces;

namespace MiniCiudad.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentController : ControllerBase
{
    private readonly IncidentUseCase _incidentUseCase;
    private readonly IGraphRepository _graphRepository;

    public IncidentController(IncidentUseCase incidentUseCase, IGraphRepository graphRepository)
    {
        _incidentUseCase = incidentUseCase;
        _graphRepository = graphRepository;
    }

    [HttpPost]
    [ProducesResponseType(typeof(IncidentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ReportIncident([FromBody] IncidentRequestDto request)
    {
        try
        {
            if (request.Scope == "SingleSegment")
            {
                var edgeExists = _graphRepository.GetAllEdges()
                    .Any(e => e.FromNodeId == request.FromNodeId && e.ToNodeId == request.ToNodeId);

                if (!edgeExists)
                    return BadRequest($"No existe una calle directa de {request.FromNodeId} a {request.ToNodeId}. Verifica la dirección de la calle.");
            }
            else if (request.Scope == "FullStreet")
            {
                var streetExists = _graphRepository.GetAllStreetNames()
                    .Any(s => s == request.StreetName);

                if (!streetExists)
                    return BadRequest($"No existe la calle '{request.StreetName}'.");
            }

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

    [HttpGet("streets")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public IActionResult GetStreetNames()
    {
        var streets = _graphRepository.GetAllStreetNames();
        return Ok(streets);
    }
}