using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWin : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource buttonSource;
    public AudioClip buttonClip;
    public AudioSource gamewinSource;
    public AudioClip gamewinClip;

    public void LoadMenu()
    {

        buttonSource.Play();
        SceneManager.LoadScene("Menu");


    }
    void Start()
    {
        buttonSource.clip = buttonClip;
        gamewinSource.clip = gamewinClip;
        gamewinSource.Play();

    }
}
