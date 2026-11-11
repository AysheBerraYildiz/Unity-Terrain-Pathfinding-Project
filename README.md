# Unity Terrain Pathfinding Project

## Project Description
A Unity-based Terrain maze project developed for pathfinding and game AI assignments.

## Week 4
- Terrain Maze
- NavMesh Navigation
- Grid A* Pathfinding
- Dijkstra Algorithm
- Path Visualization
- Path Following
- Dynamic Replanning

## Week 4 Test Results

| Measurement | Result |
|---|---:|
| Grid Nodes | 400 |
| A* Expanded | 71 |
| Dijkstra Expanded | 326 |
| Path Length | 19 m |

## Week 5
- Waypoint Graph
- Graph-based A*
- Dijkstra Pathfinding
- NPC Patrol
- Player Detection
- NPC Chase
- Return to Patrol Route

## Week 5 Test Results

| Measurement | 9 Waypoints | 13 Waypoints |
|---|---:|---:|
| Node Count | 9 | 13 |
| Edge Count | 9 | 21 |
| A* Expanded | 8 | 3 |
| Dijkstra Expanded | 9 | 5 |
| Path Length | 38.9 m | 102.25 units |

## Assignment Comment

Adding K1-K4 increased the waypoint count from 9 to 13 and the edge count from 9 to 21. In the 13-waypoint graph, A* expanded 3 nodes while Dijkstra expanded 5 nodes. However, the measured path length increased from 38.9 m to 102.25 units, so the additional corner waypoints did not shorten the path in this test.

## Technologies
- Unity
- C#
- NavMesh
- A* Pathfinding
- Dijkstra Algorithm
- Waypoint Graph