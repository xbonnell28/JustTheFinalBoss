using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MenuManager
{
    public GameObject mainMenu;
    public Stack<GameObject> menuStack = new Stack<GameObject>();

    private void Awake()
    {
        menuStack.Push(mainMenu);
    }

    private void Update()
    {
        if(Input.GetButtonDown("Cancel") && menuStack.Count > 1)
        {
            GameObject currentMenu = menuStack.Pop();
            GameObject previousMenu = menuStack.Peek();
            currentMenu.SetActive(false);
            previousMenu.SetActive(true);
        }
        else
        {
            // If only one menu left then it's the main menu
            // We can then prompt the player if they 'Really Want to Quit?'
        }
    }

    public void queueMenu(GameObject menu)
    {
        menuStack.Push(menu);
    }
}
