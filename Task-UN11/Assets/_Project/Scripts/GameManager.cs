using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("HUD")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text killsText;
    public TMP_Text hintText;
    public TMP_Text[] heartIcons;
    public GameObject bossHealthBarPanel;
    public Image bossHealthBarFill;
    public TMP_Text bossNameText;

    [Header("End Screens")]
    public TMP_Text finalScoreText;
    public TMP_Text winScoreText;
    public TMP_Text gameOverTitleText;

    [Header("World refs")]
    public GameObject player;
    public Transform playerSpawn;
    public GameObject weaponPickup;
    public GameObject startWaveTrigger;
    public EnemySpawner spawner;
    public GameObject heldWeapon;

    [Header("Settings")]
    public int maxLives = 3;
    public int totalEnemies = 13;
    public float timeLimit = 180f;

    [Header("Visual")]
    public Color heartAliveColor = new Color(1f, 0.2f, 0.2f, 1f);
    public Color heartDeadColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);

    public enum GameState { Menu, PreparingWeapon, ReadyToStart, Combat, GameOver, Win, Paused }
    public GameState state = GameState.Menu;

    int score, kills, currentLives;
    float timeLeft;
    GameState stateBeforePause;
    BossEnemy currentTrackedBoss;
    List<BossEnemy> activeBosses = new List<BossEnemy>();

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() { ShowMenu(); }

    void Update()
    {
        if (state == GameState.Combat)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0) { OnPlayerDied(); return; }
            UpdateHUD();
            UpdateBossBar();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && (state == GameState.Combat || state == GameState.Paused))
            TogglePause();
    }

    void SetPlayerControl(bool enabled)
    {
        if (!player) return;
        var fpc = player.GetComponent<FirstPersonController>();
        if (fpc) fpc.enabled = enabled;
        var inp = player.GetComponent<PlayerInput>();
        if (inp) inp.enabled = enabled;
        var shoot = player.GetComponentInChildren<PlayerShoot>(true);
        if (shoot) shoot.enabled = enabled && (state == GameState.Combat || state == GameState.ReadyToStart);
    }

    public void NewGame()
    {
        score = 0; kills = 0; timeLeft = timeLimit;
        currentLives = maxLives;
        activeBosses.Clear();
        currentTrackedBoss = null;

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var e in GameObject.FindGameObjectsWithTag("Enemy")) Destroy(e);
        RespawnPlayer();
        SetPlayerControl(true);

        if (heldWeapon) heldWeapon.SetActive(false);
        if (weaponPickup) weaponPickup.SetActive(true);
        if (startWaveTrigger) startWaveTrigger.SetActive(true);
        if (bossHealthBarPanel) bossHealthBarPanel.SetActive(false);

        var shoot = player ? player.GetComponentInChildren<PlayerShoot>(true) : null;
        if (shoot) shoot.enabled = false;

        var ph = player ? player.GetComponent<PlayerHealth>() : null;
        if (ph) ph.ResetHealth();

        if (spawner)
        {
            totalEnemies = spawner.GetTotalEnemyCount();
            spawner.OnAllWavesCompleted = Win;
            spawner.OnWaveStarted = (cur, total) => SetHint($"WAVE {cur} / {total}");
        }

        UpdateHearts();
        Show(hudPanel);
        SetHint("PICK UP THE WEAPON (E)");
        state = GameState.PreparingWeapon;
    }

    void RespawnPlayer()
    {
        if (!player || !playerSpawn) return;
        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
        player.transform.position = playerSpawn.position;
        player.transform.rotation = playerSpawn.rotation;
        if (cc) cc.enabled = true;
        player.SetActive(true);
    }

    void UpdateHearts()
    {
        if (heartIcons == null) return;
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] == null) continue;
            heartIcons[i].color = (i < currentLives) ? heartAliveColor : heartDeadColor;
        }
    }

    public void OnWeaponPickedUp()
    {
        if (state != GameState.PreparingWeapon) return;
        if (weaponPickup) weaponPickup.SetActive(false);
        if (heldWeapon) heldWeapon.SetActive(true);
        var shoot = player ? player.GetComponentInChildren<PlayerShoot>(true) : null;
        if (shoot) shoot.enabled = true;
        if (startWaveTrigger) startWaveTrigger.SetActive(true);
        SetHint("FIND THE RED PILLAR AND PRESS E TO START");
        state = GameState.ReadyToStart;
    }

    public void OnWaveStarted()
    {
        if (state != GameState.ReadyToStart) return;
        if (startWaveTrigger) startWaveTrigger.SetActive(false);
        if (spawner) spawner.SpawnEnemies();
        SetHint("");
        state = GameState.Combat;
    }

    public void RegisterBoss(BossEnemy b)
    {
        if (!activeBosses.Contains(b)) activeBosses.Add(b);
        if (bossHealthBarPanel) bossHealthBarPanel.SetActive(true);
        currentTrackedBoss = b;
    }

    void UpdateBossBar()
    {
        activeBosses.RemoveAll(b => b == null);
        if (activeBosses.Count == 0) { if (bossHealthBarPanel) bossHealthBarPanel.SetActive(false); return; }
        if (currentTrackedBoss == null) currentTrackedBoss = activeBosses[0];
        if (bossHealthBarFill) bossHealthBarFill.fillAmount = currentTrackedBoss.GetHealthPercent();
        if (bossNameText) bossNameText.text = currentTrackedBoss.bossName;
    }

    public void OnEnemyKilled(int scoreValue = 100)
    {
        kills++; score += scoreValue;
    }

    void UpdateHUD()
    {
        if (scoreText) scoreText.text = "SCORE  " + score;
        if (timerText) timerText.text = "TIME  " + Mathf.CeilToInt(timeLeft) + "s";
        if (killsText) killsText.text = "KILLS  " + kills + " / " + totalEnemies;
    }

    void SetHint(string s) { if (hintText) hintText.text = s; }

    public void OnPlayerDied()
    {
        if (state == GameState.GameOver || state == GameState.Win) return;
        state = GameState.GameOver;
        Time.timeScale = 0;
        UnlockCursor();
        SetPlayerControl(false);

        int livesAfter = currentLives - 1;
        if (gameOverTitleText)
            gameOverTitleText.text = livesAfter > 0 ? "YOU DIED" : "GAME OVER";
        if (finalScoreText)
            finalScoreText.text = livesAfter > 0
                ? $"LIVES LEFT  {livesAfter} / {maxLives}\nSCORE  {score}"
                : $"FINAL SCORE  {score}\nKILLS  {kills}";

        Show(gameOverPanel);
    }

    public void TryRetry()
    {
        currentLives--;
        UpdateHearts();

        if (currentLives <= 0) { Restart(); return; }

        var ph = player ? player.GetComponent<PlayerHealth>() : null;
        if (ph) ph.ResetHealth();
        RespawnPlayer();
        SetPlayerControl(true);

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Show(hudPanel);
        state = GameState.Combat;
    }

    public void Win()
    {
        if (state == GameState.Win) return;
        state = GameState.Win;
        Time.timeScale = 0; UnlockCursor();
        SetPlayerControl(false);
        if (winScoreText) winScoreText.text = "FINAL SCORE  " + score;
        Show(winPanel);
    }

    public void TogglePause()
    {
        if (state == GameState.Paused)
        {
            state = stateBeforePause;
            Time.timeScale = 1; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
            SetPlayerControl(true);
            Show(hudPanel);
        }
        else
        {
            stateBeforePause = state;
            state = GameState.Paused;
            Time.timeScale = 0; UnlockCursor();
            SetPlayerControl(false);
            Show(pausePanel);
        }
    }

    public void Restart() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    public void ShowMenu()
    {
        state = GameState.Menu; Time.timeScale = 0; UnlockCursor(); Show(mainMenuPanel);
        if (player) { SetPlayerControl(false); player.SetActive(false); }
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void UnlockCursor() { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }

    void Show(GameObject panel)
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(panel == mainMenuPanel);
        if (hudPanel) hudPanel.SetActive(panel == hudPanel);
        if (pausePanel) pausePanel.SetActive(panel == pausePanel);
        if (gameOverPanel) gameOverPanel.SetActive(panel == gameOverPanel);
        if (winPanel) winPanel.SetActive(panel == winPanel);
    }
}