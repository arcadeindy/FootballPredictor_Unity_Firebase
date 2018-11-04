using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace football_predictor
{
    public class weekly_results_bar_element : MonoBehaviour {

        public GameObject scene_controller;

        public int bar_matchday
        {
            get;
            set;
        }

        void Start()
        {
            //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
            this.gameObject.GetComponent<Button>().onClick.AddListener(button_press);
        }

            // If bar element clicked then update the text in the weekly results scene
            public void button_press()
        {
            scene_controller.GetComponent<weekly_results_scene_controller>().set_info_text(bar_matchday);
        }


    }
}