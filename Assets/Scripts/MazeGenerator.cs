using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] MazeNode nodePrefab;
    [SerializeField] Vector2Int mazeSize;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private GameObject speedBoostPrefab;
    [SerializeField] private int numberOfBoosts = 5;
    
    [SerializeField] private Material wallGenerationMaterial;
    [SerializeField] private Material wallsMaterial;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //GenerateMazeInstant(mazeSize);
        StartCoroutine(GenerateMaze(mazeSize));
    }
    IEnumerator GenerateMaze(Vector2Int size)
    {
        List<MazeNode> nodes = new List<MazeNode>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                MazeNode newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
                yield return null;
            }

        }
        
        // apply "Wall Generation" material to all maze node floors at start
        foreach (var node in nodes)
        {
            var renderer = node.GetComponentInChildren<MeshRenderer>();
            if (renderer != null && wallGenerationMaterial != null)
                renderer.material = wallGenerationMaterial;
        }

        List<MazeNode> currentPath = new List<MazeNode>();
        List<MazeNode> completedNodes = new List<MazeNode>();

        currentPath.Add(nodes[Random.Range(0, nodes.Count)]);
        currentPath[0].SetState(MazeNode.NodeState.Current);

        while (completedNodes.Count < nodes.Count)
        {
            // check nodes next to the current node
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();
            
            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            int currentNodeX = currentNodeIndex / mazeSize.y;
            int currentNodeY = currentNodeIndex % mazeSize.y;

            
            // the right of current node
            if (currentNodeX < size.x - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            // check left of current node
            if (currentNodeX > 0)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }

            if (currentNodeY < size.y - 1)
            {
                // check node above the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }

            if (currentNodeY > 0)
            {
                //check node below current node 
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            // choose next node
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                MazeNode chosenNode = nodes[possibleNextNodes[chosenDirection]];

                switch (possibleDirections[chosenDirection])
                {
                    case 1:
                        chosenNode.RenoveWall(1);
                        currentPath[currentPath.Count - 1].RenoveWall(0);
                        break;
                    case 2:
                        chosenNode.RenoveWall(0);
                        currentPath[currentPath.Count - 1].RenoveWall(1);
                        break;
                    case 3:
                        chosenNode.RenoveWall(3);
                        currentPath[currentPath.Count - 1].RenoveWall(2);
                        break;
                    case 4:
                        chosenNode.RenoveWall(2);
                        currentPath[currentPath.Count - 1].RenoveWall(3);
                        break;
                    
                }
                
                currentPath.Add(chosenNode);
                chosenNode.SetState(MazeNode.NodeState.Current);
            }
            else
            {
                completedNodes.Add(currentPath[currentPath.Count - 1]);
                
                currentPath[currentPath.Count - 1].SetState(MazeNode.NodeState.Completed);
                currentPath.RemoveAt(currentPath.Count - 1);
                
            }

            yield return null;
        }

        Vector3 startPos = new Vector3(-(mazeSize.x / 2f), 0f, -(mazeSize.y / 2f));
        GameObject player = Instantiate(playerPrefab, startPos, Quaternion.identity);

        Renderer playerRenderer = player.GetComponent<Renderer>();
        if (playerRenderer)
            playerRenderer.material.color = Color.red;

        FollowCamera cam = Camera.main.GetComponent<FollowCamera>();
        if (cam)
            cam.target = player.transform;

        for (int i = 0; i < numberOfBoosts; i++)
        {
            int randomX = Random.Range(0, mazeSize.x);
            int randomY = Random.Range(0, mazeSize.y);
            Vector3 pos = new Vector3(randomX - (mazeSize.x / 2f), 0.1f, randomY - (mazeSize.y / 2f));
            Instantiate(speedBoostPrefab, pos, Quaternion.identity);
        }
        
        // switch to final wall material once the maze is ready
        foreach (var node in nodes)
        {
            var renderer = node.GetComponentInChildren<MeshRenderer>();
            if (renderer != null && wallsMaterial != null)
                renderer.material = wallsMaterial;
        }

        
        
    }
    
    void GenerateMazeInstant(Vector2Int size)
    {
        List<MazeNode> nodes = new List<MazeNode>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                MazeNode newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
            }

        }

        List<MazeNode> currentPath = new List<MazeNode>();
        List<MazeNode> completedNodes = new List<MazeNode>();

        currentPath.Add(nodes[Random.Range(0, nodes.Count)]);
        //currentPath[0].SetState(MazeNode.NodeState.Current);

        while (completedNodes.Count < nodes.Count)
        {
            // check nodes next to the current node
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();
            
            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            int currentNodeX = currentNodeIndex / mazeSize.y;
            int currentNodeY = currentNodeIndex % mazeSize.y;

            
            // the right of current node
            if (currentNodeX < size.x - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            // check left of current node
            if (currentNodeX > 0)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }

            if (currentNodeY < size.y - 1)
            {
                // check node above the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }

            if (currentNodeY > 0)
            {
                //check node below current node 
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            // choose next node
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                MazeNode chosenNode = nodes[possibleNextNodes[chosenDirection]];

                switch (possibleDirections[chosenDirection])
                {
                    case 1:
                        chosenNode.RenoveWall(1);
                        currentPath[currentPath.Count - 1].RenoveWall(0);
                        break;
                    case 2:
                        chosenNode.RenoveWall(0);
                        currentPath[currentPath.Count - 1].RenoveWall(1);
                        break;
                    case 3:
                        chosenNode.RenoveWall(3);
                        currentPath[currentPath.Count - 1].RenoveWall(2);
                        break;
                    case 4:
                        chosenNode.RenoveWall(2);
                        currentPath[currentPath.Count - 1].RenoveWall(3);
                        break;
                    
                }
                
                currentPath.Add(chosenNode);
                //chosenNode.SetState(MazeNode.NodeState.Current);
            }
            else
            {
                completedNodes.Add(currentPath[currentPath.Count - 1]);
                
                //currentPath[currentPath.Count - 1].SetState(MazeNode.NodeState.Completed);
                currentPath.RemoveAt(currentPath.Count - 1);
                
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
