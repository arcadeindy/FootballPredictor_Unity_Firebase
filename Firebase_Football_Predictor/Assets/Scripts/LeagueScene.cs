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
    public class LeagueScene : MonoBehaviour
    {

        public GameObject scene_transition_manager;

        public GameObject _scrollbar_content;

        public GameObject _entry_template;

        // For auth and database
        private Firebase.Database.FirebaseDatabase _prediction_database;
        protected Firebase.Auth.FirebaseAuth auth;
        private Firebase.FirebaseApp app;

        // Use this for initialization
        void Start()
        {
            Debug.Log("in Leagues Scene");

            // Get user from authentication
            carry_out_firebase_auth();

            // Get position in league
            get_global_position();

        }


        void carry_out_firebase_auth()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        }


        private void get_global_position()
        {
            // Just find rank of total score for all users
            // Would be nice if this makes a table in future for all users
            app = CommonData.app;
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            string path = "user_scores";

            Query myquery = _prediction_database.RootReference.Child(path).Child("premier_league").OrderByChild("score"); // is limit to last X will give highest

            myquery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("task faulted");
                }
                else if (task.IsCompleted)
                {

                    DataSnapshot snapshot = task.Result;
                    // Going to try and store results in arrays as quicker than list - although fixed in size - BUT WE KNOW THE SIZE!
                    // Create arrays for user name and user score
                    string[] global_user_names = new string[snapshot.ChildrenCount];
                    int[] global_user_scores = new int[snapshot.ChildrenCount];
                    // Loop over snapshot children
                    int count = 0;
                    foreach (DataSnapshot snapshotChild in snapshot.Children)
                    {
                        string user_name = snapshotChild.Child("name").Value.ToString();
                        int user_score = int.Parse(snapshotChild.Child("score").Value.ToString());
                        Debug.Log("user = " + user_name + ", score = " + user_score);

                        // Add values found to arrays (so can reverse later, as currently in ascending order)
                        global_user_names[count] = user_name;
                        global_user_scores[count] = user_score;
                        count++;
                    }
                    // Call task to create UI
                    Debug.Log("CALLING CREATE LEAGUE UI");
                    create_global_league_UI(global_user_names, global_user_scores);
                }
            });
        }

        private void create_global_league_UI(string[] users, int[] scores)
        {
            Debug.Log("CALLED CREATE LEAGUE UI ");
            // Create table header
            create_table_entry("Users", _scrollbar_content);
            create_table_entry("Score", _scrollbar_content);


            // Loop through the arrays in reverse order 
            // Just display the top 10 for now
            for (int i = users.Length-1; i>=0; i--)
            {
                Debug.Log("i = " + i);
                Debug.Log(users[i] + " :  " + scores[i]);

                // Add user name as a child of vertical scroll content
                create_table_entry(users[i], _scrollbar_content);

                // Add user score
                create_table_entry(scores[i].ToString(), _scrollbar_content);

            }
        }

        private void create_table_entry(string value, GameObject table_content)
        {
            // Adds a value to as a child of another
            // Assumes grid content fitter used
            // Will just go through columns according to grid content fitter rules
            Debug.Log("CREATING TABLE ENTRY");
            GameObject _table_entry = Instantiate(_entry_template);
            Debug.Log("A");
            _table_entry.GetComponentInChildren<Text>().text = value;
            Debug.Log("B");
            _table_entry.transform.SetParent(table_content.transform, false);
            Debug.Log("C");
            _table_entry.SetActive(true);
        }

    }
}
