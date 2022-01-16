using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBlock : MonoBehaviour, IInteractable
{
    [SerializeField]
    GoalObjectScript goalObjectScript;

    [SerializeField]
    bool isActive;

    [SerializeField]
    float timeDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Activate()
    {
        goalObjectScript.SetGoalState(true);

        StartCoroutine(StartTimer(timeDelay));
    }

    private void Deactivate()
    {
        goalObjectScript.SetGoalState(false);

        IsActive = false;
    }

    IEnumerator StartTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Deactivate();
    }

    public bool Interact()
    {
        bool success = false;

        IsActive = true;

        Activate();

        return success;
    }

    public bool IsActive
    {
        get { return isActive; }

        private set { isActive = value; }
    }
}
