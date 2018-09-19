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

        public GameObject _no_predictions_button;

        // private DatabaseReference _prediction_database;
        private Firebase.Database.FirebaseDatabase _prediction_database;
        protected Firebase.Auth.FirebaseAuth auth;
        private Firebase.FirebaseApp app;

        private bool any_predictions_in_seven_days = false;

        // Use this for initialization
        void Start()
        {
            Debug.Log("STARTING PREDICTION");
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            Debug.Log("app: " + app);

            // Starts the scene
            StartGame();
        }


        void StartGame()
        {
            Debug.Log("Starting game...(prediction scene)");

            // Get fixtures from database
            get_fixtures_from_database();

            // Check for errors
            check_for_error_message();
    
        }


        void get_fixtures_from_database()
        {
            Debug.Log("In: get_fixtures_from_database");

            List<fixture_class> C_Fixtures = new List<fixture_class>(); // TODO: think of better name

            // Read from the database if user has set these scores before
            // (make them blue)
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

            foreach (DataSnapshot child in CommonData._database_fixtures.Children)
            {
                //Debug.Log(child.Key);
                // Create temporary fixure class
                fixture_class fix_temp = new fixture_class();
                // Get match id
                fix_temp.match_id = child.Key;
                // Get home team
                //Debug.Log("home_team??? " + child.Child("home_team").Value);
                fix_temp.home_team = child.Child("home_team").Value.ToString();
                // Get away team
                //Debug.Log("away_team??? " + child.Child("away_team").Value);
                fix_temp.away_team = child.Child("away_team").Value.ToString();
                // Get date
                //Debug.Log("home_team??? " + child.Child("time").Value);
                string time = child.Child("time").Value.ToString();
                //Debug.Log("home_team??? " + child.Child("date").Value);
                string date = child.Child("date").Value.ToString();
                DateTime fix_date;
                fix_date = new DateTime(Int32.Parse("20" + date.Substring(6, 2)),  // year
                                        Int32.Parse(date.Substring(3, 2)),  // month
                                        Int32.Parse(date.Substring(0, 2)),  // day
                                        Int32.Parse(time.Substring(0, 2)),  // hour
                                        Int32.Parse(time.Substring(2, 2)),  // minute
                                        0);                                 // second
                // Convert to local user time
                //fix_date = ConvertTime_LondonToLocal(fix_date);

                fix_temp.fixture_date = fix_date;
                // Add fixture to list
                C_Fixtures.Add(fix_temp);
                // See if any fixtures within 7 days (includes past)
                if (DateTime.Compare(DateTime.Now.AddDays(7), fix_date) > 0)
                {
                    // Check fixture is not in the past
                    if (DateTime.Compare(DateTime.Now, fix_date.AddMinutes(-30)) < 0)
                    {
                        // Create fixture UI
                        create_fixture_UI(fix_temp);
                        // If no fixtures going to be displayed then tell user
                        any_predictions_in_seven_days = true;
                        // In case error got displayed call again and it should be removed
                        check_for_error_message();

                    }
                }
            }
        }

        private DateTime ConvertTime_LondonToLocal(DateTime dateTime)
        {
            // BUG IN UNITY PREVENTS THIS. NEED TO USE .NET4.5 but firebase is incomatable with it
            TimeZoneInfo EnglishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            return TimeZoneInfo.ConvertTime(dateTime,
                EnglishTimeZone,
                TimeZoneInfo.Local);
        }

        private void create_fixture_UI(fixture_class fix)
        {
            Debug.Log("Creating fixture UI: " + fix.home_team + "v" + fix.away_team);
            // Create a fixture UI element. With home & away teams with a match ID
            // This element is outlined in "Prediction_button.cs" 
            // (which is not related to the add_prediciton_button() void in this class

            // Make instance of prediction
            GameObject prediction_instance = Instantiate(_prediction_score_template);

            // Create prediction as child of scroll view content object
            prediction_instance.transform.SetParent(_prediction_content.transform, false);

            prediction_instance.SetActive(true);

            // Add match id
            string[] split_fix_id = fix.match_id.Split('_');
            fix.match_id = split_fix_id[1];

            string match_id_str;
            if (int.Parse(fix.match_id) < 10) // SHOULD THIS BE FIXED ON DATABASE INPUT? NEXT YEAR
            {
                match_id_str = "match_ID" + int.Parse(fix.match_id).ToString("0");
            }
            else if (int.Parse(fix.match_id) < 100)
            {
                match_id_str = "match_ID" + int.Parse(fix.match_id).ToString("00");
            }
            else //(int.Parse(match_id) < 1000) // should be increased for 10000 if we ever get there
            {
                match_id_str = "match_ID" + int.Parse(fix.match_id).ToString("000");
            }

            // Read from the database if user has set these scores before
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            string user_id;

            // TO DO - change to uid
#if (UNITY_EDITOR)
            user_id = "DESKTOP4";
#else
            user_id = auth.CurrentUser.UserId;
#endif
            Debug.Log("Starting to add to button, match ID: " + fix.match_id);
            prediction_instance.GetComponent<Prediction_button>().match_id = int.Parse(fix.match_id);
            // Set team names
            //Debug.Log("Starting to add to button, team names " + fix.home_team);
            prediction_instance.GetComponent<Prediction_button>().home_team = fix.home_team;
            prediction_instance.GetComponent<Prediction_button>().update_home_team_text(fix.home_team);

            //Debug.Log("Starting to add to button, team names " + fix.away_team);
            prediction_instance.GetComponent<Prediction_button>().away_team = fix.away_team;
            prediction_instance.GetComponent<Prediction_button>().update_away_team_text(fix.away_team);

            //Set ko time and date
            //Debug.Log("Starting to add to button, date " + fix.fixture_date);
            prediction_instance.GetComponent<Prediction_button>().ko_date = fix.fixture_date;

            // Check if home prediction exists (might not have been made)
            if (CommonData._database_predictions.Child(match_id_str).Child(user_id).Child("home_prediction").Value != null)
            {
                prediction_instance.GetComponent<Prediction_button>().user_prediction_home_score_text.textComponent.color = Color.blue;
                DataSnapshot result = CommonData._database_predictions.Child(match_id_str).Child(user_id).Child("home_prediction");
                Debug.Log("HOME PRED READ IN = " + int.Parse(result.Value.ToString()));
                prediction_instance.GetComponent<Prediction_button>().user_prediction_home_score = int.Parse(result.Value.ToString());
                //prediction_instance.GetComponent<Prediction_button>().update_predicted_home_score();
            }

            // Check if away prediction exists (might not have been made)
            if (CommonData._database_predictions.Child(match_id_str).Child(user_id).Child("away_prediction").Value != null)
            {
                prediction_instance.GetComponent<Prediction_button>().user_prediction_away_score_text.textComponent.color = Color.blue;
                DataSnapshot result = CommonData._database_predictions.Child(match_id_str).Child(user_id).Child("away_prediction");
                Debug.Log("AWAY PRED READ IN = " + int.Parse(result.Value.ToString()));
                prediction_instance.GetComponent<Prediction_button>().user_prediction_away_score = int.Parse(result.Value.ToString());
                //prediction_instance.GetComponent<Prediction_button>().update_predicted_away_score();
            }

        }


        private void check_for_error_message()
        {
            // Writes message if no fixtures found within 7 days
            if (any_predictions_in_seven_days == false)
            {
                _no_predictions_button.SetActive(true);
                _submit_prediction_button.SetActive(false);
            }
            else
            {
                _no_predictions_button.SetActive(false);
                _submit_prediction_button.SetActive(true);
            }
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
            //Debug.Log("Submitting prediction for match id:" + match_id);

            string match_id_str = "match_ID" + match_id;

            string user_id;
            string name_name;

            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

#if (UNITY_EDITOR)
            user_id = "DESKTOP4";
            name_name = "DESKTOP4";
#else
            Debug.Log("in mobile");
            var user = auth.CurrentUser;
            user_id = auth.CurrentUser.UserId;
            name_name =  auth.CurrentUser.DisplayName;
#endif
            // Set the values in the database
            _prediction_database.RootReference.Child("predictions").Child(match_id_str).Child(user_id).Child("match_id").SetRawJsonValueAsync(match_id);
            _prediction_database.RootReference.Child("predictions").Child(match_id_str).Child(user_id).Child("home_prediction").SetValueAsync(home_pred);
            _prediction_database.RootReference.Child("predictions").Child(match_id_str).Child(user_id).Child("user_name").SetValueAsync(name_name);
            _prediction_database.RootReference.Child("predictions").Child(match_id_str).Child(user_id).Child("away_prediction").SetValueAsync(away_pred);

        }
    }

}
