export interface CityNode {
  id: string;
  row: number;
  col: number;
}

export interface CityEdge {
  fromNodeId: string;
  toNodeId: string;
  weight: number;
  isBlocked: boolean;
  streetName: string;
}

export interface RouteRequest {
  fromNodeId: string;
  toNodeId: string;
}

export interface RouteResponse {
  nodeIds: string[];
  totalCost: number;
  rank: number;
}

export interface IncidentRequest {
  type: 'RoadClosed' | 'TrafficJam';
  scope: 'SingleSegment' | 'FullStreet';
  description: string;
  fromNodeId?: string;
  toNodeId?: string;
  streetName?: string;
}

export interface IncidentResponse {
  id: string;
  type: string;
  scope: string;
  description: string;
  fromNodeId?: string;
  toNodeId?: string;
  streetName?: string;
  reportedAt: string;
}