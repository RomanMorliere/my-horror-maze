using UnityEngine;

public class MazeNode : MonoBehaviour
{
    public enum NodeState
    {
        Wall,       // <--- Added this so we can detect non-walkable nodes
        Available,
        Current,
        Completed
    }

    [Header("Node Info")]
    public NodeState state = NodeState.Wall; // <--- This stores the current state

    public bool IsWalkable => state != NodeState.Wall;

    [SerializeField] public GameObject[] walls;
    [SerializeField] MeshRenderer floor;

    // 0 = PosX, 1 = NegX, 2 = PosZ, 3 = NegZ
    public bool[] openWalls = new bool[4];

    public bool IsOpen(int dir)
    {
        return openWalls[dir];
    }

    public void RemoveWall(int wallToRemove)
    {
        walls[wallToRemove].SetActive(false);
        openWalls[wallToRemove] = true;
    }

    public void SetState(NodeState newState)
    {
        state = newState; // <---- This was missing before

        switch (newState)
        {
            case NodeState.Wall:
                floor.material.color = Color.gray;
                break;

            case NodeState.Available:
                floor.material.color = Color.white;
                break;

            case NodeState.Current:
                floor.material.color = Color.magenta;
                break;

            case NodeState.Completed:
                floor.material.color = Color.black;
                break;
        }
    }

    // Inside MazeNode.cs

public void DestroyClosestWall(Vector3 playerPosition)
{
    float closestDistance = float.MaxValue;
    int wallIndex = -1;

    for (int i = 0; i < walls.Length; i++)
    {
        if (walls[i] != null && walls[i].activeSelf)
        {
            float dist = Vector3.Distance(playerPosition, walls[i].transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                wallIndex = i;
            }
        }
    }

    if (wallIndex != -1 && closestDistance < 3.5f) 
    {
        GameObject wallToDestroy = walls[wallIndex];
        Vector3 wallPos = wallToDestroy.transform.position;

        // 1. Disable the target wall
        wallToDestroy.SetActive(false); 
        openWalls[wallIndex] = true; 

        // 2. ⭐ GHOST CLEANUP: Disable the neighbor's wall too
        // We look for any other wall at the exact same position
        Collider[] duplicates = Physics.OverlapSphere(wallPos, 0.2f);
        foreach (var col in duplicates)
        {
            if (col.gameObject != wallToDestroy && col.name.Contains("Wall"))
            {
                col.gameObject.SetActive(false);
            }
        }
        
        Debug.Log($"🧱 SUCCESS: Disabled {wallToDestroy.name}");
    }
}
}
