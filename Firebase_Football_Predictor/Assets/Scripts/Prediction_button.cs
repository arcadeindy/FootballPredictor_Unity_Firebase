﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Prediction_button : MonoBehaviour
{

    public string home_team;
    public string away_team;
    public DateTime ko_date;

    public int match_id;

    public int user_prediction_home_score;
    public int user_prediction_away_score;
    public Button button_home_score_increase;
    public Button button_home_score_decrease;
    public Button button_away_score_increase;
    public Button button_away_score_decrease;

    public Text home_team_text;
    public Text away_team_text;
    public Text ko_date_text;
    public Text ko_time_text;
    //public Text user_prediction_home_score_text;
    //public Text user_prediction_away_score_text;
    public InputField user_prediction_home_score_text;
    public InputField user_prediction_away_score_text;

    // Use this for initialization
    void Start()
    {
        //user_prediction_home_score = 0;
        //user_prediction_away_score = 0;
        user_prediction_home_score_text.text = user_prediction_home_score.ToString();
        user_prediction_away_score_text.text = user_prediction_away_score.ToString();

        // Ad listeners to buttons
        button_home_score_increase.onClick.AddListener(increase_home_score);
        button_home_score_decrease.onClick.AddListener(decrease_home_score);
        button_away_score_increase.onClick.AddListener(increase_away_score);
        button_away_score_decrease.onClick.AddListener(decrease_away_score);

    }


    public void update_home_team_text(String home_team)
    {
        Debug.Log("home team is = " + home_team);
        home_team = lookup_full_team_name(home_team);
        home_team_text.text = home_team;
    }

    public void update_away_team_text(String away_team)
    {
        away_team = lookup_full_team_name(away_team);
        away_team_text.text = away_team;
    }

    public void update_ko_time_text(DateTime ko_date)
    {
        ko_date_text.text = ko_date.ToString("dd/MM");
        ko_time_text.text = ko_date.ToString("HH:mm");
    }

    public void update_predicted_home_score()
    {
        user_prediction_home_score_text.text = user_prediction_home_score.ToString();
    }

    public void update_predicted_away_score()
    {
        user_prediction_away_score_text.text = user_prediction_away_score.ToString();
    }

    public void increase_home_score()
    {
        user_prediction_home_score_text.textComponent.color = Color.black;
        print("increasing_home_score" + user_prediction_home_score);
        user_prediction_home_score++;
        user_prediction_home_score_text.text = " " + user_prediction_home_score.ToString();
    }

    public void decrease_home_score()
    {
        if (user_prediction_home_score > 0)
        {
            user_prediction_home_score_text.textComponent.color = Color.black;
            user_prediction_home_score--;
            user_prediction_home_score_text.text = user_prediction_home_score.ToString();
        }
    }

    public void increase_away_score()
    {
        user_prediction_away_score_text.textComponent.color = Color.black;
        user_prediction_away_score++;
        user_prediction_away_score_text.text = user_prediction_away_score.ToString();
    }

    public void decrease_away_score()
    {
        if (user_prediction_away_score > 0)
        {
            user_prediction_away_score_text.textComponent.color = Color.black;
            user_prediction_away_score--;
            user_prediction_away_score_text.text = user_prediction_away_score.ToString();
        }
    }

    private String lookup_full_team_name(String team_abv)
    {
        if (team_abv == "BOU")
        {
            return "Bournemouth";
        }
        if (team_abv == "ARS")
        {
            return "Arsenal";
        }
        if (team_abv == "BRH")
        {
            return "Brighton";
        }
        if (team_abv == "BUR")
        {
            return "Burnley";
        }
        if (team_abv == "CHE")
        {
            return "Chelsea";
        }
        if (team_abv == "CRY")
        {
            return "Crystal Palace";
        }
        if (team_abv == "EVE")
        {
            return "Everton";
        }
        if (team_abv == "HDD")
        {
            return "Huddersfield";
        }
        if (team_abv == "LEI")
        {
            return "Leicester";
        }
        if (team_abv == "LIV")
        {
            return "Liverpool";
        }
        if (team_abv == "MCI")
        {
            return "Manchester City";
        }
        if (team_abv == "MUN")
        {
            return "Manchester United";
        }
        if (team_abv == "NEW")
        {
            return "Newcastle";
        }
        if (team_abv == "SOU")
        {
            return "Southampton";
        }
        if (team_abv == "STK")
        {
            return "Stoke";
        }
        if (team_abv == "SWA")
        {
            return "Swansea";
        }
        if (team_abv == "TOT")
        {
            return "Tottenham";
        }
        if (team_abv == "WAT")
        {
            return "Watford";
        }
        if (team_abv == "WBA")
        {
            return "West Brom";
        }
        if (team_abv == "WHU")
        {
            return "West Ham";
        }

        // Newly promoted teams
        if (team_abv == "CDF")
        {
            return "Cardiff";
        }
        if (team_abv == "FUL")
        {
            return "Fulham";
        }
        if (team_abv == "WLV")
        {
            return "Wolverhampton";
        }


        // If team not found just return abreviaiton
        return team_abv;
    }
}