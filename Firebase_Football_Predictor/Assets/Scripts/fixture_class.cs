using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace football_predictor
{
    public class fixture_class
    {

        public DateTime fixture_date
        {
            get;
            set;
        }

        public String home_team
        {
            get;
            set;
        }

        public String away_team
        {
            get;
            set;
        }

        public String match_id
        {
            get;
            set;
        }

        public int home_result
        {
            get;
            set;
        }

        public int away_result
        {
            get;
            set;
        }

        public int home_prediction
        {
            get;
            set;
        }

        public int away_prediction
        {
            get;
            set;
        }

        public int user_score
        {
            get;
            set;
        }

        public int int_id
        {
            get;
            set;
        }

        public int matchday
        {
            get;
            set;
        }


        //public int[] user_matchday_scores
        //{
        //    get;
        //    set;
        //}
    }

}

