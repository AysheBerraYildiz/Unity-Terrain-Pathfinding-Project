
using System.Collections.Generic;
using UnityEngine;

public class WaypointGraph : MonoBehaviour
{
    public float maxEdgeLength = 8f;
    public LayerMask obstacleMask;

    public List<Waypoint> waypoints = new List<Waypoint>();

    private void Start()
    {
        BuildGraph();
    }

    public void BuildGraph()
    {
        waypoints.Clear();

        waypoints.AddRange(
            GetComponentsInChildren<Waypoint>(true)
        );

        foreach (Waypoint waypoint in waypoints)
        {
            waypoint.neighbors.Clear();
        }

        int edgeCount = 0;

        for (int i = 0; i < waypoints.Count; i++)
        {
            for (int j = i + 1; j < waypoints.Count; j++)
            {
                Waypoint a = waypoints[i];
                Waypoint b = waypoints[j];

                float distance = Vector3.Distance(
                    a.transform.position,
                    b.transform.position
                );

                if (distance > maxEdgeLength)
                    continue;

                bool blocked = Physics.Linecast(
                    a.transform.position,
                    b.transform.position,
                    obstacleMask
                );

                if (!blocked)
                {
                    a.neighbors.Add(b);
                    b.neighbors.Add(a);

                    edgeCount++;
                }
            }
        }

        Debug.Log("Waypoint Graph Created");
        Debug.Log("Node Count: " + waypoints.Count);
        Debug.Log("Edge Count: " + edgeCount);
    }

    public Waypoint GetNearestWaypoint(Vector3 position)
    {
        Waypoint nearest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Waypoint waypoint in waypoints)
        {
            float distance = Vector3.Distance(
                position,
                waypoint.transform.position
            );

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = waypoint;
            }
        }

        return nearest;
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null)
            return;

        Gizmos.color = Color.green;

        foreach (Waypoint waypoint in waypoints)
        {
            if (waypoint == null)
                continue;

            foreach (Waypoint neighbor in waypoint.neighbors)
            {
                if (neighbor != null)
                {
                    Gizmos.DrawLine(
                        waypoint.transform.position,
                        neighbor.transform.position
                    );
                }
            }
        }
    }
}
