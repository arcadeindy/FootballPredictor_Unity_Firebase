using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
}
