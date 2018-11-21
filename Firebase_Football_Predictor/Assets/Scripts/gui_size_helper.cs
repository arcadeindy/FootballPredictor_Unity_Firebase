using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gui_size_helper : MonoBehaviour {

    // Positions based from top of screen as a percentage
    // Start point
    public float yperc_1;
    public float yperc_2;

    private RectTransform this_rect;

    private float screen_height_px;


	// Use this for initialization
	void Start ()
    {
        yperc_2 = 0.2f;
        yperc_1 = 0.1f;

        this_rect = this.gameObject.GetComponent<RectTransform>();

        // Get screen height in pixels
        screen_height_px = Screen.height;
        print("screen_height_px " + screen_height_px);

        // Work out y position base don requested percentages
        float new_y_pos;
        new_y_pos = -1.0f * screen_height_px * 0.5f * (yperc_2 - yperc_1);
        print("new_y_pos " + new_y_pos);
        // Convert units
        //new_y_pos = new_y_pos * (Screen.height / Camera.GetComponent<Camera>().orthographicSize * 2);
        // Set new y position
        this_rect.position = new Vector3(this_rect.position.x,
                                         new_y_pos,
                                         this_rect.position.z);

        // Work out requested size delta
        float new_y_delta = (yperc_2 - yperc_1) * screen_height_px;
        // Set Height (2nd component of size delta)
        this_rect.sizeDelta = new Vector2(this_rect.sizeDelta.x,
                                          new_y_delta);

	}
	

}
