using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyMusicManagerScript : MonoBehaviour
{
    void Awake()
    {
        GameObject A = GameObject.FindGameObjectWithTag("backgroundMusic");
        Destroy(A);
    }
}
