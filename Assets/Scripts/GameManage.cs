using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton để quản lý toàn cục

    [Header("UI Components")]
    public TextMeshProUGUI enemyCountText;
    public GameObject winUI; 
    public AudioSource winSound;
    public GameObject gameOverUI; 
    public AudioSource gameOverSound;
    [Header("Game Settings")]
    private int enemyKillCount = 0; 
    private const int enemiesToWin = 10; 

    private void Awake()
    {
        // Thiết lập Singleton
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
        UpdateEnemyCountUI();
    }

    // Phương thức để tăng số lượng enemy tiêu diệt
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
        SceneManager.LoadScene("SampleScene"); 
    }
}
