using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class delete_user_scene : MonoBehaviour
{

    public GameObject scene_transition_manager;

    public Text name_text;

    protected Firebase.Auth.FirebaseAuth auth;

    public InputField email_input;
    public InputField password_input;

    Firebase.Auth.FirebaseUser user;

    public void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        name_text.text = user.DisplayName;
    }

    public void DeleteButtonPressed()
    {
        //DeleteUserAsync();
        delete_firebase_user();
    }

    private void delete_firebase_user()
    {
        // Reauthenticate
        Firebase.Auth.Credential credential = Firebase.Auth.EmailAuthProvider.GetCredential(email_input.text, password_input.text);
        if (user != null)
        {
            user.ReauthenticateAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("ReauthenticateAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("ReauthenticateAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User reauthenticated successfully.");
            });
        }

        // Delete user
        if (user != null)
        {
            user.DeleteAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Delete user was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("Delete User encountered an error: " + task.Exception);
                    return;
                }
                Debug.Log("User deleted sucessfully");
                scene_transition_manager.GetComponent<scene_manager>().load_no_user_scene();

            });
        }

        //auth.CurrentUser.DeleteAsync();
    }

    // Delete the currently logged in user.
    protected Task DeleteUserAsync()
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log(String.Format("Attempting to delete user {0}...", auth.CurrentUser.UserId));
            return auth.CurrentUser.DeleteAsync().ContinueWith(task => {
                LogTaskCompletion(task, "Delete user");
            });
        }
        else
        {
            Debug.Log("Sign-in before deleting user.");
            // Return a finished task.
            return Task.FromResult(0);
        }
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            Debug.Log(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.Log(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                }
                Debug.Log(authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log(operation + " completed");
            complete = true;
            //ALAN:
            scene_transition_manager.GetComponent<scene_manager>().load_no_user_scene();
        }
        return complete;
    }
}