using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleManager : MonoBehaviour
{
    [Tooltip("Please do not manually add anything to this.")]
    public List<Transform> turtles;
    [Tooltip("Index of the currently active turtle\nmostly follows Hierarchy order.")]
    public int currentlyActive = 0;


    private Transform currentTurtle;
    private int turtleCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        //populate the turtles array
        foreach(Transform child in transform)
        {
            currentTurtle = child; //we do not know if this is a player yet.
            if(child.GetComponent<TurtleController>())
            {
                Debug.Log("found a turtle");
                turtles.Add(child);
                turtleCount++;
                if (child.GetComponent<Ability>() != null)
                {
                    Debug.Log("found ability" + child.GetComponent<Ability>());
                    DisablePlayer(child);
                }
            }
        }
        currentTurtle = null;

        // enable the first turtle, so we can play the game.
        EnablePlayer(turtles[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Swap Button"))
        {
            if (turtleCount - 1 == currentlyActive) // we are at the last turtle in the queue
            {
                EnablePlayer(turtles[0]); // we enable the first turle, and disable all others
                for (int i = 1; i < turtleCount; i++)
                {
                    DisablePlayer(turtles[i]);
                }
                currentlyActive = 0;
            }
            else
            {
                for (int i = 0; i < turtleCount; i++) // go though all the turtles, and disable their ability, except the new index.
                {
                    if(i == currentlyActive + 1) // we are at the new turtle to be controlled    this does not work if we are at the end of the string
                    {
                        EnablePlayer(turtles[i]);
                    }
                    else // we are not on the new index
                    {
                        DisablePlayer(turtles[i]);
                    }
                }
                currentlyActive += 1;
            }
        }
        
    }

    private void DisablePlayer(Transform player)
    {
        if (player.TryGetComponent<Ability>(out Ability ability))
        {
            ability.enabled = false;
        }
        player.GetComponent<TurtleController>().currentlyActive = false;
    }

    private void EnablePlayer(Transform player)
    {
        if (player.TryGetComponent<Ability>(out Ability ability))
        {
            ability.enabled = true;
        }
        player.GetComponent<TurtleController>().currentlyActive = true;
    }
}
