
using System.Collections.Generic;
using UnityEngine;

public class GraphPathTester : MonoBehaviour
{
    public WaypointGraph graph;
    public Transform testFrom;
    public Transform testTo;

    private void Start()
    {
        RunTest();
    }

    public void RunTest()
    {
        if (graph == null || testFrom == null || testTo == null)
        {
            Debug.LogError("GraphPathTester: Eksik referans var!");
            return;
        }

        graph.BuildGraph();

        Waypoint start = graph.GetNearestWaypoint(testFrom.position);
        Waypoint goal = graph.GetNearestWaypoint(testTo.position);

        if (start == null || goal == null)
        {
            Debug.LogError("Baslangic veya hedef waypoint bulunamadi!");
            return;
        }

        int astarExpanded;
        int dijkstraExpanded;

        List<Waypoint> astarPath = GraphAStar.FindPath(
            start, goal, true, out astarExpanded
        );

        List<Waypoint> dijkstraPath = GraphAStar.FindPath(
            start, goal, false, out dijkstraExpanded
        );

        int edgeCount = 0;

        foreach (Waypoint waypoint in graph.waypoints)
        {
            edgeCount += waypoint.neighbors.Count;
        }

        edgeCount /= 2;

        float pathLength = 0f;

        if (astarPath.Count > 0)
        {
            pathLength += Vector3.Distance(
                testFrom.position,
                astarPath[0].transform.position
            );

            for (int i = 0; i < astarPath.Count - 1; i++)
            {
                pathLength += Vector3.Distance(
                    astarPath[i].transform.position,
                    astarPath[i + 1].transform.position
                );
            }

            pathLength += Vector3.Distance(
                astarPath[astarPath.Count - 1].transform.position,
                testTo.position
            );
        }

        string pathText = "";

        foreach (Waypoint waypoint in astarPath)
        {
            pathText += waypoint.name + " -> ";
        }

        Debug.Log("===== GRAPH TEST RESULTS =====");
        Debug.Log("Node Count: " + graph.waypoints.Count);
        Debug.Log("Edge Count: " + edgeCount);
        Debug.Log("A* Path: " + pathText);
        Debug.Log("A* Expanded: " + astarExpanded);
        Debug.Log("Dijkstra Expanded: " + dijkstraExpanded);
        Debug.Log("A* Path Length: " + pathLength.ToString("F2") + " units");
    }
}

