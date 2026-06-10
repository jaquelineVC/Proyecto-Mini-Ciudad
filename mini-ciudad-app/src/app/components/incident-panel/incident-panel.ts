import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IncidentService } from '../../services/incident';
import { IncidentRequest, IncidentResponse, CityNode } from '../../models/city.models';

@Component({
  selector: 'app-incident-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './incident-panel.html',
  styleUrl: './incident-panel.css'
})
export class IncidentPanelComponent implements OnInit {
  @Output() incidentChanged = new EventEmitter<void>();

  incidents: IncidentResponse[] = [];
  nodes: CityNode[] = [];
  streetNames: string[] = [];

  scope: 'SingleSegment' | 'FullStreet' = 'SingleSegment';

  form: IncidentRequest = {
    type: 'RoadClosed',
    scope: 'SingleSegment',
    description: '',
    fromNodeId: '',
    toNodeId: '',
    streetName: ''
  };

  errorMessage = '';
  successMessage = '';

  constructor(private readonly incidentService: IncidentService) {}

  ngOnInit(): void {
    this.loadIncidents();
    this.loadStreetNames();
  }

  setNodes(nodes: CityNode[]): void {
    this.nodes = nodes;
  }

  loadIncidents(): void {
    this.incidentService.getActiveIncidents().subscribe(incidents => {
      this.incidents = incidents;
    });
  }

  loadStreetNames(): void {
    this.incidentService.getStreetNames().subscribe(names => {
      this.streetNames = names;
    });
  }

  onScopeChange(): void {
    this.form.scope = this.scope;
    this.form.fromNodeId = '';
    this.form.toNodeId = '';
    this.form.streetName = '';
    this.errorMessage = '';
    this.successMessage = '';
  }

  reportIncident(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.scope === 'SingleSegment') {
      if (!this.form.fromNodeId || !this.form.toNodeId) {
        this.errorMessage = 'Selecciona origen y destino del tramo.';
        return;
      }
      if (this.form.fromNodeId === this.form.toNodeId) {
        this.errorMessage = 'El origen y destino no pueden ser iguales.';
        return;
      }
    } else {
      if (!this.form.streetName) {
        this.errorMessage = 'Selecciona una calle o avenida.';
        return;
      }
    }

    if (!this.form.description.trim()) {
      this.errorMessage = 'Agrega una descripción del incidente.';
      return;
    }

    this.incidentService.reportIncident(this.form).subscribe({
      next: () => {
        this.successMessage = 'Incidencia reportada correctamente.';
        this.loadIncidents();
        this.incidentChanged.emit();
        this.resetForm();
      },
      error: (err) => {
        this.errorMessage = err.error ?? 'Error al reportar la incidencia.';
      }
    });
  }

  removeIncident(id: string): void {
    this.incidentService.removeIncident(id).subscribe({
      next: () => {
        this.loadIncidents();
        this.incidentChanged.emit();
      }
    });
  }

  resetForm(): void {
    this.form = {
      type: 'RoadClosed',
      scope: this.scope,
      description: '',
      fromNodeId: '',
      toNodeId: '',
      streetName: ''
    };
  }

  getIncidentIcon(type: string): string {
    return type === 'RoadClosed' ? '🚫' : '🚦';
  }

  getIncidentLabel(type: string): string {
    return type === 'RoadClosed' ? 'Calle cerrada' : 'Tráfico';
  }

  getScopeLabel(scope: string): string {
    return scope === 'FullStreet' ? 'Calle completa' : 'Tramo específico';
  }

  getIncidentLocation(incident: IncidentResponse): string {
    if (incident.scope === 'FullStreet') return incident.streetName ?? '';
    return `${incident.fromNodeId} → ${incident.toNodeId}`;
  }
}