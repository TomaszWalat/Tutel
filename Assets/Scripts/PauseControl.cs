using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl
{
    public static bool isGamePause;

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void PauseGame()
    {
        isGamePause = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isGamePause = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public bool IsGamePaused()
    {
        return isGamePause;
    }
}
