using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerScript : MonoBehaviour
{
    // Script to manage which UI canvas is displayed

    [SerializeField]
    string defaultUI;

    private Dictionary<string, CanvasObjectScript> uis;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AttachUI(GameObject ui)
    {
        bool attached = false;

        if (!uis.ContainsKey(ui.name))
        {
            if(ui.TryGetComponent(out CanvasObjectScript coScript))
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

        if(uis.ContainsKey(ui.name))
        {
            uis.Remove(ui.name);

            detached = true;
        }

        return detached;
    }
}
