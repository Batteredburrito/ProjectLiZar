using System.Collections;
using UnityEngine;


public class GetVRInputNames : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // This function gets the names for the controllers currently connected and reports the moved controller back to the Debug Log 
        int i = 0; 
        while (i < 4) {
            if (Mathf.Abs(Input.GetAxis("Controller" + i + "XAxis")) > 0.2F || Mathf.Abs(Input.GetAxis("Controller" + i + "YAxis")) > 0.2F)
            Debug.Log(Input.GetJoystickNames()[i] + " has been moved");
            i++;
        }

	}
}
