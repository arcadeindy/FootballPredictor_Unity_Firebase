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

        // User matchday score array
        public static int[] user_matchday_scores = new int[38];

        // Arrays for predictions spot on, correct, wrong and not made
        public static int[] user_pred_spoton = new int[38];
        public static int[] user_pred_correct = new int[38];
        public static int[] user_pred_wrong = new int[38];
        public static int[] user_pred_notmade = new int[38];


    }

}

