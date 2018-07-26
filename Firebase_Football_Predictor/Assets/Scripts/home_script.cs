using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class home_script : MonoBehaviour {

    public GameObject scene_transition_manager;

    protected Firebase.Auth.FirebaseAuth auth;

    public Text Text_field;

    public void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        welcome_user();
    }



    void welcome_user()
    {
        var user = auth.CurrentUser;
        if (user != null)
        {
            Text_field.text = "Welcome home " + user.DisplayName;
        }
        else
        {
            print("user is not signed in");
            scene_transition_manager.GetComponent<scene_manager>().load_signup_scene();
        }

    }

}