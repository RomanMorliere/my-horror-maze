using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Prefabs & Materials")]
    [SerializeField] MazeNode nodePrefab;
    [HideInInspector] public Vector2Int mazeSize;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject speedBoostPrefab;
    [SerializeField] GameObject shieldPrefab;
    [HideInInspector] public int numberOfBoosts = 15;
    [SerializeField] Material wallGenerationMaterial;
    [SerializeField] Material wallsMaterial;
    [Header("Reveal Boost")]
    [SerializeField] private GameObject revealBoostPrefab;
    [SerializeField] private int numberOfRevealBoosts = 3;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private float exitYOffset = 0.1f;
    [SerializeField] GameObject hammerPrefab;
    [SerializeField] GameObject enemyPrefab;

    private MazeNode[,] mazeGrid;
    private Pathfinder pathfinder;
    private EnemyAI_Follow enemyAI;
    private Transform player;

    private void Start()
    {
        Time.timeScale = 1f;
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
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            int currentNodeX = currentNodeIndex / mazeSize.y;
            int currentNodeY = currentNodeIndex % mazeSize.y;

            if (currentNodeX < size.x - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }

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
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }

            if (currentNodeY > 0)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                MazeNode chosenNode = nodes[possibleNextNodes[chosenDirection]];
                MazeNode current = currentPath[currentPath.Count - 1];

                switch (possibleDirections[chosenDirection])
                {
                    case 1:
                        current.RemoveWall(0);
                        chosenNode.RemoveWall(1);
                        break;
                    case 2:
                        current.RemoveWall(1);
                        chosenNode.RemoveWall(0);
                        break;
                    case 3:
                        current.RemoveWall(2);
                        chosenNode.RemoveWall(3);
                        break;
                    case 4:
                        current.RemoveWall(3);
                        chosenNode.RemoveWall(2);
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
        GameObject playerGO = Instantiate(playerPrefab, startPos, Quaternion.identity);
        player = playerGO.transform;

        Renderer playerRenderer = playerGO.GetComponent<Renderer>();
        if (playerRenderer)
            playerRenderer.material.color = Color.red;

        FollowCamera cam = Camera.main.GetComponent<FollowCamera>();
        if (cam)
            cam.target = player;

        for (int i = 0; i < numberOfBoosts; i++)
        {
            int randomX = Random.Range(0, mazeSize.x);
            int randomY = Random.Range(0, mazeSize.y);
            Vector3 pos = new Vector3(randomX - (mazeSize.x / 2f), 0.1f, randomY - (mazeSize.y / 2f));

            int boostType = Random.Range(0, 4);
            switch (boostType)
            {
                case 0:
                    Instantiate(speedBoostPrefab, pos, Quaternion.identity);
                    break;
                case 1:
                    Instantiate(revealBoostPrefab, pos, Quaternion.identity);
                    break;
                case 2:
                    Instantiate(shieldPrefab, pos, Quaternion.identity);
                    break;
                case 3:
                    Instantiate(hammerPrefab, pos, Quaternion.identity);
                    break;
            }
        }

        foreach (var node in nodes)
        {
            var renderer = node.GetComponentInChildren<MeshRenderer>();
            if (renderer != null && wallsMaterial != null)
                renderer.material = wallsMaterial;
        }

        float exitX = (mazeSize.x - 1) - (mazeSize.x / 2f);
        float exitZ = (mazeSize.y - 1) - (mazeSize.y / 2f);

        Vector3 exitPos = new Vector3(exitX, exitYOffset, exitZ);
        GameObject exit = Instantiate(exitPrefab, exitPos, Quaternion.identity);
        GameManager.Instance.exitTransform = exit.transform;

        mazeGrid = new MazeNode[mazeSize.x, mazeSize.y];
        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                mazeGrid[x, y] = nodes[x * mazeSize.y + y];
            }
        }

        pathfinder = new Pathfinder(mazeGrid);

        if (enemyPrefab != null)
        {
            MazeNode spawnNode = nodes[Random.Range(0, nodes.Count)];
            Vector3 epos = spawnNode.transform.position + Vector3.up;
            GameObject enemy = Instantiate(enemyPrefab, epos, Quaternion.identity);

            enemyAI = enemy.GetComponent<EnemyAI_Follow>();
            if (enemyAI != null)
                enemyAI.SetTarget(player);
        }
        else
        {
            Debug.LogWarning("EnemyPrefab not assigned – no enemy spawned.");
        }

        if (enemyAI != null)
            StartCoroutine(UpdateEnemyPath());
    }

    IEnumerator UpdateEnemyPath()
    {
        while (true)
        {
            if (enemyAI == null || player == null) yield return new WaitForSeconds(5f);
            Vector2Int enemyCell = WorldToCell(enemyAI.transform.position);
            Vector2Int playerCell = WorldToCell(player.position);

            List<MazeNode> path = pathfinder.FindPath(enemyCell, playerCell);
            if (path != null)
                enemyAI.SetPath(path);

            yield return new WaitForSeconds(5f);
        }
    }

    private Vector2Int WorldToCell(Vector3 p)
    {
        int x = Mathf.RoundToInt(p.x + mazeSize.x / 2f);
        int y = Mathf.RoundToInt(p.z + mazeSize.y / 2f);
        return new Vector2Int(x, y);
    }

    public Vector3 GetSafeRespawnPosition(Transform enemy, Transform player, float minDistance = 5f)
    {
        int attempts = 0;
        while (attempts < 100)
        {
            int x = Random.Range(0, mazeSize.x);
            int y = Random.Range(0, mazeSize.y);

            MazeNode node = mazeGrid[x, y];

            bool isOpen = node.openWalls[0] || node.openWalls[1] || node.openWalls[2] || node.openWalls[3];
            if (!isOpen) continue;

            Vector3 pos = node.transform.position + Vector3.up * 0.2f;

            if (enemy != null && Vector3.Distance(pos, enemy.position) < minDistance)
            {
                attempts++;
                continue;
            }

            if (Vector3.Distance(pos, player.position) < 3f)
            {
                attempts++;
                continue;
            }

            if (GameManager.Instance.exitTransform != null)
            {
                float distToExit = Vector3.Distance(pos, GameManager.Instance.exitTransform.position);

                if (distToExit < GameManager.Instance.minRespawnDistanceFromExit)
                {
                    attempts++;
                    continue;
                }
            }

            return pos;
        }

        Debug.LogWarning("Could not find a safe respawn position after 100 attempts. Spawning at a random open node.");
        while (true)
        {
            int x = Random.Range(0, mazeSize.x);
            int y = Random.Range(0, mazeSize.y);
            MazeNode node = mazeGrid[x, y];
            bool isOpen = node.openWalls[0] || node.openWalls[1] || node.openWalls[2] || node.openWalls[3];
            if (isOpen)
            {
                return node.transform.position + Vector3.up * 0.2f;
            }
        }
    }

    public IEnumerator RevealWalls(float duration)
    {
        Debug.Log($"💡 Starting wall reveal for {duration} seconds. (Shadows OFF)");

        foreach (var node in mazeGrid)
        {
            foreach (var wall in node.walls)
            {
                if (wall.activeSelf)
                {
                    var renderer = wall.GetComponentInChildren<MeshRenderer>();

                    if (renderer != null)
                    {
                        renderer.receiveShadows = false;
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }
        }

        yield return new WaitForSeconds(duration);

        Debug.Log("🌑 Ending wall reveal. Shadows back ON.");

        foreach (var node in mazeGrid)
        {
            foreach (var wall in node.walls)
            {
                if (wall.activeSelf)
                {
                    var renderer = wall.GetComponentInChildren<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.receiveShadows = true;
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                }
            }
        }
    }
}
