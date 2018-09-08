using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
//using Firebase.Messaging;


namespace football_predictor
{
    public class StartUp : MonoBehaviour
    {

        public GameObject scene_transition_manager;

        public Text Welcome_Text;

        //If user not signed in then add option to sign up!

        // Once signed up unlock all other scenes...

        // If user signed in then give option to sign out
        // and other buttons to modify email password etc

        // When the app starts, check to make sure that we have
        // the required dependencies to use Firebase, and if not,
        // add them if possible.



        protected Firebase.Auth.FirebaseAuth auth;
        protected Firebase.Auth.FirebaseAuth otherAuth;
        protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
        new Dictionary<string, Firebase.Auth.FirebaseUser>();

        //public GUISkin fb_GUISkin;
        //private string logText = "";
        protected string email = "";
        protected string password = "";
        protected string displayName = "";
        protected string phoneNumber = "";
        protected string receivedCode = "";
        // Whether to sign in / link or reauthentication *and* fetch user profile data.
        protected bool signInAndFetchProfile = true;
        // Flag set when a token is being fetched.  This is used to avoid printing the token
        // in IdTokenChanged() when the user presses the get token button.
        private bool fetchingToken = false;
        // Enable / disable password input box.
        // NOTE: In some versions of Unity the password input box does not work in
        // iOS simulators.
        public bool usePasswordInput = false;
        //private Vector2 controlsScrollViewVector = Vector2.zero;
        //private Vector2 LogScrollViewVector = Vector2.zero;

        //private Vector2 scrollViewVector = Vector2.zero;
        //private Vector2 LogViewVector = Vector2.zero;

        // Set the phone authentication timeout to a minute.
        private uint phoneAuthTimeoutMs = 60 * 1000;
        // The verification id needed along with the sent code for phone authentication.
        private string phoneAuthVerificationId;

        // ALAN
        Dictionary<string, object> defaults = new Dictionary<string, object>();


        // Options used to setup secondary authentication object.
        private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions
        {
            ApiKey = "",
            AppId = "",
            ProjectId = ""
        };

        const int kMaxLogSize = 16382;
        Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

        // When the app starts, check to make sure that we have
        // the required dependencies to use Firebase, and if not,
        // add them if possible.
        public virtual void Start()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError(
                      "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
#if UNITY_EDITOR
            StartGame(); // FOR DESKTOP
#endif
        }

        // Handle initialization of the necessary firebase modules:
        protected void InitializeFirebase()
        {
            //Debug.Log("Setting up firebase cloud messaging (push notifications)");
            //Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            //Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

            Debug.Log("Setting up Firebase Auth");
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            auth.IdTokenChanged += IdTokenChanged;
            // needed for testing database in unity editor

            // Specify valid options to construct a secondary authentication object.
            if (otherAuthOptions != null &&
                !(String.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
                  String.IsNullOrEmpty(otherAuthOptions.AppId) ||
                  String.IsNullOrEmpty(otherAuthOptions.ProjectId)))
            {
                try
                {
                    otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(
                      otherAuthOptions, "Secondary"));
                    otherAuth.StateChanged += AuthStateChanged;
                    otherAuth.IdTokenChanged += IdTokenChanged;
                }
                catch (Exception)
                {
                    Debug.Log("ERROR: Failed to initialize secondary authentication object.");
                }
            }
            AuthStateChanged(this, null);
#if UNITY_EDITOR
            System.Threading.Tasks.Task.WhenAll(InitializeRemoteConfig());
#else
            // Initialise remote config
            Debug.Log("InitializeFirebaseComponents...(remote config)");
            System.Threading.Tasks.Task.WhenAll(
                InitializeRemoteConfig()
                ).ContinueWith(task => { StartGame(); });
#endif
        }

        //// For cloud messaging (push notifications)
        //public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        //{
        //    UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        //}
        //public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        //{
        //    UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
        //}


        void StartGame()
        {
            Firebase.AppOptions ops = new Firebase.AppOptions();
            CommonData.app = Firebase.FirebaseApp.Create(ops);
#if UNITY_EDITOR
            CommonData.app.SetEditorDatabaseUrl("https://unityfirebasefootballpredictor.firebaseio.com/");
            //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unityfirebasefootballpredictor.firebaseio.com/");
#endif
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            var user = auth.CurrentUser;
            if (user != null)
            {
                Debug.Log("user is signed in");
                Welcome_Text.text = "Welcome back " + user.DisplayName;
                CommonData.first_home_view = true;
                scene_transition_manager.GetComponent<scene_manager>().load_user_scene();
            }
            else
            {
                Debug.Log("user is not signed in");
                scene_transition_manager.GetComponent<scene_manager>().load_no_user_scene();
            }
        }

        //Sets the default values for remote config.These are the values that will
        //be used if we haven't fetched yet.
        System.Threading.Tasks.Task InitializeRemoteConfig()
        {
            // TODO CAN REMOVE

            // Adding first match as a test
            // Match ID and Team names and Time and Date
            defaults.Add("Match_1", "0001 ARSVMCI TIME1500 DATE02072018");
            defaults.Add("Match_2", "0001 BOUVBRH TIME1730 DATE02072018");
            defaults.Add("Match_3", "HDDVCHE TIME1500 DATE03072018");
            defaults.Add("Match_4", "WHUVWAT TIME1500 DATE03072018");
            defaults.Add("Match_5", "TOTVLIV TIME1500 DATE03072018");
            defaults.Add("Match_6", "MUNVSOU TIME1500 DATE03072018");
            defaults.Add("Match_7", "CRYVLEI TIME1500 DATE03072018");
            defaults.Add("Match_8", "LIVvNEW TIME1500 DATE03072018");
            defaults.Add("Match_9", "BOUvARS TIME1500 DATE03072018");
            defaults.Add("Match_10", "FULVWLV TIME1500 DATE03072018");

            defaults.Add("Match_11", "ARSVMCI TIME1500 DATE02072018");
            defaults.Add("Match_12", "BOUVBRH TIME1730 DATE02072018");
            defaults.Add("Match_13", "HDDVCHE TIME1500 DATE03072018");
            defaults.Add("Match_14", "WHUVWAT TIME1500 DATE03072018");
            defaults.Add("Match_15", "TOTVLIV TIME1500 DATE03072018");
            defaults.Add("Match_16", "MUNVSOU TIME1500 DATE03072018");
            defaults.Add("Match_17", "CRYVLEI TIME1500 DATE03072018");
            defaults.Add("Match_18", "LIVvNEW TIME1500 DATE03072018");
            defaults.Add("Match_19", "BOUvARS TIME1500 DATE03072018");
            defaults.Add("Match_20", "FULVWLV TIME1500 DATE03072018");

            defaults.Add("Match_21", "ARSVMCI TIME1500 DATE02072018");
            defaults.Add("Match_22", "BOUVBRH TIME1730 DATE02072018");
            defaults.Add("Match_23", "HDDVCHE TIME1500 DATE03072018");
            defaults.Add("Match_24", "WHUVWAT TIME1500 DATE03072018");
            defaults.Add("Match_25", "TOTVLIV TIME1500 DATE03072018");
            defaults.Add("Match_26", "MUNVSOU TIME1500 DATE03072018");
            defaults.Add("Match_27", "CRYVLEI TIME1500 DATE03072018");
            defaults.Add("Match_28", "LIVvNEW TIME1500 DATE03072018");
            defaults.Add("Match_29", "BOUvARS TIME1500 DATE03072018");
            defaults.Add("Match_30", "FULVWLV TIME1500 DATE03072018");

            defaults.Add("Match_31", "ARSVMCI TIME1500 DATE02072018");
            defaults.Add("Match_32", "BOUVBRH TIME1730 DATE02072018");
            defaults.Add("Match_33", "HDDVCHE TIME1500 DATE03072018");
            defaults.Add("Match_34", "WHUVWAT TIME1500 DATE03072018");
            defaults.Add("Match_35", "TOTVLIV TIME1500 DATE03072018");
            defaults.Add("Match_36", "MUNVSOU TIME1500 DATE03072018");
            defaults.Add("Match_37", "CRYVLEI TIME1500 DATE03072018");
            defaults.Add("Match_38", "LIVvNEW TIME1500 DATE03072018");
            defaults.Add("Match_39", "BOUvARS TIME1500 DATE03072018");
            defaults.Add("Match_40", "FULVWLV TIME1500 DATE03072018");

            Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
            return Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(System.TimeSpan.Zero);
        }

        // Exit if escape (or back, on mobile) is pressed.
        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        void OnDestroy()
        {
            auth.StateChanged -= AuthStateChanged;
            auth.IdTokenChanged -= IdTokenChanged;
            auth = null;
            if (otherAuth != null)
            {
                otherAuth.StateChanged -= AuthStateChanged;
                otherAuth.IdTokenChanged -= IdTokenChanged;
                otherAuth = null;
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

        // Track state changes of the auth object.
        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            Debug.Log("tracking auth changes");
            Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
            Firebase.Auth.FirebaseUser user = null;
            if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
            if (senderAuth == auth && senderAuth.CurrentUser != user)
            {
                bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
                if (!signedIn && user != null)
                {
                    Debug.Log("Signed out " + user.UserId);
                }
                user = senderAuth.CurrentUser;
                userByAuth[senderAuth.App.Name] = user;
                if (signedIn)
                {
                    Debug.Log("Signed in " + user.UserId);

                    displayName = user.DisplayName ?? "";
                    DisplayDetailedUserInfo(user, 1);
                }
            }
            Debug.Log("finished tracking auth changes");
        }

        // Track ID token changes.
        void IdTokenChanged(object sender, System.EventArgs eventArgs)
        {
            Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
            if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
            {
                senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
                  task => Debug.Log(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
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
            }
            return complete;
        }

        // Sign-in with an email and password.
        public Task SigninWithEmailAsync()
        {
            Debug.Log(String.Format("Attempting to sign in as {0}...", email));
            //DisableUI();
            if (signInAndFetchProfile)
            {
                // ALAN: greet the user
                Welcome_Text.text = "Welcome Back" + displayName;

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

        // This is functionally equivalent to the Signin() function.  However, it
        // illustrates the use of Credentials, which can be aquired from many
        // different sources of authentication.
        public Task SigninWithEmailCredentialAsync()
        {
            Debug.Log(String.Format("Attempting to sign in as {0}...", email));
            //DisableUI();
            if (signInAndFetchProfile)
            {
                return auth.SignInAndRetrieveDataWithCredentialAsync(
                  Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWith(
                   HandleSignInWithSignInResult);
            }
            else
            {
                return auth.SignInWithCredentialAsync(
                  Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWith(
                   HandleSignInWithUser);
            }
        }

        // Called when a sign-in without fetching profile data completes.
        void HandleSignInWithUser(Task<Firebase.Auth.FirebaseUser> task)
        {
            //EnableUI();
            if (LogTaskCompletion(task, "Sign-in"))
            {
                Debug.Log(String.Format("{0} signed in", task.Result.DisplayName));
            }
        }

        // Called when a sign-in with profile data completes.
        void HandleSignInWithSignInResult(Task<Firebase.Auth.SignInResult> task)
        {
            //EnableUI();
            if (LogTaskCompletion(task, "Sign-in"))
            {
                DisplaySignInResult(task.Result, 1);
            }
        }

        // Link the current user with an email / password credential.
        protected Task LinkWithEmailCredentialAsync()
        {
            if (auth.CurrentUser == null)
            {
                Debug.Log("Not signed in, unable to link credential to user.");
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(new Exception("Not signed in"));
                return tcs.Task;
            }
            Debug.Log("Attempting to link credential to user...");
            Firebase.Auth.Credential cred =
              Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
            if (signInAndFetchProfile)
            {
                return auth.CurrentUser.LinkAndRetrieveDataWithCredentialAsync(cred).ContinueWith(
                  task => {
                      if (LogTaskCompletion(task, "Link Credential"))
                      {
                          DisplaySignInResult(task.Result, 1);
                      }
                  });
            }
            else
            {
                return auth.CurrentUser.LinkWithCredentialAsync(cred).ContinueWith(task => {
                    if (LogTaskCompletion(task, "Link Credential"))
                    {
                        DisplayDetailedUserInfo(task.Result, 1);
                    }
                });
            }
        }

        // Reauthenticate the user with the current email / password.
        protected Task ReauthenticateAsync()
        {
            var user = auth.CurrentUser;
            if (user == null)
            {
                Debug.Log("Not signed in, unable to reauthenticate user.");
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(new Exception("Not signed in"));
                return tcs.Task;
            }
            Debug.Log("Reauthenticating...");
            //DisableUI();
            Firebase.Auth.Credential cred = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
            if (signInAndFetchProfile)
            {
                return user.ReauthenticateAndRetrieveDataAsync(cred).ContinueWith(task => {
                    //EnableUI();
                    if (LogTaskCompletion(task, "Reauthentication"))
                    {
                        DisplaySignInResult(task.Result, 1);
                    }
                });
            }
            else
            {
                return user.ReauthenticateAsync(cred).ContinueWith(task => {
                    //EnableUI();
                    if (LogTaskCompletion(task, "Reauthentication"))
                    {
                        DisplayDetailedUserInfo(auth.CurrentUser, 1);
                    }
                });
            }
        }

        // Reload the currently logged in user.
        public void ReloadUser()
        {
            if (auth.CurrentUser == null)
            {
                Debug.Log("Not signed in, unable to reload user.");
                return;
            }
            Debug.Log("Reload User Data");
            auth.CurrentUser.ReloadAsync().ContinueWith(task => {
                if (LogTaskCompletion(task, "Reload"))
                {
                    DisplayDetailedUserInfo(auth.CurrentUser, 1);
                }
            });
        }

        // Fetch and display current user's auth token.
        public void GetUserToken()
        {
            if (auth.CurrentUser == null)
            {
                Debug.Log("Not signed in, unable to get token.");
                return;
            }
            Debug.Log("Fetching user token");
            fetchingToken = true;
            auth.CurrentUser.TokenAsync(false).ContinueWith(task => {
                fetchingToken = false;
                if (LogTaskCompletion(task, "User token fetch"))
                {
                    Debug.Log("Token = " + task.Result);
                }
            });
        }

        // Display information about the currently logged in user.
        void GetUserInfo()
        {
            if (auth.CurrentUser == null)
            {
                Debug.Log("Not signed in, unable to get info.");
            }
            else
            {
                Debug.Log("Current user info:");
                DisplayDetailedUserInfo(auth.CurrentUser, 1);
            }
        }

    }
}
