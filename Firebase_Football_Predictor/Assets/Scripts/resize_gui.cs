using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resize_gui : MonoBehaviour {

    private RectTransform panelRectTransform;


    // Use this for initialization


    void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();

        panelRectTransform.sizeDelta = new Vector2(1.0f, panelRectTransform.sizeDelta.y);
        //panelRectTransform.anchorMax = new Vector2(0, 1);
        //panelRectTransform.pivot = new Vector2(0.5f, 0.5f);
    }


// Update is called once per frame
void Update () {
		
	}
}
