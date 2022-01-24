using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerScript : GoalObjectScript
{
    // Script for level/gameplay related stuff

    public enum LevelState { Loading, Ready, InProgress, Paused, Failed, Complete }

    [SerializeField]
    ScreenManagerScript screenManager;

    [SerializeField]
    LevelState state = LevelState.Loading;

    bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Load());
        isPaused = false;
       // state = LevelState.Loading;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //if (state == LevelState.InProgress || state == LevelState.Ready)
            //{
            //    state = LevelState.Paused;
            //    screenManager.GoToMenu("PauseMenu");
            //}
            //else if(state == LevelState.Paused)
            //{
            //    state = LevelState.InProgress;
            //    screenManager.GoToMenu("ResumeLevel");
            //} 
            if (!isPaused)
            {
                isPaused = true;
                screenManager.GoToMenu("PauseMenu");
            }
            else if(isPaused)
            {
                screenManager.GoToMenu("ResumeLevel");
                isPaused = false;
            }
        }
    }

    public string GetState()
    {
        string sState = "";

        switch (state)
        {
            case LevelState.Loading:
                sState = "Loading";
                break;
            case LevelState.Ready:
                sState = "Ready";
                break;
            case LevelState.InProgress:
                sState = "InProgress";
                break;
            case LevelState.Paused:
                sState = "Paused";
                break;
            case LevelState.Complete:
                sState = "Complete";
                break;
            default: 
                break;
        }
        return sState;
    }

    IEnumerator Load()
    {
        // Initialise everything with the level here
        // ----

        yield return new WaitForEndOfFrame();

        state = LevelState.Ready;
    }


    // ---------- Methods from GoalObjectScript ---------- //

    // Only works if goal is not a parent
    new async void SetGoalState(bool complete)
    {
        if (children.Count <= 0)
        {
            isComplete = complete;

            state = LevelState.Complete;

            CheckProgress();
        }
    }

    new async void CheckProgress()
    {
        bool childrenComplete = true;

        // Check children status
        if (children.Count > 0)
        {
            Dictionary<string, bool>.ValueCollection values = progress.Values;

            // If any children are false, childrenComplete will be false
            foreach (bool childState in values)
            {
                childrenComplete = childrenComplete && childState;
            }

            isComplete = childrenComplete;

            state = LevelState.Complete;

            screenManager.GoToMenu("VictoryMenu");
        }
    }

    public void TriggerFailState()
    {
        state = LevelState.Failed;

        screenManager.GoToMenu("FailStateMenu");
    }
}
