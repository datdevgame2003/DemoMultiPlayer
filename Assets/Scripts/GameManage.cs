using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManage : NetworkBehaviour
{
    public static GameManage Instance; // Singleton

    [Header("UI Components")]
    public TextMeshProUGUI enemyCountText;
    public GameObject winUI;
    public AudioSource winSound;
    public GameObject gameOverUI;
    public AudioSource gameOverSound;
    [SerializeField] private TextMeshProUGUI goldText;
    [Header("Game Settings")]
    private const int enemiesToWin = 50;
    // NetworkVariable to sync values
    private NetworkVariable<int> enemyKillCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> goldCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isWinUIActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isLostUIActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isPaused = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public GameObject enemy;
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
        winUI.SetActive(false);
        gameOverUI.SetActive(false);
        enemyCountText.gameObject.SetActive(false);

        isWinUIActive.OnValueChanged += OnWinUIStateChanged;
        isLostUIActive.OnValueChanged += OnLostUIStateChanged;
        isPaused.OnValueChanged += OnPauseStateChanged;
        UpdateEnemyCountUI();
        UpdateGoldUI(goldCount.Value);
    }

    private void Update()
    {
        if (!enemyCountText.gameObject.activeSelf)
        {
            enemyCountText.gameObject.SetActive(true);
        }
        if (isPaused.Value)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void EnemyKilled()
    {
        if (!IsServer) return;

        enemyKillCount.Value++;
        UpdateEnemyCountUI();

        if (enemyKillCount.Value >= enemiesToWin)
        {
            Destroy(enemy);
            ShowWinUI();
        }
    }

    private void UpdateEnemyCountUI()
    {
        enemyCountText.text = "Enemies Killed: " + enemyKillCount.Value;
        UpdateEnemyCountClientRpc(enemyKillCount.Value);
    }

    [ClientRpc]
    private void UpdateEnemyCountClientRpc(int count)
    {
        enemyCountText.text = "Enemies Killed: " + count;
    }

    [ServerRpc]
    public void AddGoldServerRpc(int amount)
    {
        if (!IsServer) return;
        goldCount.Value += amount;
        UpdateGoldUIClientRpc(goldCount.Value);
    }

    [ClientRpc]
    private void UpdateGoldUIClientRpc(int count)
    {
        UpdateGoldUI(count);
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
            isLostUIActive.Value = true;
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

    public void UpdateGoldUI(int count)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {count}";
        }
    }
    [ServerRpc]
    public void PauseGameServerRpc()
    {
        isPaused.Value = true;
    }

    [ServerRpc]
    public void ResumeGameServerRpc()
    {
        isPaused.Value = false;
    }

    private void OnPauseStateChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }


    //public void Restart()
    //{
    //    if (!IsServer) return;

    //    // Reset all values
    //    enemyKillCount.Value = 0;
    //    goldCount.Value = 0;
    //    isWinUIActive.Value = false;
    //    isLostUIActive.Value = false;

    //    // Notify clients to reset UI
    //    ResetClientRpc();
    //    NetworkManager.Singleton.Shutdown();
    //    SceneManager.LoadScene("SampleScene");
    //    // Reload the scene
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //}

    //[ClientRpc]
    //private void ResetClientRpc()
    //{
    //    // Reset UI and values on clients
    //    winUI.SetActive(false);
    //    gameOverUI.SetActive(false);
    //    Time.timeScale = 1f;
    //    UpdateEnemyCountUI();
    //    UpdateGoldUI(0);
    //}
}
