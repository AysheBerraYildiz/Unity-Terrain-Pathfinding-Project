
using System.Collections.Generic;
using UnityEngine;

public static class GraphAStar
{
    public static List<Waypoint> FindPath(
        Waypoint start,
        Waypoint goal,
        bool useHeuristic,
        out int expandedCount)
    {
        expandedCount = 0;

        List<Waypoint> openList = new List<Waypoint>();
        HashSet<Waypoint> closedList = new HashSet<Waypoint>();

        Dictionary<Waypoint, float> gScore =
            new Dictionary<Waypoint, float>();

        Dictionary<Waypoint, Waypoint> cameFrom =
            new Dictionary<Waypoint, Waypoint>();

        if (start == null || goal == null)
            return new List<Waypoint>();

        openList.Add(start);
        gScore[start] = 0f;

        while (openList.Count > 0)
        {
            Waypoint current = openList[0];

            float bestScore = Mathf.Infinity;

            foreach (Waypoint node in openList)
            {
                float g = gScore[node];

                float h = useHeuristic
                    ? Vector3.Distance(
                        node.transform.position,
                        goal.transform.position
                    )
                    : 0f;

                float f = g + h;

                if (f < bestScore)
                {
                    bestScore = f;
                    current = node;
                }
            }

            openList.Remove(current);

            if (closedList.Contains(current))
                continue;

            expandedCount++;
            closedList.Add(current);

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (Waypoint neighbor in current.neighbors)
            {
                if (closedList.Contains(neighbor))
                    continue;

                float tentativeG = gScore[current] +
                    Vector3.Distance(
                        current.transform.position,
                        neighbor.transform.position
                    );

                if (!gScore.ContainsKey(neighbor) ||
                    tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return new List<Waypoint>();
    }

    private static List<Waypoint> ReconstructPath(
        Dictionary<Waypoint, Waypoint> cameFrom,
        Waypoint current)
    {
        List<Waypoint> path = new List<Waypoint>();

        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return path;
    }
}

