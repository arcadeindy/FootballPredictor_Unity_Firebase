using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;
using Firebase;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using Firebase.Database;

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
        }

        void get_fixtures_from_firebase()
        {
            Debug.Log("In: get_fixtures_from_firebase");

            //List<string> Fixtures = new List<string>();
            List<fixture_class> C_Fixtures = new List<fixture_class>();

            // Get Fixtures from database
            // Read from the database if user has set these scores before
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            _prediction_database.GetReference("premier_league_fixtures").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // task faulted
                    Debug.Log("DATABASE task faulted");
                }
                else if (task.IsCompleted)
                {
                    print("SUCCESS");
                    DataSnapshot snapshot = task.Result;

                    Debug.Log("DATABASE snapshot ref: " + snapshot.Reference);
                    Debug.Log("DATABASE snapshot value: " + snapshot.Value);
                    //db_snap = task.Result;
                    int count = 1;
                    Debug.Log("children count = " + snapshot.ChildrenCount);
                    foreach (DataSnapshot snap in snapshot.Children)
                    {
                        fixture_class temp_fix = new fixture_class();

                        DateTime fix_date;
                        fix_date = new DateTime(Int32.Parse("20" + snap.Child("/date").Value.ToString().Substring(6, 2)), // year
                                                Int32.Parse(snap.Child("/date").Value.ToString().Substring(3, 2)),        // month
                                                Int32.Parse(snap.Child("/date").Value.ToString().Substring(0, 2)),        // day
                                                Int32.Parse(snap.Child("/time").Value.ToString().Substring(0, 2)),        // hour
                                                Int32.Parse(snap.Child("/time").Value.ToString().Substring(2, 2)),        // min
                                                            0);                                                           // sec
                        print(fix_date.Day + "/" + fix_date.Month + "/" + fix_date.Year + "-" + fix_date.Hour + ":" + fix_date.Minute);
                        temp_fix.fixture_date = fix_date;
                        //string the_date = ko_day + "/" + ko_month + "/" + ko_year + "-" + ko_hour + ":" + ko_min;
                        ////Debug.Log(count + "-" + snap.Child("/home_team").Value + "-" + the_date);

                        if (DateTime.Compare(DateTime.Now.AddDays(7), fix_date) > 0)
                        {
                            temp_fix.home_team = snap.Child("/home_team").Value.ToString();
                            temp_fix.away_team = snap.Child("/away_team").Value.ToString();
                            temp_fix.match_id = snapshot.Value.ToString();
                            C_Fixtures.Add(temp_fix);
                            Debug.Log("count= " + count + "temp fix info = " + temp_fix.home_team);
                        }
                        count++;
                    }
                    Debug.Log("fixture length = " + C_Fixtures.Count);
                    // Sort fixtures by date
                    C_Fixtures.Sort((x, y) => DateTime.Compare(x.fixture_date, y.fixture_date));
                    Debug.Log("fixture length = " + C_Fixtures.Count);

                    // Create fixture UI
                    foreach (fixture_class fix in C_Fixtures)
                    {
                        Debug.Log(" call create fix ui");
                        create_fixture_UI(fix);
                    }
                    // Add button at bottom
                    add_prediciton_button();
                }
            });

        } // end get fixtures from firebase


        private void create_fixture_UI(fixture_class fix)
        {
            Debug.Log("In create fixture UI");
            // Create a fixture UI element. With home & away teams with a match ID
            // This element is outlined in "Prediction_button.cs" 
            // (which is not related to the add_prediciton_button() void in this class

            // Make instance of prediction
            GameObject prediction_instance = Instantiate(_prediction_score_template);
            // Create prediction as child of scroll view content object
            prediction_instance.transform.SetParent(_prediction_content.transform, false);
            // Pass fixture
            prediction_instance.GetComponent<Prediction_button>().fix = fix;
            // Set team names
            prediction_instance.GetComponent<Prediction_button>().update_home_team_text(fix.home_team);
            prediction_instance.GetComponent<Prediction_button>().update_away_team_text(fix.away_team);
            //Set ko time and date
            prediction_instance.GetComponent<Prediction_button>().update_ko_time_text(fix.fixture_date);

            // Read from the database if user has set these scores before
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            // Set strings for db path
            string match_id_str = fix.match_id;
            string display_name;
            string db_path;
#if (UNITY_EDITOR)
            display_name = "DESKTOP4";
#else
            display_name = auth.CurrentUser.UserId;
#endif
           db_path = "premier_league_predictions" + "/" +
                match_id_str + "/" +
                display_name + "/" +
                "home_prediction";

            _prediction_database.GetReference(db_path).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // task faulted
                    Debug.Log("task faulted");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    //Debug.Log("snapshot ref: " + snapshot.Reference);
                    //Debug.Log("snapshot value: " + snapshot.Value);
                    prediction_instance.GetComponent<Prediction_button>().user_prediction_home_score_text.textComponent.color = Color.blue;
                    prediction_instance.GetComponent<Prediction_button>().user_prediction_home_score = int.Parse(snapshot.Value.ToString());
                    prediction_instance.GetComponent<Prediction_button>().update_predicted_home_score();
                }
            });

            // Repeat for away scores TODO: tidy this up in a loop or something
            db_path = "premier_league_predictions" + "/" +
                 match_id_str + "/" +
                 display_name + "/" +
                 "away_prediction";

            _prediction_database.GetReference(db_path).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // task faulted
                    Debug.Log("task faulted");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    prediction_instance.GetComponent<Prediction_button>().user_prediction_away_score_text.textComponent.color = Color.blue;
                    prediction_instance.GetComponent<Prediction_button>().user_prediction_away_score = int.Parse(snapshot.Value.ToString());
                    prediction_instance.GetComponent<Prediction_button>().update_predicted_away_score();
                }
            });
        }

        private void add_prediciton_button()
        {
            // Creates the button to press to submit all user predictions
            // Should be at bottom of the prediction scene

            GameObject prediction_button_instance;
            prediction_button_instance = Instantiate(_submit_prediction_button);
            prediction_button_instance.transform.SetParent(_prediction_content.transform, false);

            GameObject _footer_instance;
            _footer_instance = Instantiate(_footer_button);
            _footer_instance.transform.SetParent(_prediction_content.transform, false);
        }

        public void submit_predictions()
        {
            // Submit iterate through prediction buttons on page and call submission function
            Debug.Log("Submitting predictions");

            GameObject[] Predictions = GameObject.FindGameObjectsWithTag("PredictionElement");
            foreach (GameObject pred in Predictions)
            {
                submit_prediciton(pred.GetComponent<Prediction_button>().fix.match_id,
                                  pred.GetComponent<Prediction_button>().user_prediction_home_score,
                                  pred.GetComponent<Prediction_button>().user_prediction_away_score);
            }
            // If complete then load scene saying all is successful
            scene_transition_manager.GetComponent<scene_manager>().load_prediction_submitted_scene();
        }

        private void submit_prediciton(string match_id, float home_pred, float away_pred)
        {
            // Submit a given prediction to the database
            Debug.Log("Submitting prediction for match id:" + match_id);

            string match_id_str = match_id;
            string match_id_json = JsonUtility.ToJson(match_id);
            string home_pred_str = JsonUtility.ToJson(home_pred);
            string away_pred_str = JsonUtility.ToJson(away_pred);
            string display_name;

            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            Debug.Log("app: " + app);
            Debug.Log("in unity editor");
            Debug.Log("info: " + match_id + " " + home_pred + " " + away_pred);
            Debug.Log("mDatabaseRef: " + _prediction_database);
#if (UNITY_EDITOR)
            display_name = "DESKTOP4";
#else
            Debug.Log("in mobile");
            var user = auth.CurrentUser;
            display_name = auth.CurrentUser.DisplayName;
#endif
            _prediction_database.RootReference.Child("premier_league_predictions").Child(match_id_str).Child(display_name).Child("user_name").SetRawJsonValueAsync(display_name);
            _prediction_database.RootReference.Child("premier_league_predictions").Child(match_id_str).Child(display_name).Child("match_id").SetRawJsonValueAsync(match_id);
            _prediction_database.RootReference.Child("premier_league_predictions").Child(match_id_str).Child(display_name).Child("home_prediction").SetValueAsync(home_pred);
            _prediction_database.RootReference.Child("premier_league_predictions").Child(match_id_str).Child(display_name).Child("away_prediction").SetValueAsync(away_pred);

            // // add to stats - run transaction? might be best way of doing it
            // if (home_pred > away_pred)
            // {
            //     string path = "premier_league_fixtures" + "/" + match_id_str;
            //     _prediction_database.DefaultInstance.GetReference(path)
            //     _prediction_database.RootReference.RunTransaction(add_home_win_transaction).ContinueWith(task=>
            //     {
            //         if (task.Exception != null)
            //         {
            //             Debug.Log(task.Exception.ToString());
            //         }
            //         else if (task.IsCompleted)
            //         {
            //             Debug.Log("task complete");
            //         }

            //     })
            // }
            // else if (away_pred > home_pred)
            // {
            // }
            // else
            // {
            // }
            
        }
        
        TransactionResult add_home_win_transaction(MutableData mutableData)
        {
            mutableData.Child("premier_league_fixtures/")

        }

    }
}
