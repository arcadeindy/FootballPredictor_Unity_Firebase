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

        public GameObject _loading_cover;
        private bool reload_asked;

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

            reload_asked = false;
        }

        private void Update()
        {
            if (reload_asked)
            {
                if (CommonData._loaded_fixtures &&
                    CommonData._loaded_scores)
                {
                    // Load scene again
                    scene_transition_manager.GetComponent<scene_manager>().load_league_scene();
                }
            }
        }


        void carry_out_firebase_auth()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        }


        private void get_global_position()
        {
            // Just find rank of total score for all users
            // Would be nice if this makes a table in future for all users

            // Going to try and store results in arrays as quicker than list - although fixed in size - BUT WE KNOW THE SIZE!
            // Create arrays for user name and user score
            string[] global_user_names = new string[CommonData._database_scores.ChildrenCount];
            int[] global_user_scores = new int[CommonData._database_scores.ChildrenCount];
            // Loop over snapshot children
            int count = 0;
            foreach (DataSnapshot snapshotChild in CommonData._database_scores.Children)
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

        public void update_and_refresh()
        {
            // Update results and scores
            // Get predictions from database
            database_caller dgp = new database_caller();
            //CommonData._loaded_predictions = false;
            //dgp.get_predictions_from_database();

            // Get fixtures and results
            CommonData._loaded_fixtures = false;
            dgp.get_fixtures_and_results_from_database();

            // Get scores from database
            CommonData._loaded_scores = false;
            dgp.get_scores_from_database();

            reload_asked = true;

            // Show loading screen if it is not being shown
            if (_loading_cover.activeInHierarchy == false)
            {
                _loading_cover.SetActive(true);
            }
            
            _loading_cover.SetActive(false);

        }

    }


}
