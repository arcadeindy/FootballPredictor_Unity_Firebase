using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

namespace football_predictor
{
    public class SignUpScript : MonoBehaviour
    {

        public GameObject scene_transition_manager;

        public GameObject menu_bar_for_editor;

        protected Firebase.Auth.FirebaseAuth auth;

        private Firebase.FirebaseApp app;
        private Firebase.Database.FirebaseDatabase _user_database;


        public InputField email_input;
        public InputField password_input;
        public InputField display_name_input;
        protected string email = "";
        protected string password = "";
        protected string displayName = "";

        public Text error_text;

        // Buttons will be done in the inspector

        // Use this for initialization
        void Start()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
#if (UNITY_EDITOR)
            menu_bar_for_editor.SetActive(true);
#endif
        }

        public void SignUpButtonPressed()
        {
            //email = "demofirebase2" + Time.time + "@gmail.com";
            //password = "abcdefgh";
            //displayName = "Mr Happy";

            email = email_input.text;
            password = password_input.text;
            displayName = display_name_input.text;

            error_text.enabled = false;

            CreateUserWithEmailAsync();

        }

        public void IsSignUpComplete()
        {
            // If sucessful go to welcome page
            var user = auth.CurrentUser;
            if (user != null)
            {
                Debug.Log("user is signed in");
                scene_transition_manager.GetComponent<scene_manager>().load_user_scene();
            }
            else
            {
                print("user is not signed in");
            }
        }

        private void add_user_to_database(string user_name, string email_address)
        {

            Debug.Log("adding user to database");
            app = CommonData.app;
            _user_database = Firebase.Database.FirebaseDatabase.GetInstance(app);

            var user = auth.CurrentUser;
            var uid = user.UserId;

            _user_database.RootReference.Child("users").Child(uid).Child("user_name").SetRawJsonValueAsync(user_name);
            _user_database.RootReference.Child("users").Child(uid).Child("email").SetRawJsonValueAsync(email_address);
        }

        private bool check_if_user_name_in_use(string user_name)
        {
            bool is_user_name_taken = false;

            // Check realtime database for user name
            app = CommonData.app;
            Firebase.Database.FirebaseDatabase user_db = Firebase.Database.FirebaseDatabase.GetInstance(app);
            user_db.RootReference.Child("users").Child(user_name).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot user in snapshot.Children)
                    {
                        if ((string)user.Value == user_name)
                        {
                            is_user_name_taken = true;
                            return;
                        }
                    }
                    return;
                }
            });
            return is_user_name_taken;
        }

        // Create a user with the email and password.
        public Task CreateUserWithEmailAsync()
        {
            Debug.Log(String.Format("Attempting to create user {0}...", email));

            // Check if email and username already in use
            if (check_if_user_name_in_use(displayName))
            {
                Debug.Log("user name already taken!");
                // Print Error
                error_text.text = "User name already taken";
                error_text.enabled = true;
            }

            // This passes the current displayName through to HandleCreateUserAsync
            // so that it can be passed to UpdateUserProfile().  displayName will be
            // reset by AuthStateChanged() when the new user is created and signed in.
            string newDisplayName = displayName;
            return auth.CreateUserWithEmailAndPasswordAsync(email, password)
              .ContinueWith((task) =>
              {
                  if (task.IsCanceled)
                  {
                      Debug.LogError("User creation task was canceled.");
                      return;
                  }
                  if (task.IsFaulted)
                  {
                      Debug.LogError("User creation task encountered an error: " + task.Exception);
                      error_text.text = "ERROR: " + task.Exception;
                      error_text.enabled = true;
                      return;
                  }
                  Debug.Log("User reauthenticated successfully.");
                  add_user_to_database(email, password);
                  UpdateUserProfileAsync(newDisplayName: newDisplayName);
              });
        }

        // Update the user's display name with the currently selected display name.
        public Task UpdateUserProfileAsync(string newDisplayName = null)
        {
            if (auth.CurrentUser == null)
            {
                Debug.Log("Not signed in, unable to update user profile");
                return Task.FromResult(0);
            }
            displayName = newDisplayName ?? displayName;
            Debug.Log("Updating user profile");
            return auth.CurrentUser.UpdateUserProfileAsync
                (
                new Firebase.Auth.UserProfile
                {
                    DisplayName = displayName,
                    PhotoUrl = auth.CurrentUser.PhotoUrl,
                }).ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("UpdateUserProfileAsync task was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("UpdateUserProfileAsync task encountered an error: " + task.Exception);
                        return;
                    }
                    Debug.Log("UpdateUserProfileAsync successful.");
                    IsSignUpComplete();
                }
                );
        }

    }
}

