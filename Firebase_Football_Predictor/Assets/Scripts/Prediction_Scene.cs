using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;

public class Prediction_Scene : MonoBehaviour {


    public GameObject _prediction_content;
    public GameObject _prediction_score_template;

	// Use this for initialization
	void Start ()
    {
        get_fixtures_from_firebase();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Sets the default values for remote config.  These are the values that will
    // be used if we haven't fetched yet.
    System.Threading.Tasks.Task InitializeRemoteConfig()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>();

        // Adding first match as a test
        // Match ID and Team names and Time and Date
        defaults.Add("Match_1", "ARSVMCI TIME1500 DATE07072018");
        defaults.Add("Match_2", "BOUVBRH TIME1730 DATE17072018");
        defaults.Add("Match_3", "HDDVCHE TIME1500 DATE08072018");
        defaults.Add("Match_3", "WHUVWAT TIME1500 DATE08072018");
        defaults.Add("Match_4", "TOTVLIV TIME1500 DATE08072018");
        defaults.Add("Match_5", "MUNVSOU TIME1500 DATE08072018");
        defaults.Add("Match_6", "CRYVLEI TIME1500 DATE08072018");


        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
        return Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(System.TimeSpan.Zero);
    }

    void get_fixtures_from_firebase()
    {
        List<string> Fixtures = new List<string>();
        print("Fixtures: " + Fixtures);
        // For testing:
        Fixtures.Add("ARSVMCI TIME1500 DATE01072018");
        Fixtures.Add("BOUVBRH TIME1730 DATE02072018");
        Fixtures.Add("HDDVCHE TIME1500 DATE04072018");
        Fixtures.Add("WHUVWAT TIME1500 DATE04072018");
        Fixtures.Add("TOTVLIV TIME1500 DATE04072018");
        Fixtures.Add("MUNVSOU TIME1500 DATE04072018");
        Fixtures.Add("CRYVLEI TIME1500 DATE04072018");


        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_1").StringValue);
        Fixtures.Add(FirebaseRemoteConfig.GetValue("Match_2").StringValue);


        // Set up scene
        foreach (string fixture in Fixtures)
        {
            print(fixture);
            // If in future and within next week TODO: next week will need to be looked at
            DateTime fix_date;
            fix_date = new DateTime(Int32.Parse(fixture.Substring(25,4)),  // year
                                    Int32.Parse(fixture.Substring(23,2)),  // month
                                    Int32.Parse(fixture.Substring(21,2)),  // day
                                    Int32.Parse(fixture.Substring(12,2)),  // hour
                                    Int32.Parse(fixture.Substring(14,2)),  // minute
                                    0);                                    // second
            print("now date: " + DateTime.Now.Day + "/" + DateTime.Now.Month);
            print("fix date: " + fix_date.Day + "/" + fix_date.Month);
            // Check that fixture is within 5 days
            if(DateTime.Compare(DateTime.Now.AddDays(5), fix_date) > 0)
            {
                print("PASSED CHECK ONE");
                // Check fixture is not in the past
                if(DateTime.Compare(DateTime.Now, fix_date.AddMinutes(-30)) < 0)
                {
                    print("PASSED CHECK TWO");
                    // If fixture not in past and is within x days than format in scene
                    create_fixture_UI(fix_date,
                                      fixture.Substring(0, 3),
                                      fixture.Substring(4, 3));
                }

            }
        }

    }

    private void create_fixture_UI(DateTime ko_date, String home_team, String away_team)
    {
        print("Creating fixture UI");

        // Make instance of prediction
        GameObject prediction_instance = Instantiate(_prediction_score_template);

        // Create prediction as child of scroll view content object
        prediction_instance.transform.parent = _prediction_content.transform;

        // Set team names
        prediction_instance.GetComponent<Prediction_button>().update_home_team_text(home_team);
        prediction_instance.GetComponent<Prediction_button>().update_away_team_text(away_team);
        //Set ko time and date
        prediction_instance.GetComponent<Prediction_button>().update_ko_time_text(ko_date);


    }



}
