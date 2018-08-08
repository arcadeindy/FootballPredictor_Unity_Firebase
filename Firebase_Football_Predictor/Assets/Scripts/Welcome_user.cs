using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Welcome_user : MonoBehaviour
{

    public GameObject scene_transition_manager;

    protected Firebase.Auth.FirebaseAuth auth;

    public Text Welcome_Text;

    public Text user_points;

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
            print("user is signed in. Email" + user.Email);
            Welcome_Text.text = "Welcome back " + user.DisplayName;
        }
        else
        {
            print("user is not signed in");
            scene_transition_manager.GetComponent<scene_manager>().load_no_user_scene();
        }

    }

}