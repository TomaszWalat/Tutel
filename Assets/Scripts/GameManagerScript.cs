using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    void Awake()
    {
        // Find any all objects tagged with GameManager
        GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");

        if (objects.Length > 1)
        {
            // If one already exists, destroy self (as you're the duplicate)
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
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
