using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace football_predictor
{
    public class home_script : MonoBehaviour {

        public GameObject scene_transition_manager;

        protected Firebase.Auth.FirebaseAuth auth;

        public Text Text_field;
        public Text user_points;

        public Text Delete_text_field;


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
                if (CommonData.first_home_view)
                {
                    Text_field.text = "Welcome back " + user.DisplayName;
                    CommonData.first_home_view = false;
                }
                else
                {
                    Text_field.text = "Welcome home " + user.DisplayName;
                }

                // Set last known user points
                user_points.text = " " + PlayerPrefs.GetInt("HighScore", 0).ToString() + " ";

            }
            else
            {
                print("user is not signed in");
                scene_transition_manager.GetComponent<scene_manager>().load_no_user_scene();
            }
        }

        // Sign out the current user
        public void sign_out()
        {
            Debug.Log("Signing out.");
            auth.SignOut();
            scene_transition_manager.GetComponent<scene_manager>().load_no_user_scene();
        }


        // Delete the currently logged in user.
        public void delete_user()
        {
            if (auth.CurrentUser != null)
            {
                auth.CurrentUser.DeleteAsync();
            }
            else
            {
                Debug.Log("Sign-in before deleting user.");
            }
        }
    }
}