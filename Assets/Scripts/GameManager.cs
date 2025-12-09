using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    public GameObject winPanel;   // ⭐ ADD THIS


public Transform exitTransform; // reference set by MazeGenerator
public float minRespawnDistanceFromExit = 5f;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
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

    // -------------------------------------
    // -------- LOSE LOGIC -----------------
    // -------------------------------------
    public void PlayerCaught()
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

    // -------------------------------------
    // -------- WIN / LOSE FUNCTIONS -------
    // -------------------------------------

    public void LoseGame()
    {
        Debug.Log("💀 GAME OVER");
        Time.timeScale = 0f;
        if (losePanel) losePanel.SetActive(true);
    }

    public void WinGame()
    {
        Debug.Log("🏆 YOU WIN!");
        Time.timeScale = 0f;
        if (winPanel) winPanel.SetActive(true);
    }

    // -------------------------------------
    // -------- BUTTON FUNCTIONS -----------
    // -------------------------------------

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
