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

    [SerializeField]
    GameObject modelActiveted;
    [SerializeField]
    GameObject modelDeactiveted;

    [SerializeField]
    buttonFX buttonSound;

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
        buttonSound.ClickSound();
        
        modelDeactiveted.SetActive(false);
        modelActiveted.SetActive(true);

        goalObjectScript.SetGoalState(true);

        StartCoroutine(StartTimer(timeDelay));
    }

    private void Deactivate()
    {
        modelDeactiveted.SetActive(true);
        modelActiveted.SetActive(false);

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

    private void OnTriggerEnter(Collider other)
    {
        //if(other.tag == "")
        //{
        if (!isActive)
        {
            Interact();
        }
        Debug.Log("BUtton active");
        //}
    }
}
