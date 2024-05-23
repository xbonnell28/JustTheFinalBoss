using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
}
