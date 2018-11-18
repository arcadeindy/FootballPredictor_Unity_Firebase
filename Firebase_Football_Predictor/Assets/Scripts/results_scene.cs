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
    public class results_scene : MonoBehaviour
    {

        public GameObject scene_transition_manager;

        // For auth and database
        private Firebase.Database.FirebaseDatabase _prediction_database;
        protected Firebase.Auth.FirebaseAuth auth;
        private Firebase.FirebaseApp app;

        //string user_uid; //uid
        //string display_name; //display name

        int total_user_score = 0;
        int prediction_spot_on = 0;
        int prediction_result_correct = 0;
        int prediction_wrong = 0;
        int prediction_not_made = 0;

        public Text Total_score_text;
        public Text SpotOn_text;
        public Text CorrectResult_text;
        public Text ResultWrong_text;
        public Text PredictionNotMade_text;

        // Variable to allow user score only to be written once to database instead of every loop
        int results_to_check;

        // Use this for initialization
        void Start()
        {
            Debug.Log("in Results Scene");

            // Rest due to bug FIXME:
            PlayerPrefs.DeleteKey("HighestWeeklyScore");
            PlayerPrefs.SetInt("HighestWeeklyScore", 0);
            for (int i = 0; i < CommonData.user_matchday_scores.Length; i++)
            {
                CommonData.user_matchday_scores[i] = 0;
            }
            PlayerPrefs.DeleteKey("Prediction_SpotOn");
            PlayerPrefs.DeleteKey("Prediction_Correct_result");
            PlayerPrefs.DeleteKey("Prediction_Correct");
            PlayerPrefs.DeleteKey("Prediction_Wrong");
            PlayerPrefs.DeleteKey("Prediction_NotMade");
            PlayerPrefs.SetInt("Prediction_SpotOn", 0);
            PlayerPrefs.SetInt("Prediction_Correct_result", 0);
            PlayerPrefs.SetInt("Prediction_Correct", 0);
            PlayerPrefs.SetInt("Prediction_Wrong", 0);
            PlayerPrefs.SetInt("Prediction_NotMade", 0);

            // Reset the matchday score arrays as at the moment these are updated
            // each time the scene is called atm FIXME:
            for (int matchday = 0; matchday < CommonData.user_pred_spoton.Length; matchday++)
            {
                CommonData.user_pred_spoton[matchday] = 0;
                CommonData.user_pred_correct[matchday] = 0;
                CommonData.user_pred_wrong[matchday] = 0;
                CommonData.user_pred_notmade[matchday] = 0;
            }

            // Get user from authentication
            carry_out_firebase_auth();

            // Makke empty list
            List<fixture_class> fixtures = new List<fixture_class>();
            get_fixtures_from_database(fixtures);

            // Get scores
            save_score_player_prefs();

            // Move to own scene TODO
            add_score_to_db();

        }

        private void Update()
        {
            Total_score_text.text = "Total Score : " + total_user_score;
            SpotOn_text.text = "Spot On = " + prediction_spot_on;
            CorrectResult_text.text = "Correct Result = " + prediction_result_correct;
            ResultWrong_text.text = "Wrong = " + prediction_wrong;
            PredictionNotMade_text.text = "Prediction not made = " + prediction_not_made;
        }

        void carry_out_firebase_auth()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        }


        void get_fixtures_from_database(List<fixture_class> fixture_class_list)
        {
            Debug.Log("Getting fixtures from database");

            // Get user home score prediction from database
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);

            foreach (DataSnapshot child in CommonData._database_fixtures.Children)
            {
                fixture_class fix_temp = new fixture_class();

                // If match is in the future then dont bother and skip
                string time = child.Child("time").Value.ToString();
                string date = child.Child("date").Value.ToString();
                DateTime fix_date;
                fix_date = new DateTime(Int32.Parse("20" + date.Substring(6, 2)),  // year
                                        Int32.Parse(date.Substring(3, 2)),  // month
                                        Int32.Parse(date.Substring(0, 2)),  // day
                                        Int32.Parse(time.Substring(0, 2)),  // hour
                                        Int32.Parse(time.Substring(2, 2)),  // minute
                                        0);                                 // second
                fix_temp.fixture_date = fix_date;
                if (DateTime.Compare(DateTime.Now, fix_date) < 0)
                {
                    continue;
                }

                fix_temp.match_id = child.Key;
                fix_temp.home_team = child.Child("home_team").Value.ToString();
                fix_temp.away_team = child.Child("away_team").Value.ToString();

                fix_temp.home_result = int.Parse(child.Child("home_score").Value.ToString());
                fix_temp.away_result = int.Parse(child.Child("away_score").Value.ToString());

                fix_temp.matchday = int.Parse(child.Child("matchday").Value.ToString());

                //fixture_class_list.Add(fix_temp);
                if (fix_temp.home_result > -1 && fix_temp.away_result > -1)
                {
                    // NOW CHECK RESULTS
                    check_home_prediction(fix_temp);

                    // Add to total results to check
                    results_to_check++;
                }
            }
        }

        private void check_home_prediction(fixture_class fix)
        {
            string pred_team = "home_prediction";

            // Get match ID
            string match_id_str = "match_ID" + int.Parse(fix.match_id.Substring(6, 3));

            string uid;
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
#if (UNITY_EDITOR)
            uid = "DESKTOP4";
#else
            var user = auth.CurrentUser;
            uid = auth.CurrentUser.UserId;
#endif    

            if(CommonData._database_predictions.Child(match_id_str).Child(uid).Child(pred_team).Value != null)
            {
                int score_out;
                if (int.TryParse(CommonData._database_predictions.Child(match_id_str).Child(uid).Child(pred_team).Value.ToString(), out score_out))
                {
                    fix.home_prediction = score_out;
                    // Home prediction done, next up away prediction
                    check_away_prediction(fix);
                }
                else
                {
                    // Add to prediction not made count
                    prediction_not_made++;
                }
            }
            else
            {
                // Add to prediction not made count
                prediction_not_made++;
            }

        }

        private void check_away_prediction(fixture_class fix)
        {
            string pred_team = "away_prediction";

            string match_id_str = "match_ID" + int.Parse(fix.match_id.Substring(6, 3));
            string uid;
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
#if (UNITY_EDITOR)
            uid = "DESKTOP4";
#else
            var user = auth.CurrentUser;
            uid = auth.CurrentUser.UserId;
#endif    

            if(CommonData._database_predictions.Child(match_id_str).Child(uid).Child(pred_team).Value != null)
            {
                int score_out;
                if (int.TryParse(CommonData._database_predictions.Child(match_id_str).Child(uid).Child(pred_team).Value.ToString(), out score_out))
                {
                    fix.away_prediction = score_out;
                    // Home prediction done, next up away prediction
                    check_score(fix);
                }
                else
                {
                    // Add to prediction not made count
                    prediction_not_made++;
                }
            }
            else
            {
                // Add to prediction not made count
                prediction_not_made++;
            }
        }

        private void check_score(fixture_class fix)
        {

            // Check scores are postive (have been updated)
            if (fix.home_prediction < 0 || fix.away_prediction < 0 || fix.home_result < 0 || fix.away_result < 0)
            {
                // If user has not made a prediction award 0 (should not get this far)
                fix.user_score = 0;
            }
            else
            {
                // if home score and away score exactly correct award 30 points
                if (fix.home_prediction == fix.home_result && fix.away_prediction == fix.away_result)
                {
                    fix.user_score = 30;
                    prediction_spot_on++;
                }
                // If the user didnt get it spot on see if the result was correct
                else
                {
                    // Did home team win and did user predict that?
                    if (fix.home_result > fix.away_result && fix.home_prediction > fix.away_prediction)
                    {
                        // award 10 points
                        fix.user_score = 10;
                        prediction_result_correct++;
                    }
                    // DId away team win and did user predict that?
                    else if (fix.away_result > fix.home_result && fix.away_prediction > fix.home_prediction)
                    {
                        // award 10 points
                        fix.user_score = 10;
                        prediction_result_correct++;
                    }
                    // Was it a draw and did the user predict that?
                    else if (fix.home_result == fix.away_result && fix.home_prediction == fix.away_prediction)
                    {
                        //draw - give 10 points
                        fix.user_score = 10;
                        prediction_result_correct++;
                    }
                    // Was the user wrong?
                    else
                    {
                        // no points
                        fix.user_score = 0;
                        prediction_wrong++;
                    }
                }
            }
            total_user_score = total_user_score + fix.user_score;
            // Add to matchday scores
            CommonData.user_matchday_scores[fix.matchday] = CommonData.user_matchday_scores[fix.matchday] + fix.user_score;

            if(fix.user_score == 30)
            {
                CommonData.user_pred_spoton[fix.matchday] = CommonData.user_pred_spoton[fix.matchday] + 1;
            }
            if (fix.user_score == 10)
            {
                CommonData.user_pred_correct[fix.matchday] = CommonData.user_pred_correct[fix.matchday] + 1;
            }
            if (fix.user_score == 0)
            {
                CommonData.user_pred_wrong[fix.matchday] = CommonData.user_pred_wrong[fix.matchday] + 1;
            }


            // TODO: MAKE SO DONE ONCE ALL SCORES CHECKED (ONLY DO ONCE)
            //save_score_player_prefs();
        }

        private void save_score_player_prefs()
        {
            //Get the highScore from player prefs if it is there, 0 otherwise.
            int highScore;
            highScore = PlayerPrefs.GetInt("HighScore", 0);
            //If our scoree is greter than highscore, set new higscore and save.
            if (total_user_score > highScore)
            {
                PlayerPrefs.SetInt("HighScore", total_user_score);
                PlayerPrefs.Save();
            }

            // Save spot on, corect result, wrong and nto made
            PlayerPrefs.SetInt("Prediction_SpotOn", prediction_spot_on);
            PlayerPrefs.SetInt("Prediction_Correct_result", prediction_result_correct);
            PlayerPrefs.SetInt("Prediction_Correct", prediction_spot_on + prediction_result_correct);
            PlayerPrefs.SetInt("Prediction_Wrong", prediction_wrong);
            PlayerPrefs.SetInt("Prediction_NotMade", prediction_not_made);


            // Set the users highest weekly score
            int highest_weekly_score;
            highest_weekly_score = PlayerPrefs.GetInt("HighestWeeklyScore", 0);
            int highest_weekly_score_matchday;
            highest_weekly_score_matchday = PlayerPrefs.GetInt("HighestWeeklyScoreMatchday", 0);
            int matchday = 1;
            foreach(int matchday_score in CommonData.user_matchday_scores)
            {
                if(matchday_score > highest_weekly_score)
                {
                    // Set new high score
                    highest_weekly_score = matchday_score;
                    // Update player prefs
                    PlayerPrefs.SetInt("HighestWeeklyScore", matchday_score);
                    PlayerPrefs.SetInt("HighestWeeklyScoreMatchday", matchday);
                }
                matchday++;
            }

        }

        private void add_score_to_db()
        {
            Debug.Log("ADDING SCORE TO DATABASE");

            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

            string display_name;
            string user_id;
#if (UNITY_EDITOR)
            user_id = "DESKTOP4";
            display_name = "DESKTOP4";
#else
            Debug.Log("in mobile");
            var user = auth.CurrentUser;
            user_id = auth.CurrentUser.UserId;
            display_name =  auth.CurrentUser.DisplayName;
#endif
            // Set the values in the database
            //_prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).SetValueAsync(total_user_score);
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("name").SetValueAsync(display_name);
            // total_user_score
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("score").SetValueAsync(PlayerPrefs.GetInt("HighScore", 0));
            // User highest weekly score
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("highest_weekly_score").SetValueAsync(PlayerPrefs.GetInt("HighestWeeklyScore", 0));
            // The matchday on which the users highest weekly score occured
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("highest_weekly_score_matchday").SetValueAsync(PlayerPrefs.GetInt("HighestWeeklyScoreMatchday", 0));
            //
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("Prediction_SpotOn").SetValueAsync(PlayerPrefs.GetInt("Prediction_SpotOn", 0));
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("Prediction_Correct_result").SetValueAsync(PlayerPrefs.GetInt("Prediction_Correct_result", 0));
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("Prediction_Correct").SetValueAsync(PlayerPrefs.GetInt("Prediction_Correct", 0));
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("Prediction_Wrong").SetValueAsync(PlayerPrefs.GetInt("Prediction_Wrong", 0));
            _prediction_database.RootReference.Child("user_scores").Child("premier_league").Child(user_id).Child("Prediction_NotMade").SetValueAsync(PlayerPrefs.GetInt("Prediction_NotMade", 0));



            // Is this needed?
            get_global_position();
        }

        private void get_global_position()
        {
            // Just find rank of total score for all users
            // Would be nice if this makes a table in future for all users
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            string path = "user_scores";

            Query myquery = _prediction_database.RootReference.Child(path).Child("premier_league").OrderByChild("score");

            myquery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("task faulted");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log("SNAPSHOT value = " + snapshot.Value);
                    Debug.Log("SNAPSHOT key = " + snapshot.Key);
                    Debug.Log("SNAPSHOT reference = " + snapshot.Reference);
                    Debug.Log("top score? = " + snapshot.Child("score").Value);

                }
            });

        }
    }
}