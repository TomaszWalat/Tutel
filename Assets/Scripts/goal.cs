using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goal : MonoBehaviour
{
    public SceneManagement manager;
    public int sceneNumber = 0;

    //if the block hits it it loads the scene
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "pushBlock")
        {
            manager.LoadScene(sceneNumber);
        }

    }
}
