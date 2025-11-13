using UnityEngine;

public class MazeNode : MonoBehaviour
{
    public enum NodeState
    {
        Available,
        Current,
        Completed
    }

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

    public void SetState(NodeState state)
    {
        switch (state)
        {
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
