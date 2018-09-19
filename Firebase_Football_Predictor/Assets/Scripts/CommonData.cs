using UnityEngine;
using Firebase;
using Firebase.Database;

namespace football_predictor
{
    public class CommonData
    {

        public static Firebase.FirebaseApp app;

        // Has home scene been views?
        public static bool first_home_view;

        // MAYBE THE BELOW SHOULD BE A DATABASE CLASS?

        // Store fixtures and results firebase database query
        public static DataSnapshot _database_fixtures;
        public static bool _loaded_fixtures;

        // Store user predictions
        public static DataSnapshot _database_predictions;
        public static bool _loaded_predictions;

        // Store user scores
        public static DataSnapshot _database_scores;
        public static bool _loaded_scores;

    }

}

