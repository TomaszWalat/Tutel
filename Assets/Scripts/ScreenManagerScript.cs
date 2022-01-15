using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManagerScript : MonoBehaviour
{
    // Script to act as a switchboard between the game manager, level manager, and ui manager

    //[SerializeField]
    //GameManagerScript gameManager;

    [SerializeField]
    LevelManagerScript levelManager;

    [SerializeField]
    UIManagerScript uiManager;

    [SerializeField]
    bool isLevel;

    [SerializeField]
    bool hasUI;

    void Awake()
    {
        if(levelManager != null)
        {
            isLevel = true;
        }
        if(uiManager != null)
        {
            hasUI = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
