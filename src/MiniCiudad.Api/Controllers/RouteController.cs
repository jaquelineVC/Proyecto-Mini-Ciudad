using Microsoft.AspNetCore.Mvc;
using MiniCiudad.Application.DTOs;
using MiniCiudad.Application.UseCases;
using MiniCiudad.Domain.Interfaces;

namespace MiniCiudad.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly CalculateRoutesUseCase _calculateRoutesUseCase;

    public RouteController(CalculateRoutesUseCase calculateRoutesUseCase)
    {
        _calculateRoutesUseCase = calculateRoutesUseCase;
    }

    [HttpPost("calculate")]
    [ProducesResponseType(typeof(IEnumerable<RouteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Calculate([FromBody] RouteRequestDto request)
    {
        if (string.IsNullOrEmpty(request.FromNodeId) || string.IsNullOrEmpty(request.ToNodeId))
            return BadRequest("Se requiere origen y destino.");

        if (request.FromNodeId == request.ToNodeId)
            return BadRequest("El origen y destino no pueden ser iguales.");

        var routes = _calculateRoutesUseCase.Execute(request);

        if (!routes.Any())
            return NotFound("No se encontró ninguna ruta entre los nodos indicados.");

        return Ok(routes);
    }

    [HttpGet("nodes")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetNodes([FromServices] IGraphRepository graphRepository)
    {
        var nodes = graphRepository.GetAllNodes().Select(n => new { n.Id, n.Row, n.Col });
        return Ok(nodes);
    }

    [HttpGet("edges")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetEdges([FromServices] IGraphRepository graphRepository)
    {
        var edges = graphRepository.GetAllEdges().Select(e => new
        {
            e.FromNodeId,
            e.ToNodeId,
            e.Weight,
            e.IsBlocked
        });
        return Ok(edges);
    }
}