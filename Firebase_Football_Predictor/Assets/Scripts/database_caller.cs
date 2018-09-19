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
// TIDY UP THE ABOVE

namespace football_predictor
{
    public class database_caller
    {
        /// <summary>
        /// Want to return database query containing the current state
        /// The quey we want to update is referenced from CommonData
        /// </summary>

        private Firebase.Database.FirebaseDatabase _prediction_database;
        protected Firebase.Auth.FirebaseAuth auth;
        private Firebase.FirebaseApp app;

        // Use this for initialization
        public void get_predictions_from_database()
        {
            Debug.Log("Getting predictions from database");

            // Get app
            app = CommonData.app;
            // Get database instance
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

            // Seee if data exists at query
            _prediction_database.RootReference.Child("predictions").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // task faulted
                    Debug.Log("task faulted");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log(" get predictions task completed");
                    // Get snapshot from task
                    DataSnapshot snapshot = task.Result;
                    // Set snapshot
                    CommonData._database_predictions = snapshot;
                    CommonData._loaded_predictions = true;
                }
            });
        }


        public void get_fixtures_and_results_from_database()
        {
            Debug.Log("Getting fixtures and results from database");

            // Get app
            app = CommonData.app;
            // Get database instance
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

            // Seee if data exists at query
            _prediction_database.RootReference.Child("premier_league_fixtures").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // task faulted
                    Debug.Log("task faulted");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Get fixtures and results task completed");
                    // Get snapshot from task
                    DataSnapshot snapshot = task.Result;
                    // Set snapshot
                    CommonData._database_fixtures = snapshot;
                    CommonData._loaded_fixtures = true;
                }
            });
        }


        public void get_scores_from_database()
        {
            Debug.Log("Getting scores from database");

            // Get app
            app = CommonData.app;
            // Get database instance
            _prediction_database = Firebase.Database.FirebaseDatabase.GetInstance(app);
            // Get user from authentication
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

            // Seee if data exists at query
            Query score_query = _prediction_database.RootReference.Child("user_scores").Child("premier_league").OrderByChild("score");

            score_query.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // task faulted
                    Debug.Log("task faulted");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log(" get scores task completed");
                    // Get snapshot from task
                    DataSnapshot snapshot = task.Result;
                    // Set snapshot
                    CommonData._database_scores = snapshot;
                    CommonData._loaded_scores = true;
                }
            });
        }
    }
}
