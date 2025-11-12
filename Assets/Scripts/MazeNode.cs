using UnityEngine;

public class MazeNode : MonoBehaviour
{
    public enum NodeState
    {
        Available,
        Current,
        Completed
    }
    [SerializeField] GameObject[] walls;
    [SerializeField] MeshRenderer floor = new MeshRenderer();


    public void RenoveWall(int wallToRemove)
    {
        walls[wallToRemove].gameObject.SetActive(false);
    }
    public void SetState(NodeState state)
    {
        switch (state)
        {
            case NodeState.Available:
                floor.material.color = Color.white;
                break;
            case NodeState.Current:
                floor.material.color = Color.yellow;
                break;
            case NodeState.Completed:
                floor.material.color = Color.blue;
                break;
        }  

    }
}
