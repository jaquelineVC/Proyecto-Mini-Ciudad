using MiniCiudad.Application.DTOs;
using MiniCiudad.Domain.Entities;
using MiniCiudad.Domain.Interfaces;

namespace MiniCiudad.Application.UseCases;

public class IncidentUseCase
{
    private readonly IGraphRepository _graphRepository;

    public IncidentUseCase(IGraphRepository graphRepository)
    {
        _graphRepository = graphRepository;
    }

    public IncidentResponseDto ReportIncident(IncidentRequestDto request)
    {
        if (!Enum.TryParse<IncidentType>(request.Type, out var incidentType))
            throw new ArgumentException($"Tipo de incidencia inválido: {request.Type}");

        var incident = new Incident(
            request.FromNodeId,
            request.ToNodeId,
            incidentType,
            request.Description
        );

        _graphRepository.ApplyIncident(incident);

        return MapToDto(incident);
    }

    public void RemoveIncident(Guid incidentId)
    {
        _graphRepository.RemoveIncident(incidentId);
    }

    public IEnumerable<IncidentResponseDto> GetActiveIncidents()
    {
        return _graphRepository.GetActiveIncidents().Select(MapToDto);
    }

    private static IncidentResponseDto MapToDto(Incident incident) => new()
    {
        Id = incident.Id,
        FromNodeId = incident.FromNodeId,
        ToNodeId = incident.ToNodeId,
        Type = incident.Type.ToString(),
        Description = incident.Description,
        ReportedAt = incident.ReportedAt
    };
}