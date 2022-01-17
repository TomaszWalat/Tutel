using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManagerScript : MonoBehaviour
{
    // Script to act as a switchboard between the game manager, level manager, and ui manager

    [SerializeField]
    GameManagerScript gameManager;

    [SerializeField]
    LevelManagerScript levelManager;

    private Dictionary<string, CanvasObjectScript> uis;

    [SerializeField]
    bool isLevel;

    [SerializeField]
    string sceneName;

    [SerializeField]
    CanvasObjectScript startingUI;

    [SerializeField]
    CanvasObjectScript activeUI;

    PauseControl pauseControl;

    void Awake()
    {
        uis = new Dictionary<string, CanvasObjectScript>();

        if (levelManager != null)
        {
            isLevel = true;
        }

        pauseControl = new PauseControl();
    }

    // Start is called before the first frame update
    void Start()
    {
        startingUI.EnableUI();
        StartCoroutine(Load());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Load()
    {
        while(gameManager == null)
        {
            yield return new WaitForFixedUpdate(); 
        }

        sceneName = gameManager.GetCurrentSceneName();
    }

    public void GoToMenu(string ui)
    {
        if (gameManager != null)
        {
            switch (ui)
            {
                case "MainMenuScene":
                    gameManager.GoToScene("MainMenuScene");
                    break;

                case "LevelsScene":
                    gameManager.GoToScene("LevelsScene");
                    break;

                case "SettingsScene":
                    gameManager.GoToScene("SettingsScene");
                    break;

                case "Level_tutorial_1":
                    gameManager.GoToScene("Level_tutorial_1");
                    break;

                case "Level_tutorial_2":
                    //gameManager.GoToScene("Level_tutorial_2");
                    break;

                case "Level_tutorial_3":
                    //gameManager.GoToScene("Level_tutorial_3");
                    break;

                case "Level_tutorial_4":
                    //gameManager.GoToScene("Level_tutorial_4");
                    break;

                case "PauseMenu":
                    if (!pauseControl.IsGamePaused())
                    {
                        PauseLevel();
                        DisableUI("SettingsMenu");
                        EnableUI("PauseMenu");
                    }
                    break;

                case "SettingsMenu":
                    DisableUI("PauseMenu");
                    EnableUI("SettingsMenu");
                    break;

                case "VictoryMenu":
                    PauseLevel();
                    EnableUI("VictoryMenu");
                    break;

                case "FailStateMenu":
                    PauseLevel();
                    EnableUI("FailStateMenu");
                    break;

                case "ResumeLevel":
                    DisableUI("PauseMenu");
                    ResumeLevel();
                    break;

                case "RestartLevel":
                    gameManager.GoToScene(sceneName);
                    break;

                case "NextLevel":
                    // For now use the scene name directly
                    break;

                case "QuitGame":
                    gameManager.QuitGame();
                    break;

                case "ExitPreloadScene":
                    gameManager.ExitPreloadScene();
                    break;

                case "": // Copy and paste dummy

                    break;

                default:
                    break;
            }
        }
    }

    private void EnableUI(string uiName)
    {
        if(uis.TryGetValue(uiName, out CanvasObjectScript coScript))
        {
            coScript.EnableUI();
        }
    }

    private void DisableUI(string uiName)
    {
        if(uis.TryGetValue(uiName, out CanvasObjectScript coScript))
        {
            coScript.DisableUI();
        }
    }

    public void PauseLevel()
    {
        pauseControl.PauseGame();
    }

    public void ResumeLevel()
    {
        pauseControl.ResumeGame();
    }

    //public void RequestUI(MenuUI ui)
    //{
    //    //switch(ui)
    //    //{
    //    //    case MenuUI.MainMenuScene:
    //    //        //
    //    //        break;
    //    //    case MenuUI.LevelsScene:
    //    //        //
    //    //        break;
    //    //    case MenuUI.SettingsScene:
    //    //        //
    //    //        break;
    //    //    default:
    //    //        break;
    //    //}
    //}

    public bool AttachUI(GameObject ui)
    {
        bool attached = false;

        if (!uis.ContainsKey(ui.name))
        {
            if (ui.TryGetComponent(out CanvasObjectScript coScript))
            {
                uis.Add(ui.name, coScript);

                attached = true;
            }
        }

        return attached;
    }

    public bool DetachUI(GameObject ui)
    {
        bool detached = false;

        if (uis.ContainsKey(ui.name))
        {
            uis.Remove(ui.name);

            detached = true;
        }

        return detached;
    }

    public bool AttachGameManager(GameManagerScript gmScript)
    {
        bool attached = false;

        if(gmScript != null)
        {
            gameManager = gmScript;
            attached = true;
        }

        return attached;
    }
}
