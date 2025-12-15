using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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
        PlayerController p = FindFirstObjectByType<PlayerController>();
        if (p && p.IsInvincible == false)
        {
            lives--;
            UpdateHeartsUI();
            
            if (lives <= 0)
            {
                LoseGame();
                return;
            }
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

    // Add this to GameManager.cs
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
                $"<color=#FFF700><b>    [ R ]</b></color> Speed:  <b>{cachedPlayer.speedCharges}</b>\n" + // ⭐ NEW LINE
                $"<color=#FF00BB><b>    [SPACE]</b></color> Hammer: <b>{cachedPlayer.wallBreakerCharges}</b>\n" +
                $"<color=#6780FF><b>    [ Q ]</b></color> Shield: <b>{cachedPlayer.shieldCharges}</b>\n" +
                $"<color=#0054FF><b>    [ E ]</b></color> Reveal: <b>{cachedPlayer.revealCharges}</b>";
        }
    }
}

}
