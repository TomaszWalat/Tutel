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
    bool isToggle;

    [SerializeField]
    bool isOneShot;

    [SerializeField]
    bool hasTimer;

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

        //modelDeactiveted.SetActive(false);
        //modelActiveted.SetActive(true);

        ButtonModelPressed(true);

        goalObjectScript.SetGoalState(true);

        StartCoroutine(StartTimer(timeDelay));
    }

    private void Deactivate()
    {
        //modelDeactiveted.SetActive(true);
        //modelActiveted.SetActive(false);

        ButtonModelPressed(false);

        goalObjectScript.SetGoalState(false);

        IsActive = false;
    }

    private void ButtonModelPressed(bool state)
    {
        modelActiveted.SetActive(state);
        modelDeactiveted.SetActive(!state);
    }

    IEnumerator StartTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (!isOneShot)
        {
            Deactivate();
        }
        else
        {
            ButtonModelPressed(true);
        }
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
        Debug.Log("Button active");
        //}
    }
}
