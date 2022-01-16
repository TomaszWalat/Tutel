using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerScript : GoalObjectScript
{
    // Script for level/gameplay related stuff

    private enum LevelState { Loading, Ready, InProgress, Paused, Complete }

    [SerializeField]
    LevelState state = LevelState.Loading;

    [SerializeField]
    bool levelLoaded;


    [SerializeField]
    bool levelComplete;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
    }
}
