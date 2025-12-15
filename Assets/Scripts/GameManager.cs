using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Level
{
    public Vector2Int mazeSize;
    public int numberOfBoosts;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int currentLevel = 1; // Start at level 1

    [Header("Levels")]
    public List<Level> levels;

    private PlayerController cachedPlayer;

    [Header("UI Hearts")]
    public List<Image> hearts = new List<Image>();
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Player Settings")]
    public Transform player;
    public MazeGenerator mazeGenerator;
    public int lives = 3;

    [Header("Panels")]
    public GameObject losePanel;
    public GameObject winPanel;

    public Transform exitTransform;
    public float minRespawnDistanceFromExit = 5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (currentLevel > levels.Count)
        {
            currentLevel = Mathf.Clamp(currentLevel, 1, levels.Count);
        }

        // Set up the maze generator for the current level
        if (levels.Count > 0 && currentLevel -1 < levels.Count)
        {
            Level levelData = levels[currentLevel - 1];
            mazeGenerator.mazeSize = levelData.mazeSize;
            mazeGenerator.numberOfBoosts = levelData.numberOfBoosts;
        }

        StartCoroutine(AssignPlayerWhenReady());

        if (losePanel) losePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    IEnumerator AssignPlayerWhenReady()
    {
        Debug.Log("⏳ Waiting for player spawn...");

        while (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                Debug.Log("✅ Player assigned.");
                break;
            }

            yield return null;
        }

        UpdateHeartsUI();
    }

    public void PlayerCaught()
    {
        PlayerController p = FindFirstObjectByType<PlayerController>();
        if (p && !p.IsInvincible)
        {
            lives--;
            UpdateHeartsUI();

            if (lives <= 0)
            {
                LoseGame();
                return;
            }
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        Debug.Log("🔄 Respawning player...");

        Vector3 newPos = mazeGenerator.GetSafeRespawnPosition(
            enemy: FindObjectOfType<EnemyAI_Follow>().transform,
            player: player
        );

        newPos.y = player.position.y;
        player.position = newPos;
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < lives ? fullHeart : emptyHeart;
        }
    }

    public void LoseGame()
    {
        Debug.Log("💀 GAME OVER");
        Time.timeScale = 0f;
        if (losePanel) losePanel.SetActive(true);
    }

    public void WinGame()
    {
        currentLevel++;
        if (currentLevel > levels.Count)
        {
            Debug.Log("🏆 YOU WIN THE WHOLE GAME!");
            Time.timeScale = 0f;
            if (winPanel) winPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"🏆 LEVEL {currentLevel - 1} COMPLETE! Loading next level...");
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        currentLevel = 1; // Reset to level 1 when going to menu
        SceneManager.LoadScene("MainMenu");
    }

    public Text boostStatusText;
    void Update()
    {
        if (boostStatusText != null)
        {
            if (cachedPlayer == null) cachedPlayer = FindFirstObjectByType<PlayerController>();

            if (cachedPlayer != null)
            {
                boostStatusText.text =
                    "<size=14><b>INVENTORY : </b></size>\n\n" +
                    $"<color=#FFF700><b>    [ R ]</b></color> Speed:  <b>{cachedPlayer.speedCharges}</b>\n" +
                    $"<color=#FF00BB><b>    [SPACE]</b></color> Hammer: <b>{cachedPlayer.wallBreakerCharges}</b>\n" +
                    $"<color=#6780FF><b>    [ Q ]</b></color> Shield: <b>{cachedPlayer.shieldCharges}</b>\n" +
                    $"<color=#0054FF><b>    [ E ]</b></color> Reveal: <b>{cachedPlayer.revealCharges}</b>";
            }
        }
    }
}
