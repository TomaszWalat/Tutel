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

    void Awake()
    {
        uis = new Dictionary<string, CanvasObjectScript>();

        if (levelManager != null)
        {
            isLevel = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startingUI.EnableUI();
    }

    // Update is called once per frame
    void Update()
    {
        
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

                case "PauseMenu":

                    break;

                case "SettingsMenu":

                    break;

                case "VictoryMenu":

                    break;

                case "Level_tutorial_1":
                    gameManager.GoToScene("Level_tutorial_1");
                    break;

                case "Level_tutorial_2":
                    gameManager.GoToScene("Level_tutorial_2");
                    break;

                case "Level_tutorial_3":
                    gameManager.GoToScene("Level_tutorial_3");
                    break;

                case "Level_tutorial_4":
                    gameManager.GoToScene("Level_tutorial_4");
                    break;

                case "QuitApplication":
                    gameManager.QuitGame();
                    break;

                case "ExitPreloadScene":
                    gameManager.ExitPreloadScene();
                    break;

                case "":

                    break;

                default:
                    break;
            }
        }
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
