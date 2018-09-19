using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace football_predictor
{
    public class Prediction_Submitted_script : MonoBehaviour
    {

        public GameObject scene_transition_manager;

        protected Firebase.Auth.FirebaseAuth auth;

        public Text Text_field;

        public void Start()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

            welcome_user();

            // If predictions resubmitted then need to update the database stored on device
            database_caller dgp = new database_caller();
            dgp.get_predictions_from_database();
        }



        void welcome_user()
        {
            var user = auth.CurrentUser;
            if (user != null)
            {
                Text_field.text = "Predictions Submitted! \n \n user: \n" + user.DisplayName;
            }
            else
            {
                print("user is not signed in");
                scene_transition_manager.GetComponent<scene_manager>().load_no_user_scene();
            }

        }

    }
}
