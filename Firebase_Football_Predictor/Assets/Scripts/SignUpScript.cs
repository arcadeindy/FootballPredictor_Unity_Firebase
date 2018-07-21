using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SignUpScript : MonoBehaviour {

    public GameObject scene_transition_manager;

    protected Firebase.Auth.FirebaseAuth auth;

    public InputField email_input;
    public InputField password_input;
    public InputField display_name_input;
    protected string email = "";
    protected string password = "";
    protected string displayName = "";

    // Buttons will be done in the inspector

    // Use this for initialization
    void Start ()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void Initialize()
    {
        
    }

    public void SignUpButtonPressed()
    {
        email = "demofirebase2" + Time.time + "@gmail.com";// email_input.text;
        password = "abcdefgh";  //password_input.text;
        displayName = "Mr Happy"; // display_name_input.text;

        CreateUserWithEmailAsync();


    }

    public void IsSignUpComplete()
    {
        // If sucessful go to welcome page
        var user = auth.CurrentUser;
        if (user != null)
        {
            print("user is signed in");
            scene_transition_manager.GetComponent<scene_manager>().load_welcome_scene();
        }
        else
        {
            print("user is not signed in");
            // Return why?
        }
    }


    // Create a user with the email and password.
    public Task CreateUserWithEmailAsync()
    {
        Debug.Log(String.Format("Attempting to create user {0}...", email));
        //DisableUI();

        // This passes the current displayName through to HandleCreateUserAsync
        // so that it can be passed to UpdateUserProfile().  displayName will be
        // reset by AuthStateChanged() when the new user is created and signed in.
        string newDisplayName = displayName;
        return auth.CreateUserWithEmailAndPasswordAsync(email, password)
          .ContinueWith((task) => {
              //EnableUI();
              if (LogTaskCompletion(task, "User Creation"))
              {
                  var user = task.Result;
                  DisplayDetailedUserInfo(user, 1);
                  return UpdateUserProfileAsync(newDisplayName: newDisplayName);
              }
              return task;
          }).Unwrap();
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
        //DisableUI();
        return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
        {
            DisplayName = displayName,
            PhotoUrl = auth.CurrentUser.PhotoUrl,
        }).ContinueWith(task => {
            //EnableUI();
            if (LogTaskCompletion(task, "User profile"))
            {
                DisplayDetailedUserInfo(auth.CurrentUser, 1);
                IsSignUpComplete();
            }
        });
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
        }
        return complete;
    }

    // Display user information.
    protected void DisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        var userProperties = new Dictionary<string, string> {
      {"Display Name", userInfo.DisplayName},
      {"Email", userInfo.Email},
      {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
      {"Provider ID", userInfo.ProviderId},
      {"User ID", userInfo.UserId}
    };
        foreach (var property in userProperties)
        {
            if (!String.IsNullOrEmpty(property.Value))
            {
                Debug.Log(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
            }
        }
    }

    // Display a more detailed view of a FirebaseUser.
    protected void DisplayDetailedUserInfo(Firebase.Auth.FirebaseUser user, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        DisplayUserInfo(user, indentLevel);
        Debug.Log(String.Format("{0}Anonymous: {1}", indent, user.IsAnonymous));
        Debug.Log(String.Format("{0}Email Verified: {1}", indent, user.IsEmailVerified));
        Debug.Log(String.Format("{0}Phone Number: {1}", indent, user.PhoneNumber));
        var providerDataList = new List<Firebase.Auth.IUserInfo>(user.ProviderData);
        var numberOfProviders = providerDataList.Count;
        if (numberOfProviders > 0)
        {
            for (int i = 0; i < numberOfProviders; ++i)
            {
                Debug.Log(String.Format("{0}Provider Data: {1}", indent, i));
                DisplayUserInfo(providerDataList[i], indentLevel + 2);
            }
        }
    }


}
