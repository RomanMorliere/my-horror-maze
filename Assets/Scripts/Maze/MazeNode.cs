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
}
