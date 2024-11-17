using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public AudioSource clickButtonSound;
    public AudioClip playClip;
 
    void Start()
    {
        clickButtonSound.clip = playClip;
    }
    public void LoadGame()
    {
        clickButtonSound.Play();
        SceneManager.LoadScene("SampleScene");

    }
}
