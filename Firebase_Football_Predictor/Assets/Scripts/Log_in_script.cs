using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Log_in_script : MonoBehaviour {

    public GameObject scene_transition_manager;

    protected Firebase.Auth.FirebaseAuth auth;

    public InputField email_input;
    public InputField password_input;
    //public InputField display_name_input;
    protected string email = "";
    protected string password = "";
    //protected string displayName = "";

    // Whether to sign in / link or reauthentication *and* fetch user profile data.
    protected bool signInAndFetchProfile = false;

    public Text error_text;

    // Use this for initialization
    void Start ()
    {

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;


    }

    public void LogInButtonPressed()
    {
        email = email_input.text;
        password = password_input.text;

        error_text.enabled = false;

        SigninWithEmailAsync();
    }

    public void IsSignInComplete()
    {
        // If sucessful go to welcome page
        var user = auth.CurrentUser;
        if (user != null)
        {
            Debug.Log("user is signed in");
            error_text.color = Color.green;
            error_text.text = "Log in sucessful!";
            error_text.enabled = true;
            scene_transition_manager.GetComponent<scene_manager>().load_user_scene();
        }
        else
        {
            Debug.Log("user is not signed in");
            error_text.text = "Log in unsucessful!";
            error_text.enabled = true;
        }
    }

    // Sign-in with an email and password.
    public Task SigninWithEmailAsync()
    {
        Debug.Log(String.Format("Attempting to sign in as {0}...", email));
        if (signInAndFetchProfile)
        {
            return auth.SignInAndRetrieveDataWithCredentialAsync(
              Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWith(
                HandleSignInWithSignInResult);
        }
        else
        {
            return auth.SignInWithEmailAndPasswordAsync(email, password)
              .ContinueWith(HandleSignInWithUser);
        }
    }

    // Called when a sign-in with profile data completes.
    void HandleSignInWithSignInResult(Task<Firebase.Auth.SignInResult> task)
    {
        if (LogTaskCompletion(task, "Sign-in"))
        {
            DisplaySignInResult(task.Result, 1);
        }
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(Task task, string operation)
    {
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
                error_text.text = "Error" + exception.ToString();
                error_text.enabled = true;
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log(operation + " completed");
        }
        return task.IsCompleted;// complete;
    }

    // Display user information reported
    protected void DisplaySignInResult(Firebase.Auth.SignInResult result, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        DisplayDetailedUserInfo(result.User, indentLevel);
        var metadata = result.Meta;
        if (metadata != null)
        {
            Debug.Log(String.Format("{0}Created: {1}", indent, metadata.CreationTimestamp));
            Debug.Log(String.Format("{0}Last Sign-in: {1}", indent, metadata.LastSignInTimestamp));
        }
        var info = result.Info;
        if (info != null)
        {
            Debug.Log(String.Format("{0}Additional User Info:", indent));
            Debug.Log(String.Format("{0}  User Name: {1}", indent, info.UserName));
            Debug.Log(String.Format("{0}  Provider ID: {1}", indent, info.ProviderId));
            DisplayProfile<string>(info.Profile, indentLevel + 1);
        }
        IsSignInComplete();
    }

    // Called when a sign-in without fetching profile data completes.
    void HandleSignInWithUser(Task<Firebase.Auth.FirebaseUser> task)
    {
        IsSignInComplete();
        //if (LogTaskCompletion(task, "Sign-in"))
        //{
        //    Debug.Log(String.Format("{0} signed in", task.Result.DisplayName));
        //}
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

    // Display additional user profile information.
    protected void DisplayProfile<T>(IDictionary<T, object> profile, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        foreach (var kv in profile)
        {
            var valueDictionary = kv.Value as IDictionary<object, object>;
            if (valueDictionary != null)
            {
                Debug.Log(String.Format("{0}{1}:", indent, kv.Key));
                DisplayProfile<object>(valueDictionary, indentLevel + 1);
            }
            else
            {
                Debug.Log(String.Format("{0}{1}: {2}", indent, kv.Key, kv.Value));
            }
        }
    }

}
