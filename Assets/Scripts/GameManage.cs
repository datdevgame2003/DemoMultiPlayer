using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance; // Singleton:global management

    [Header("UI Components")]
    public TextMeshProUGUI enemyCountText;
    public GameObject winUI;
    public AudioSource winSound;
    public GameObject gameOverUI;
    public AudioSource gameOverSound;

    [Header("Game Settings")]
    private const int enemiesToWin = 10;

    // NetworkVariable to sync enemy kill count
    private NetworkVariable<int> enemyKillCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isWinUIActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isLostUIActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private GameObject player;
  

    private void Awake()
    {
        // Singleton
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
        winUI.SetActive(false);
        isWinUIActive.OnValueChanged += OnWinUIStateChanged;
        isLostUIActive.OnValueChanged += OnLostUIStateChanged;
        gameOverUI.SetActive(false);
        enemyCountText.gameObject.SetActive(false);
        UpdateEnemyCountUI();
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        if (player != null && !enemyCountText.gameObject.activeSelf)
        {
            enemyCountText.gameObject.SetActive(true);
        }
    }

    // Called when an enemy is killed
    public void EnemyKilled()
    {
        // Only the server can update the NetworkVariable
        if (!IsServer) return;

        enemyKillCount.Value++;
        UpdateEnemyCountUI();

        // Check if the player has won
        if (enemyKillCount.Value >= enemiesToWin)
        {
            ShowWinUI();
        }
    }

    private void UpdateEnemyCountUI()
    {
        // Sync the value with clients
        if (enemyCountText != null)
        {
            enemyCountText.text = "Enemies Killed: " + enemyKillCount.Value;
        }

        // Notify all clients about the updated kill count
        UpdateEnemyCountClientRpc(enemyKillCount.Value);
    }

    [ClientRpc]
    private void UpdateEnemyCountClientRpc(int killCount)
    {
        // Update the enemy count UI on all clients
        if (enemyCountText != null)
        {
            enemyCountText.text = "Enemies Killed: " + killCount;
        }
    }

    private void ShowWinUI()
    {
        if (IsServer) 
        {
            isWinUIActive.Value = true;
        }
    }
    private void OnWinUIStateChanged(bool oldValue, bool newValue)
    {
        winUI.SetActive(newValue);
        if (newValue)
        {
            winSound.Play();
            Time.timeScale = 0f;
        }
    }

    public void ShowGameOverUI()
    {
        if (IsServer)
        {
            isWinUIActive.Value = true;
        }
        
    }
    private void OnLostUIStateChanged(bool oldValue, bool newValue)
    {
        gameOverUI.SetActive(newValue);
        if (newValue)
        {
            gameOverSound.Play();
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        enemyKillCount.Value = 0;
        UpdateEnemyCountUI();
        winUI.SetActive(false);
        gameOverUI.SetActive(false);

        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        var uiNetworkManager = FindObjectOfType<UINetworkManager>();
        if (uiNetworkManager != null)
        {
            uiNetworkManager.gameObject.SetActive(true);

        }
    }
}
