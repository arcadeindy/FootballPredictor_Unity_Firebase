﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scene_manager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void load_prediction_scene()
    {
        SceneManager.LoadScene("prediction_scene");
        // add an advertisement
    }

    public void load_user_scene()
    {
        SceneManager.LoadScene("user_scene");
        // add an advertisement
    }

    public void load_welcome_scene()
    {
        SceneManager.LoadScene("welcome_user_scene");
    }

    public void load_signup_scene()
    {
        SceneManager.LoadScene("sign_up_scene");
    }

    public void load_startup_scene()
    {
        // This scene should not be called as it will reset firebase
        SceneManager.LoadScene("startup_scene");

    }
}
