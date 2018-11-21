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
using UnityEngine.SceneManagement;

namespace football_predictor
{
    public class LeagueScene : MonoBehaviour
    {

        public GameObject scene_transition_manager;

        public GameObject _scrollbar_content;

        public GameObject _entry_template;

        public GameObject _loading_cover;

        public string[] table_vars;

        private bool reload_asked;

        // For auth and database
        private Firebase.Database.FirebaseDatabase _prediction_database;
        protected Firebase.Auth.FirebaseAuth auth;
        private Firebase.FirebaseApp app;

        // Use this for initialization
        void Start()
        {
            Debug.Log("In Leagues Scene");

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
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    //scene_transition_manager.GetComponent<scene_manager>().load_league_scene();
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

            int[,] global_scores = new int[CommonData._database_scores.ChildrenCount, table_vars.Length];

            // Loop over snapshot children
            int user_index = 0;

            // Go through each user score for each week
            foreach (DataSnapshot snapshotChild in CommonData._database_scores.Children)
            {
                // Get from database snapshot
                string user_name = snapshotChild.Child("name").Value.ToString();
                int var_index = 0;
                foreach (string tablevar in table_vars)
                {
                    foreach(DataSnapshot child in snapshotChild.Children)
                    {
                        print(child.Key);

                    }
                    if (snapshotChild.Child(tablevar).Exists)
                    {
                        global_scores[user_index, var_index] = int.Parse(snapshotChild.Child(tablevar).Value.ToString());

                    }
                    var_index++;
                }

                // Add values found to arrays (so can reverse later, as currently in ascending order)
                global_user_names[user_index] = user_name;
                user_index++;
            }

            // Call task to create UI
            //create_global_league_UI(global_user_names, global_user_scores);
            create_global_league_UI(global_user_names, global_scores);

        }

        private void create_global_league_UI(string[] users, int[,] scores)
        {
            // Set table length (number of table vars + 1 (for user name)
            _scrollbar_content.GetComponent<GridLayoutGroup>().constraintCount = table_vars.Length + 1;

            // Create table header
            create_table_entry("Users", _scrollbar_content);
            foreach(string tablevar in table_vars)
            {
                create_table_entry(tablevar, _scrollbar_content);

            }

            // Loop through the arrays in reverse order (high to low)
            for (int i = users.Length-1; i>=0; i--)
            {
                //Debug.Log("i = " + i);
                //Debug.Log(users[i] + " :  " + scores[i]);

                // Add user name as a child of vertical scroll content
                create_table_entry(users[i], _scrollbar_content);

                // Add table vars
                int var_count = 0;
                foreach(string tablevar in table_vars)
                {
                    create_table_entry(scores[i, var_count].ToString(), _scrollbar_content);
                    var_count++;
                }

            }
        }

        private void create_table_entry(string value, GameObject table_content)
        {
            // Adds a value to as a child of another
            // Assumes grid content fitter used
            // Will just go through columns according to grid content fitter rules - TODO: if more than two columns then set here?!
            GameObject _table_entry = Instantiate(_entry_template);
            // Set text
            _table_entry.GetComponentInChildren<Text>().text = value;
            // Set as child
            _table_entry.transform.SetParent(table_content.transform, false);
            // Set as active so we can see it
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
