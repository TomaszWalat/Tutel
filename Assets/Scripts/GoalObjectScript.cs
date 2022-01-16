using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalObjectScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Fill in only if this a child of another goal (part of a daisy chain) - i.e. leave empty for Level Manager")]
    // Parent of this goal, null is this is the higest level parent
    protected GoalObjectScript parent;

    [SerializeField]
    // Goal completion status, if this has sub-goals, they must all be complete
    protected bool isComplete;

    // Set of children names and their goal scripts
    protected Dictionary<string, GoalObjectScript> children;

    // Set of children names and their goal states
    protected Dictionary<string, bool> progress;

    void Awake()
    {
        children = new Dictionary<string, GoalObjectScript>();
        progress = new Dictionary<string, bool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(parent != null)
        {
            parent.AttachChild(this.gameObject);
        }
    }


    public bool SetParent(GameObject newParent)
    {
        bool success = false;

        if (newParent != null)
        {
            if (newParent.TryGetComponent(out GoalObjectScript goScript))
            {
                bool oldParentDetached = true;

                if (parent != null)
                {
                    oldParentDetached = parent.DetachChild(this.gameObject);
                }

                if(oldParentDetached)
                {
                    parent = goScript;

                    parent.AttachChild(this.gameObject);

                    success = true;
                }
            }
        }
        else
        {
            if (parent != null)
            {
                parent.DetachChild(this.gameObject);

                parent = null;

                success = true;
            }
        }

        return success;
    }

    public GameObject GetParent()
    {
        return parent.gameObject;
    }

    public bool GetGoalState()
    {
        return isComplete;
    }

    // Only works if goal is not a parent
    public void SetGoalState(bool complete)
    {
        if(children.Count <= 0)
        {
            isComplete = complete;

            CheckProgress();
        }
    }

    public bool AttachChild(GameObject child)
    {
        bool attached = false;

        if (!children.ContainsKey(child.name))
        {
            if (child.TryGetComponent(out GoalObjectScript goScript))
            {
                children.Add(child.name, goScript);
                progress.Add(child.name, goScript.GetGoalState());

                attached = true;

                CheckProgress();
            }
        }

        return attached;
    }

    public bool DetachChild(GameObject child)
    {
        bool detached = false;

        if(children.ContainsKey(child.name))
        { 
            progress.Remove(child.name);
            children.Remove(child.name);

            detached = true;

            CheckProgress();
        }

        return detached;
    }

    protected async void CheckProgress()
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
        }

        if(parent != null)
        {
            parent.UpdateChildStatus(this.gameObject);
        }
    }

    public async void UpdateChildStatus(GameObject child)
    {
        if (children.TryGetValue(child.name, out GoalObjectScript goScript))
        {
            progress[child.name] = goScript.GetGoalState();
        }
    }
}
