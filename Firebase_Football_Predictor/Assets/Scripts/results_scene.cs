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

public class results_scene : MonoBehaviour {

    public GameObject scene_transition_manager;

    private Firebase.Database.FirebaseDatabase _prediction_database;
    protected Firebase.Auth.FirebaseAuth auth;
    private Firebase.FirebaseApp app;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
