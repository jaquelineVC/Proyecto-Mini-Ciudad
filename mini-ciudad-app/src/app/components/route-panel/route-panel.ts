import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouteResponse } from '../../models/city.models';

@Component({
  selector: 'app-route-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './route-panel.html',
  styleUrl: './route-panel.css'
})
export class RoutePanelComponent {
  @Input() routes: RouteResponse[] = [];
  @Input() fromNodeId: string = '';
  @Input() toNodeId: string = '';

  getRouteColor(rank: number): string {
    return rank === 1 ? '#3b82f6' : '#f97316';
  }

  getRouteLabel(rank: number): string {
    return rank === 1 ? 'Ruta más rápida' : 'Ruta alternativa';
  }

  getRouteIcon(rank: number): string {
    return rank === 1 ? '🥇' : '🥈';
  }

  formatPath(nodeIds: string[]): string {
    return nodeIds.join(' → ');
  }
}