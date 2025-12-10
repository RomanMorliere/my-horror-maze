using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Prefabs & Materials")]
    [SerializeField] MazeNode nodePrefab;
    [SerializeField] Vector2Int mazeSize;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject speedBoostPrefab;
    [SerializeField] int numberOfBoosts = 5;
    [SerializeField] Material wallGenerationMaterial;
    [SerializeField] Material wallsMaterial;
    [Header("Reveal Boost")]
[SerializeField] private GameObject revealBoostPrefab; // Drag your RevealBoost Prefab here in the Inspector!
[SerializeField] private int numberOfRevealBoosts = 3;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private float exitYOffset = 0.1f;
    
    [SerializeField] GameObject enemyPrefab;

    // ---- Pathfinding stuff ----
    private MazeNode[,] mazeGrid;
    private Pathfinder pathfinder;
    private EnemyAI_Follow enemyAI;
    private Transform player;

    private void Start()
    {
        StartCoroutine(GenerateMaze(mazeSize));
    }

    IEnumerator GenerateMaze(Vector2Int size)
    {
        List<MazeNode> nodes = new List<MazeNode>();

        // ---------------------------
        // 1. CREATE GRID (animated)
        // ---------------------------
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                MazeNode newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
                yield return null;         // <-- nice build animation
            }
        }

        // ---------------------------
        // 2. APPLY "GENERATION" MATERIAL
        // ---------------------------
        foreach (var node in nodes)
        {
            var renderer = node.GetComponentInChildren<MeshRenderer>();
            if (renderer != null && wallGenerationMaterial != null)
                renderer.material = wallGenerationMaterial;
        }

        // ---------------------------
        // 3. CARVE MAZE (original algo)
        // ---------------------------
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

            // RIGHT
            if (currentNodeX < size.x - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }

            // LEFT
            if (currentNodeX > 0)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }

            // UP
            if (currentNodeY < size.y - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }

            // DOWN
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

                // Remove walls (visual) + mark open walls for pathfinding
                switch (possibleDirections[chosenDirection])
                {
                    case 1: // RIGHT (current -> PosX, chosen -> NegX)
                        chosenNode.RemoveWall(1);
                        current.RemoveWall(0);
                        break;

                    case 2: // LEFT (current -> NegX, chosen -> PosX)
                        chosenNode.RemoveWall(0);
                        current.RemoveWall(1);
                        break;

                    case 3: // UP (current -> PosZ, chosen -> NegZ)
                        chosenNode.RemoveWall(3);
                        current.RemoveWall(2);
                        break;

                    case 4: // DOWN (current -> NegZ, chosen -> PosZ)
                        chosenNode.RemoveWall(2);
                        current.RemoveWall(3);
                        break;
                }

                // state colors
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

        // ---------------------------
        // 4. SPAWN PLAYER (same as before)
        // ---------------------------
        Vector3 startPos = new Vector3(-(mazeSize.x / 2f), 0f, -(mazeSize.y / 2f));
        GameObject playerGO = Instantiate(playerPrefab, startPos, Quaternion.identity);
        player = playerGO.transform;   // keep reference for AI

        Renderer playerRenderer = playerGO.GetComponent<Renderer>();
        if (playerRenderer)
            playerRenderer.material.color = Color.red; // same as friend

        FollowCamera cam = Camera.main.GetComponent<FollowCamera>();
        if (cam)
            cam.target = player;

        // ---------------------------
        // 5. SPEED BOOSTS (safe)
        // ---------------------------
        if (speedBoostPrefab != null)
        {
            for (int i = 0; i < numberOfBoosts; i++)
            {
                int randomX = Random.Range(0, mazeSize.x);
                int randomY = Random.Range(0, mazeSize.y);
                Vector3 pos = new Vector3(randomX - (mazeSize.x / 2f), 0.1f, randomY - (mazeSize.y / 2f));
                Instantiate(speedBoostPrefab, pos, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("SpeedBoostPrefab not assigned – no boosts spawned.");
        }


if (revealBoostPrefab != null)
{
    for (int i = 0; i < numberOfRevealBoosts; i++)
    {
        // Get a random X and Y coordinate within the maze grid
        int randomX = Random.Range(0, mazeSize.x);
        int randomY = Random.Range(0, mazeSize.y);
        
        // Calculate the world position of the random node/cell
        Vector3 pos = new Vector3(randomX - (mazeSize.x / 2f), 0.1f, randomY - (mazeSize.y / 2f));
        
        // Create the boost object in the scene
        Instantiate(revealBoostPrefab, pos, Quaternion.identity, transform); // Set parent for organization
    }
}
else
{
    Debug.LogWarning("⚠️ RevealBoostPrefab not assigned – no shadow reveal boosts spawned.");
}

        // ---------------------------
        // 6. FINAL WALL MATERIAL
        // ---------------------------
        foreach (var node in nodes)
        {
            var renderer = node.GetComponentInChildren<MeshRenderer>();
            if (renderer != null && wallsMaterial != null)
                renderer.material = wallsMaterial;
        }
        
        // EXIT
// Place exit at the top-right corner of the maze
        float exitX = (mazeSize.x - 1) - (mazeSize.x / 2f);
        float exitZ = (mazeSize.y - 1) - (mazeSize.y / 2f);

        Vector3 exitPos = new Vector3(exitX, exitYOffset, exitZ);
        GameObject exit = Instantiate(exitPrefab, exitPos, Quaternion.identity);
GameManager.Instance.exitTransform = exit.transform;   // ⭐ store exit reference


        // ---------------------------
        // 7. BUILD GRID FOR PATHFINDER
        // ---------------------------
        mazeGrid = new MazeNode[mazeSize.x, mazeSize.y];
        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                mazeGrid[x, y] = nodes[x * mazeSize.y + y];
            }
        }

        pathfinder = new Pathfinder(mazeGrid);

        // ---------------------------
        // 8. SPAWN ENEMY
        // ---------------------------
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

        // ---------------------------
        // 9. START A* UPDATE LOOP
        // ---------------------------
        if (enemyAI != null)
            StartCoroutine(UpdateEnemyPath());
    }

    IEnumerator UpdateEnemyPath()
    {
        while (true)
        {
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
    while (true)
    {
        int x = Random.Range(0, mazeSize.x);
        int y = Random.Range(0, mazeSize.y);

        MazeNode node = mazeGrid[x, y];

        // Must have at least one open wall (walkable)
        bool isOpen = node.openWalls[0] || node.openWalls[1] || node.openWalls[2] || node.openWalls[3];
        if (!isOpen) continue;

        Vector3 pos = node.transform.position + Vector3.up * 0.2f;

        // -------- 1️⃣ Avoid enemy ----------
        if (enemy != null && Vector3.Distance(pos, enemy.position) < minDistance)
            continue;

        // -------- 2️⃣ Avoid player's previous position ----------
        if (Vector3.Distance(pos, player.position) < 3f)
            continue;

        // -------- 3️⃣ Avoid EXIT ----------
        if (GameManager.Instance.exitTransform != null)
        {
            float distToExit = Vector3.Distance(pos, GameManager.Instance.exitTransform.position);

            if (distToExit < GameManager.Instance.minRespawnDistanceFromExit)
                continue;  // too close to exit → try another cell
        }

        return pos;
    }

    
    
}


public IEnumerator RevealWalls(float duration)
{
    Debug.Log($"💡 Starting wall reveal for {duration} seconds. (Shadows OFF)");
    
    // --- 1. Turn shadows OFF ---
    foreach (var node in mazeGrid)
    {
        foreach (var wall in node.walls)
        {
            if (wall.activeSelf)
            {
                // Use GetComponentInChildren for safety
                var renderer = wall.GetComponentInChildren<MeshRenderer>(); 
                
                if (renderer != null)
                {
                    // ⭐ CONFIRM RECEIVE SHADOWS IS THE TARGET PROPERTY
                    renderer.receiveShadows = false; 
                    // ⭐ NEW: FORCE ALL SHADOWS OFF, just in case (casting shadows)
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
        }
    }

    // --- 2. Wait for the boost duration ---
    yield return new WaitForSeconds(duration);

    Debug.Log("🌑 Ending wall reveal. Shadows back ON.");
    
    // --- 3. Turn shadows back ON ---
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
                    // ⭐ Restore SHADOW CASTING
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
            }
        }
    }
}

}
