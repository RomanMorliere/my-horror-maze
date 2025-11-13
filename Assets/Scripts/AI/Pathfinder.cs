using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private MazeNode[,] grid;
    private int width, height;

    public Pathfinder(MazeNode[,] grid)
    {
        this.grid = grid;
        width = grid.GetLength(0);
        height = grid.GetLength(1);
    }

    public List<MazeNode> FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == goal)
                return Reconstruct(cameFrom, start, goal);

            foreach (Vector2Int next in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(next))
                {
                    cameFrom[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        return null;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        MazeNode node = grid[pos.x, pos.y];

        // PosX
        if (node.IsOpen(0) && pos.x + 1 < width)
            neighbors.Add(new Vector2Int(pos.x + 1, pos.y));

        // NegX
        if (node.IsOpen(1) && pos.x - 1 >= 0)
            neighbors.Add(new Vector2Int(pos.x - 1, pos.y));

        // PosZ
        if (node.IsOpen(2) && pos.y + 1 < height)
            neighbors.Add(new Vector2Int(pos.x, pos.y + 1));

        // NegZ
        if (node.IsOpen(3) && pos.y - 1 >= 0)
            neighbors.Add(new Vector2Int(pos.x, pos.y - 1));

        return neighbors;
    }

    private List<MazeNode> Reconstruct(
        Dictionary<Vector2Int, Vector2Int> cameFrom,
        Vector2Int start,
        Vector2Int goal)
    {
        List<MazeNode> path = new List<MazeNode>();
        Vector2Int current = goal;

        while (current != start)
        {
            path.Add(grid[current.x, current.y]);
            current = cameFrom[current];
        }

        path.Add(grid[start.x, start.y]);
        path.Reverse();
        return path;
    }
}
