using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class password_reset : MonoBehaviour
{

    public GameObject scene_transition_manager;

    public InputField email_input;

    public Text log_text;

    protected Firebase.Auth.FirebaseAuth auth;

    // Use this for initialization
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }


    public void send_password_reset_email()
    {
        var user = auth.CurrentUser;

        string emailAddress = email_input.text;

        Debug.Log("password reset email asked for");

        //if (user != null)
        if (1==1)
            {
                auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("SendPasswordResetEmailAsync was canceled.");
                    log_text.color = Color.red;
                    log_text.text = "Password reset was canceled - check email adress is valid";
                    log_text.enabled = true;
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.Log("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    log_text.color = Color.red;
                    log_text.text = "Password reset encountered an error: " + task.Exception + "Either sign up or check email is valid";
                    log_text.enabled = true;
                    return;
                }

                Debug.Log("Password reset email sent successfully.");
                log_text.color = Color.green;
                log_text.text = "Password reset email sent successfully!";
                log_text.enabled = true;
            });
        }
    }
}
