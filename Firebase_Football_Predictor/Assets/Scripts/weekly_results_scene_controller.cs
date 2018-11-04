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
    public class weekly_results_scene_controller : MonoBehaviour
    {

        public GameObject bar_element;

        public GameObject graph_space;

        public Text matchday_result_info;


        // Use this for initialization
        void Start()
        {

            make_bar_chart(CommonData.user_matchday_scores);

            //int[] test_y_vals  = new int[10] { 30, 60, 20, 40, 150, 30, 70, 30, 10, 50};

            //make_bar_chart(test_y_vals);

            // Set default info text
            set_info_text(1);

        }

        public void set_info_text(int matchday)
        {
            // fix bug needs fixing
            matchday = matchday - 1;
            print("mathday = " + matchday);
            matchday_result_info.text = (" MD = " + (1 + matchday).ToString() + " |" +
                                         " SO=" + CommonData.user_pred_spoton[matchday].ToString() +
                                         " C=" + CommonData.user_pred_correct[matchday].ToString() +
                                         " W=" + CommonData.user_pred_wrong[matchday].ToString()
                                         );
        }

        // Update is called once per frame
        void Update()
        {

        }


        private void make_bar_chart(int[] y_values)
        {
            // Set x & y base of bar element
            int x_s = 1;
            int y_s = 1;
            // Set bar spacing
            int x_spacing = 1;

            int count = 1;


            foreach(int y_val in y_values)
            {
                int temp_yval;
                // if yval is zero set it to 5 so we can see the graph
                temp_yval = y_val;
                if (temp_yval == 0)
                {
                    temp_yval = 5;
                }

                // Create a bar element
                GameObject bar_temp = GameObject.Instantiate(bar_element);
                // Set active
                bar_temp.SetActive(true);
                // Set parent
                bar_temp.transform.SetParent(graph_space.transform, false);
                // Set base position
                bar_temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(x_s, y_s);
                // Get bar element base width
                float element_base_x = bar_temp.GetComponent<RectTransform>().sizeDelta.x; 
                // Set height (and base again)
                bar_temp.GetComponent<RectTransform>().sizeDelta = new Vector2(element_base_x, temp_yval * 4);

                // Set text of bar element to score that week
                bar_temp.GetComponentInChildren<Text>().text = y_val.ToString();

                // Set matchday value of bar element - so that if bar element is clicked on that matchday can be loaded
                bar_temp.GetComponent<weekly_results_bar_element>().bar_matchday = count;

                // Increment x position so bar elements not on top of each other
                x_s = x_s + x_spacing + (int)element_base_x;

                // Increment matchday 
                count++;
            }
        }
    }
}

