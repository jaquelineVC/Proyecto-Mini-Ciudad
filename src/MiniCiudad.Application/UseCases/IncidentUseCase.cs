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

        if (!Enum.TryParse<IncidentScope>(request.Scope, out var incidentScope))
            throw new ArgumentException($"Alcance de incidencia inválido: {request.Scope}");

        if (incidentScope == IncidentScope.SingleSegment)
        {
            if (string.IsNullOrEmpty(request.FromNodeId) || string.IsNullOrEmpty(request.ToNodeId))
                throw new ArgumentException("Se requiere FromNodeId y ToNodeId para un tramo específico.");
        }
        else
        {
            if (string.IsNullOrEmpty(request.StreetName))
                throw new ArgumentException("Se requiere StreetName para una calle completa.");
        }

        var incident = new Incident(
            incidentType,
            incidentScope,
            request.Description,
            request.FromNodeId,
            request.ToNodeId,
            request.StreetName
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
        Type = incident.Type.ToString(),
        Scope = incident.Scope.ToString(),
        Description = incident.Description,
        FromNodeId = incident.FromNodeId,
        ToNodeId = incident.ToNodeId,
        StreetName = incident.StreetName,
        ReportedAt = incident.ReportedAt
    };
}