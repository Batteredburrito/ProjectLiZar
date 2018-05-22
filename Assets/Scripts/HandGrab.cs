using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandGrab : MonoBehaviour

{

    public string buttonName;
    public XRNode nodeType;
    // This affects the distance of object grab from controller
    public Vector3 objectOffset;
    // Default grab distance relative to the controller, (Where the collider sphere is)
    public float grabDistance = 0.1f;
    // Force at which an object is thrown or considered moving (escape velocity)
    public float throwingMultiplier = 1.25f;
    // To determine if "grabbing" is determined
    public string grabName = "Grabbing";

    // Gets the current position of the controller and its transform values
    private Transform _currentObject;
    // Does as it says and stores the last fram position to reduce stutter
    private Vector3 _lastFramePosition;
    
	// Use this for initialization
	void Start ()
    {
        // Clearing all variables back to zero
        _currentObject = null;
        _lastFramePosition = transform.position;
        		
	}
	
	// Update is called once per frame
	void Update ()
    {
        // rotation and position tracking. Updates every frame, should be 90fps
        transform.localPosition = UnityEngine.XR.InputTracking.GetLocalPosition(nodeType);
        transform.localRotation = UnityEngine.XR.InputTracking.GetLocalRotation(nodeType);

        // Check the object in hand variable. If hand is empty, check to see if we can pick anything up (Object highlight posibilites)
        if (_currentObject == null)
        {
            // Check to see if there are any colliders in the proximity of hands
            Collider[] colliders = Physics.OverlapSphere(transform.position, grabDistance);
            if (colliders.Length > 0)
            {
                //If there is a collision, then pickup the object if we press the grab button and give it the tag "Grabbing"
                if (Input.GetAxis(buttonName) >= 0.01F && colliders[0].transform.CompareTag(grabName))
                {
                    //Sets the current object to the object we have just picked up
                    _currentObject = colliders[0].transform;

                    //If there is no RigidBody attached to the current object, then assign it a RigidBody variable
                    if (_currentObject.GetComponent<Rigidbody>() == null)
                    {
                        _currentObject.gameObject.AddComponent<Rigidbody>();
                    }

                    // Disable all physics on the grabbed object until it is released (May need to change if we want to enable objects to be swung and hit over "live objects")
                    _currentObject.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
        else
        // We must now have an object in our hand at this point. We now update is current position to the current hand poistion + the offset definded above
        {
            _currentObject.position = transform.position + objectOffset;

            //If we release the grab button, we drop the object here
            if (Input.GetAxis(buttonName) < 0.01f)
            {
                // Return the value of the object back to Non-Kinematic and re-enable physics
                Rigidbody _objectRB = _currentObject.GetComponent<Rigidbody>();
                _objectRB.isKinematic = false;

                // Fun stuff starts here, we now calculate the hands current velocity
                Vector3 Velocity = (transform.position - _lastFramePosition) / Time.deltaTime;

                // We now set that object velocity to the current velocity of the hand (I may adjust this to improve physics simulation)
                _objectRB.velocity = Velocity * throwingMultiplier;

                // Kill the reference and revert the hands back to being empty
                _currentObject = null;

            }
        }

        // Store the current position of the hand for velocity calculation in the upcoming frame
        _lastFramePosition = transform.position;

     //Tying off all loose ends here
	}
}

