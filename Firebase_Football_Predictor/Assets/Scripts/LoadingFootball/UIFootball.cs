using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFootball : MonoBehaviour {


    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        // Rotate around z axis
        transform.Rotate(0f, 0f, 1f);
    }
}
