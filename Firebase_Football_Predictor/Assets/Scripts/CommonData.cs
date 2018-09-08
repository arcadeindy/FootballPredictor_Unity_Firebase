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

        // Store prediction fixtures and results firebase database query
        public static Query _database_premier_league_fixtures;

    }

}

