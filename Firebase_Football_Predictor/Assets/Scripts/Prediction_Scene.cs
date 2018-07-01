using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;

public class Prediction_Scene : MonoBehaviour {


    public GameObject _prediction_content;
    public GameObject _prediction_score_template;
    public GameObject _submit_prediction_button;


    Dictionary<string, object> defaults = new Dictionary<string, object>();

    // Use this for initialization
    void Start ()
    {
        //set_firebase_defaults();
        //get_fixtures_from_firebase();
        //add_prediciton_button();

        //Screen.SetResolution(Screen.width / 2, Screen.height / 2, true);
        //GooglePlayServicesSignIn.InitializeGooglePlayGames();
        InitializeFirebaseAndStart();
    }

    void StartGame()
    {
        // Remote Config data has been fetched, so this applies it for this play session:
        Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
        Firebase.AppOptions ops = new Firebase.AppOptions();
       // CommonData.app = Firebase.FirebaseApp.Create(ops);
      //  stateManager.PushState(new States.Startup());

        get_fixtures_from_firebase();
        add_prediciton_button();
    }

    // Update is called once per frame
    void Update () {
		
	}

    // Sets the default values for remote config.  These are the values that will
    // be used if we haven't fetched yet.
    void set_firebase_defaults()
    {
        defaults.Add("Match_1", "ARSVMCI TIME1500 DATE02072018");
        defaults.Add("Match_2", "BOUVBRH TIME1730 DATE02072018");
        defaults.Add("Match_3", "HDDVCHE TIME1500 DATE03072018");
        defaults.Add("Match_4", "WHUVWAT TIME1500 DATE03072018");
        defaults.Add("Match_5", "TOTVLIV TIME1500 DATE03072018");
        defaults.Add("Match_6", "MUNVSOU TIME1500 DATE03072018");
        defaults.Add("Match_7", "CRYVLEI TIME1500 DATE03072018");
        defaults.Add("Match_8", "LIVvNEW TIME1500 DATE03072018");
        defaults.Add("Match_9", "BOUvARS TIME1500 DATE03072018");
        defaults.Add("Match_10", "FULVWLV TIME1500 DATE03072018");

        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
        Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(System.TimeSpan.Zero);
    }

    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    void InitializeFirebaseAndStart()
    {
        Firebase.DependencyStatus dependencyStatus = Firebase.FirebaseApp.CheckDependencies();

        if (dependencyStatus != Firebase.DependencyStatus.Available)
        {
            Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
                dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    InitializeFirebaseComponents();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                    Application.Quit();
                }
            });
        }
        else
        {
            InitializeFirebaseComponents();
        }
    }

    void InitializeFirebaseComponents()
    {
        System.Threading.Tasks.Task.WhenAll(
            InitializeRemoteConfig()
          ).ContinueWith(task => { StartGame(); });
    }

    

    //Sets the default values for remote config.These are the values that will
    //be used if we haven't fetched yet.
    System.Threading.Tasks.Task InitializeRemoteConfig()
    {

        // Adding first match as a test
        // Match ID and Team names and Time and Date
        defaults.Add("Match_1", "ARSVMCI TIME1500 DATE02072018");
        defaults.Add("Match_2", "BOUVBRH TIME1730 DATE02072018");
        defaults.Add("Match_3", "HDDVCHE TIME1500 DATE03072018");
        defaults.Add("Match_4", "WHUVWAT TIME1500 DATE03072018");
        defaults.Add("Match_5", "TOTVLIV TIME1500 DATE03072018");
        defaults.Add("Match_6", "MUNVSOU TIME1500 DATE03072018");
        defaults.Add("Match_7", "CRYVLEI TIME1500 DATE03072018");
        defaults.Add("Match_8", "LIVvNEW TIME1500 DATE03072018");
        defaults.Add("Match_9", "BOUvARS TIME1500 DATE03072018");
        defaults.Add("Match_10", "FULVWLV TIME1500 DATE03072018");

        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
        return Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(System.TimeSpan.Zero);
    }

    void get_fixtures_from_firebase()
    {
        List<string> Fixtures = new List<string>();


        // Check firebase for updates
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_1").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_2").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_3").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_4").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_5").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_6").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_7").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_8").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_9").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_10").StringValue);


        // Set up scene
        foreach (string fixture in Fixtures)
        {
            print(fixture);
            // If in future and within next week TODO: next week will need to be looked at
            DateTime fix_date;
            fix_date = new DateTime(Int32.Parse(fixture.Substring(25,4)),  // year
                                    Int32.Parse(fixture.Substring(23,2)),  // month
                                    Int32.Parse(fixture.Substring(21,2)),  // day
                                    Int32.Parse(fixture.Substring(12,2)),  // hour
                                    Int32.Parse(fixture.Substring(14,2)),  // minute
                                    0);                                    // second
            print("now date: " + DateTime.Now.Day + "/" + DateTime.Now.Month);
            print("fix date: " + fix_date.Day + "/" + fix_date.Month);
            // Check that fixture is within 5 days
            if(DateTime.Compare(DateTime.Now.AddDays(150), fix_date) > 0)
            {
                print("PASSED CHECK ONE");
                // Check fixture is not in the past
                if(DateTime.Compare(DateTime.Now, fix_date.AddMinutes(-30)) < 0)
                {
                    print("PASSED CHECK TWO");
                    // If fixture not in past and is within x days than format in scene
                    create_fixture_UI(fix_date,
                                      fixture.Substring(0, 3),
                                      fixture.Substring(4, 3));
                }

            }
        }

    }

    private void create_fixture_UI(DateTime ko_date, String home_team, String away_team)
    {
        print("Creating fixture UI");

        // Make instance of prediction
        GameObject prediction_instance = Instantiate(_prediction_score_template);

        // Create prediction as child of scroll view content object
        prediction_instance.transform.parent = _prediction_content.transform;

        // Set team names
        prediction_instance.GetComponent<Prediction_button>().update_home_team_text(home_team);
        prediction_instance.GetComponent<Prediction_button>().update_away_team_text(away_team);
        //Set ko time and date
        prediction_instance.GetComponent<Prediction_button>().update_ko_time_text(ko_date);

    }

    private void add_prediciton_button()
    {
        GameObject prediction_button_instance;
        prediction_button_instance = Instantiate(_submit_prediction_button);
        prediction_button_instance.transform.parent = _prediction_content.transform;
    }



}
