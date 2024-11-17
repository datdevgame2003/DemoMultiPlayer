using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
   
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI winText;
    public GameObject gameWinUI;
    private int enemyKillCount = 0; 
    private const int enemiesToWin = 25; 

    void Awake()
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

    void OnEnable()
    {
        
        EnemyController.OnEnemyKilled += HandleEnemyKilled;
    }

    void OnDisable()
    {
        
        EnemyController.OnEnemyKilled -= HandleEnemyKilled;
    }

   
    private void HandleEnemyKilled()
    {
        enemyKillCount++; 

        if (enemyKillCount >= enemiesToWin)
        {
            WinGame();
        }
    }

   
    private void WinGame()
    {
       
        gameWinUI.SetActive(true);
        winText.text = "Win!"; 

        
        Time.timeScale = 0;
    }

  
    public void RestartGame()
    {
        
        SceneManager.LoadScene("Menu"); 
    }
}


