using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton:global management

    [Header("UI Components")]
    public TextMeshProUGUI enemyCountText;
    public GameObject winUI;
    public AudioSource winSound;
    public GameObject gameOverUI;
    public AudioSource gameOverSound;
    [Header("Game Settings")]
    private int enemyKillCount = 0;
    private const int enemiesToWin = 10;
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
    // enemy kill +
    public void EnemyKilled()
    {
        enemyKillCount++;
        UpdateEnemyCountUI();


        if (enemyKillCount >= enemiesToWin)
        {
            ShowWinUI();
        }
    }


    private void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = "Enemies Killed: " + enemyKillCount;
        }
    }


    private void ShowWinUI()
    {
        winUI.SetActive(true);

        winSound.Play();



        Time.timeScale = 0f;
    }

    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
        gameOverSound.Play();
        Time.timeScale = 0f;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        enemyKillCount = 0;
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
