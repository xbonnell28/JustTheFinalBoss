using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] UiManager uiManager;
    [SerializeField] GameObject pauseMenu;
    private bool isPaused = false;
    private void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if(isPaused)
            {
                isPaused = false;
                pauseMenu.SetActive(isPaused);
                Time.timeScale = 1.0f;
            } else
            {
                isPaused = true;
                pauseMenu.SetActive(isPaused);
                Time.timeScale = 0f;
            }
        }
    }

    public void updateCharacterHealth(float health)
    {
        uiManager.updateHealthText(health);
        if (health <= 0)
        {
            SceneManager.LoadScene("Death Screen");
        }
    }
}
