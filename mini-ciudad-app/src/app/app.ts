import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CityGridComponent } from './components/city-grid/city-grid';
import { IncidentPanelComponent } from './components/incident-panel/incident-panel';
import { RoutePanelComponent } from './components/route-panel/route-panel';
import { RouteService } from './services/route';
import { CityNode, RouteResponse } from './models/city.models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, CityGridComponent, IncidentPanelComponent, RoutePanelComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit, AfterViewInit {
  @ViewChild('cityGrid') cityGrid!: CityGridComponent;
  @ViewChild('incidentPanel') incidentPanel!: IncidentPanelComponent;

  nodes: CityNode[] = [];
  routes: RouteResponse[] = [];
  fromNodeId = '';
  toNodeId = '';
  isCalculating = false;
  errorMessage = '';

  constructor(private readonly routeService: RouteService) {}

  ngOnInit(): void {
    this.routeService.getNodes().subscribe(nodes => {
      this.nodes = nodes;
    });
  }

  ngAfterViewInit(): void {
    this.routeService.getNodes().subscribe(nodes => {
      this.incidentPanel.setNodes(nodes);
    });
  }

  onNodeSelected(): void {
    if (!this.cityGrid.selectedFrom || !this.cityGrid.selectedTo) return;

    this.fromNodeId = this.cityGrid.selectedFrom.id;
    this.toNodeId = this.cityGrid.selectedTo.id;
    this.errorMessage = '';
    this.isCalculating = true;

    this.routeService.calculateRoutes({
      fromNodeId: this.fromNodeId,
      toNodeId: this.toNodeId
    }).subscribe({
      next: (routes) => {
        this.routes = routes;
        this.cityGrid.showRoutes(routes);
        this.isCalculating = false;
      },
      error: () => {
        this.errorMessage = 'No se encontró ruta entre los nodos seleccionados.';
        this.isCalculating = false;
      }
    });
  }

  onIncidentChanged(): void {
    this.cityGrid.refreshEdges();
    if (this.fromNodeId && this.toNodeId) {
      this.recalculateRoutes();
    }
  }

  recalculateRoutes(): void {
    this.routeService.calculateRoutes({
      fromNodeId: this.fromNodeId,
      toNodeId: this.toNodeId
    }).subscribe({
      next: (routes) => {
        this.routes = routes;
        this.cityGrid.showRoutes(routes);
      },
      error: () => {
        this.routes = [];
        this.cityGrid.clearRoutes();
        this.errorMessage = 'No hay ruta disponible con las incidencias actuales.';
      }
    });
  }

  clearAll(): void {
    this.routes = [];
    this.fromNodeId = '';
    this.toNodeId = '';
    this.errorMessage = '';
    this.cityGrid.clearRoutes();
  }
}