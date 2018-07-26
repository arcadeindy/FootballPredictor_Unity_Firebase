using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;
using Firebase;
using Firebase.Unity.Editor;
using UnityEngine.UI;

namespace football_predictor
{
    public class Prediction_Scene : MonoBehaviour
    {
        public GameObject scene_transition_manager;

        public GameObject _prediction_content;
        public GameObject _prediction_score_template;
        public GameObject _submit_prediction_button;
        public GameObject _footer_button;

        // private DatabaseReference _prediction_database;
        private Firebase.Database.FirebaseDatabase _prediction_database;
        protected Firebase.Auth.FirebaseAuth auth;
        private Firebase.FirebaseApp app;


        // Use this for initialization
        void Start()
        {
            // needed for testing database in unity editor
            //#if (UNITY_EDITOR)

            //        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            //            var dependencyStatus = task.Result;
            //            if (dependencyStatus == Firebase.DependencyStatus.Available)
            //            {
            //                // Set a flag here indiciating that Firebase is ready to use by your
            //                // application.
            //                Debug.Log("firebase ready to use");
            //            }
            //            else
            //            {
            //                UnityEngine.Debug.LogError(System.String.Format(
            //                  "Coulds not resolve all Firebase dependencies: {0}", dependencyStatus));
            //                // Firebase Unity SDK is not safe to use here.
            //            }
            //        });

            //        Debug.Log("Setting up firebase database for editor");

            //        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unityfirebasefootballpredictor.firebaseio.com/");

            //#endif
            Debug.Log("STARTING PREDICTION");
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            Debug.Log("app: " + app);

            // Get the root reference location of the database.
            //_prediction_database = FirebaseDatabase.DefaultInstance.RootReference;

            // Starts the scene
            StartGame();

        }


        void StartGame()
        {
            Debug.Log("Starting game...(prediction scene)");

            // Remote Config data has been fetched, so this applies it for this play session:
            Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
            Firebase.AppOptions ops = new Firebase.AppOptions();

            get_fixtures_from_firebase();
            add_prediciton_button();
        }

        void get_fixtures_from_firebase()
        {
            Debug.Log("In: get_fixtures_from_firebase");

            List<string> Fixtures = new List<string>();
            List<fixture_class> C_Fixtures = new List<fixture_class>();

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

            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_11").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_12").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_13").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_14").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_15").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_16").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_17").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_18").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_19").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_20").StringValue);

            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_21").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_22").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_23").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_24").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_25").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_26").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_27").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_28").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_29").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_30").StringValue);

            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_31").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_32").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_33").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_34").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_35").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_36").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_37").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_38").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_39").StringValue);
            Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_40").StringValue);


            // Set up scene
            foreach (string fixture in Fixtures)
            {
                print(fixture);
                // If in future and within next week TODO: next week will need to be looked at
                DateTime fix_date;
                fix_date = new DateTime(Int32.Parse(fixture.Substring(30, 4)),  // year
                                        Int32.Parse(fixture.Substring(28, 2)),  // month
                                        Int32.Parse(fixture.Substring(26, 2)),  // day
                                        Int32.Parse(fixture.Substring(17, 2)),  // hour
                                        Int32.Parse(fixture.Substring(19, 2)),  // minute
                                        0);                                    // second
                                                                               // Check that fixture is within 5 days
                if (DateTime.Compare(DateTime.Now.AddDays(20), fix_date) > 0)
                {
                    // Check fixture is not in the past
                    if (DateTime.Compare(DateTime.Now, fix_date.AddMinutes(-30)) < 0)
                    {
                        fixture_class temp_fix = new fixture_class();
                        temp_fix.fixture_date = fix_date;
                        temp_fix.home_team = fixture.Substring(5, 3);
                        temp_fix.away_team = fixture.Substring(9, 3);
                        temp_fix.match_id = fixture.Substring(0, 4);
                        C_Fixtures.Add(temp_fix);
                    }

                }
            }

            // Sort fixtures by date
            C_Fixtures.Sort((x, y) => DateTime.Compare(x.fixture_date, y.fixture_date));

            //Debug.Log(" Going to make UI");
            // Create fixture UI
            foreach (fixture_class fix in C_Fixtures)
            {
                create_fixture_UI(fix.fixture_date,
                                  fix.home_team,
                                  fix.away_team,
                                  fix.match_id);
            }

        }

        private void create_fixture_UI(DateTime ko_date, String home_team, String away_team, String match_id)
        {
            //Debug.Log("Creating UI element");
            // Make instance of prediction
            GameObject prediction_instance = Instantiate(_prediction_score_template);

            // Create prediction as child of scroll view content object
            prediction_instance.transform.SetParent(_prediction_content.transform, false);

            // Add match id
            prediction_instance.GetComponent<Prediction_button>().match_id = int.Parse(match_id);

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
            prediction_button_instance.transform.SetParent(_prediction_content.transform, false);

            GameObject _footer_instance;
            _footer_instance = Instantiate(_footer_button);
            _footer_instance.transform.SetParent(_prediction_content.transform, false);

        }

        public void submit_predictions()
        {
            // Submit the users predictions to the database
            Debug.Log("Submitting predictions");

            GameObject[] Predictions = GameObject.FindGameObjectsWithTag("PredictionElement");
            foreach (GameObject pred in Predictions)
            {
                submit_prediciton(pred.GetComponent<Prediction_button>().match_id.ToString(),
                                  pred.GetComponent<Prediction_button>().user_prediction_home_score,
                                  pred.GetComponent<Prediction_button>().user_prediction_away_score);
            }

            scene_transition_manager.GetComponent<scene_manager>().load_prediction_submitted_scene();
        }

        private void submit_prediciton(string match_id, float home_pred, float away_pred)
        {
            Debug.Log("Submitting prediction for match id:" + match_id);

            string match_id_str = "match_ID: " + match_id;
            string match_id_json = JsonUtility.ToJson(match_id);
            string home_pred_str = JsonUtility.ToJson(home_pred);
            string away_pred_str = JsonUtility.ToJson(away_pred);

            // _prediction_database = FirebaseDatabase.DefaultInstance.RootReference;
            //_prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);

            string display_name;

            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            Debug.Log("app: " + app);

#if (UNITY_EDITOR)
            Debug.Log("in unity editor");
            Debug.Log("info: " + match_id + " " + home_pred + " " + away_pred);
            Debug.Log("mDatabaseRef: " + _prediction_database);

            display_name = "DESKTOP 4";

#else
            Debug.Log("in mobile");
            var user = auth.CurrentUser;
            display_name = auth.CurrentUser.DisplayName;
#endif

            _prediction_database.RootReference.Child("predictions").Child(match_id_str).Child(display_name).Child("match_id").SetRawJsonValueAsync(match_id);
            _prediction_database.RootReference.Child("predictions").Child(match_id_str).Child(display_name).Child("home_prediction").SetValueAsync(home_pred);
            _prediction_database.RootReference.Child("predictions").Child(match_id_str).Child(display_name).Child("away_prediction").SetValueAsync(away_pred);

        }

    }

}
