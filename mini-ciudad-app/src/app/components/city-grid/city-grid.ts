import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouteService } from '../../services/route';
import { CityNode, CityEdge, RouteResponse } from '../../models/city.models';

@Component({
  selector: 'app-city-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './city-grid.html',
  styleUrl: './city-grid.css'
})
export class CityGridComponent implements OnInit {
  @Output() nodeSelected = new EventEmitter<CityNode>();

  nodes: CityNode[] = [];
  edges: CityEdge[] = [];
  routes: RouteResponse[] = [];

  selectedFrom: CityNode | null = null;
  selectedTo: CityNode | null = null;

  readonly ROWS = 4;
  readonly COLS = 7;
  readonly CELL_SIZE = 100;
  readonly NODE_RADIUS = 18;

  constructor(private readonly routeService: RouteService) {}

  ngOnInit(): void {
    this.loadGraph();
  }

  loadGraph(): void {
    this.routeService.getNodes().subscribe(nodes => this.nodes = nodes);
    this.routeService.getEdges().subscribe(edges => this.edges = edges);
  }

  refreshEdges(): void {
    this.routeService.getEdges().subscribe(edges => this.edges = edges);
  }

  showRoutes(routes: RouteResponse[]): void {
    this.routes = routes;
  }

  clearRoutes(): void {
    this.routes = [];
    this.selectedFrom = null;
    this.selectedTo = null;
  }

  onNodeClick(node: CityNode): void {
    if (!this.selectedFrom) {
      this.selectedFrom = node;
    } else if (!this.selectedTo && node.id !== this.selectedFrom.id) {
      this.selectedTo = node;
      this.nodeSelected.emit(node);
    } else {
      this.selectedFrom = node;
      this.selectedTo = null;
      this.routes = [];
    }
  }

  getNodeX(col: number): number {
    return (col + 1) * this.CELL_SIZE;
  }

  getNodeY(row: number): number {
    return (row + 1) * this.CELL_SIZE;
  }

  getSvgWidth(): number {
    return (this.COLS + 1) * this.CELL_SIZE;
  }

  getSvgHeight(): number {
    return (this.ROWS + 1) * this.CELL_SIZE;
  }

  getNodeColor(node: CityNode): string {
    if (this.selectedFrom?.id === node.id) return '#22c55e';
    if (this.selectedTo?.id === node.id) return '#ef4444';
    if (this.isNodeInRoute(node.id, 0)) return '#3b82f6';
    if (this.isNodeInRoute(node.id, 1)) return '#f97316';
    return '#1e293b';
  }

  getEdgeColor(edge: CityEdge): string {
    if (edge.isBlocked) return '#ef4444';
    if (edge.weight > 1) return '#f97316';
    return '#94a3b8';
  }

  getEdgeWidth(edge: CityEdge): number {
    if (this.isEdgeInRoute(edge, 0)) return 5;
    if (this.isEdgeInRoute(edge, 1)) return 5;
    return 2;
  }

  getRouteEdgeColor(edge: CityEdge): string {
    if (this.isEdgeInRoute(edge, 0)) return '#3b82f6';
    if (this.isEdgeInRoute(edge, 1)) return '#f97316';
    return this.getEdgeColor(edge);
  }

  isNodeInRoute(nodeId: string, routeIndex: number): boolean {
    if (!this.routes[routeIndex]) return false;
    return this.routes[routeIndex].nodeIds.includes(nodeId);
  }

  isEdgeInRoute(edge: CityEdge, routeIndex: number): boolean {
    if (!this.routes[routeIndex]) return false;
    const nodeIds = this.routes[routeIndex].nodeIds;
    for (let i = 0; i < nodeIds.length - 1; i++) {
      if (nodeIds[i] === edge.fromNodeId && nodeIds[i + 1] === edge.toNodeId) return true;
    }
    return false;
  }

  getArrowPoints(edge: CityEdge): string {
    const from = this.nodes.find(n => n.id === edge.fromNodeId);
    const to = this.nodes.find(n => n.id === edge.toNodeId);
    if (!from || !to) return '';

    const x1 = this.getNodeX(from.col);
    const y1 = this.getNodeY(from.row);
    const x2 = this.getNodeX(to.col);
    const y2 = this.getNodeY(to.row);

    const dx = x2 - x1;
    const dy = y2 - y1;
    const len = Math.hypot(dx, dy);
    const ux = dx / len;
    const uy = dy / len;

    const startX = x1 + ux * this.NODE_RADIUS;
    const startY = y1 + uy * this.NODE_RADIUS;
    const endX = x2 - ux * this.NODE_RADIUS;
    const endY = y2 - uy * this.NODE_RADIUS;

    return `M${startX},${startY} L${endX},${endY}`;
  }

  getArrowMarkerId(edge: CityEdge): string {
    if (this.isEdgeInRoute(edge, 0)) return 'arrow-route1';
    if (this.isEdgeInRoute(edge, 1)) return 'arrow-route2';
    if (edge.isBlocked) return 'arrow-blocked';
    if (edge.weight > 1) return 'arrow-traffic';
    return 'arrow-default';
  }
}