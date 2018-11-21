using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

namespace football_predictor
{
    public class firebase_helper : MonoBehaviour
    {
        public static string get_firebase_exception(AggregateException ex)
        {
            //AggregateException ex = task.Exception as AggregateException;
            if (ex != null)
            {
                Firebase.FirebaseException fbEx = null;
                foreach (Exception e in ex.InnerExceptions)
                {
                    fbEx = e as Firebase.FirebaseException;
                    if (fbEx != null)
                        break;
                }

                if (fbEx != null)
                {
                    Debug.LogError("Encountered a FirebaseException:" + fbEx.Message);
                    return fbEx.Message;
                }
            }
            return ("");
        }


    }

}

