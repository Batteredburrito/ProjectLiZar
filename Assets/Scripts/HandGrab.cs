using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandGrab : MonoBehaviour

{
    // Variable for Controller button register

    public string buttonName;

    // Allows objects to be transferred between hands

    public HandGrab secondaryHandRef;

    // Determines the referenced object type

    public XRNode nodeType;

    // This affects the distance of object grab from controller

    public Vector3 objectGrabOffset;

    // Default grab distance relative to the controller, (Where the collider sphere is) This can be changed to affect the visuals and where the items get manipulated from

    public float grabDistance = 0.1f;

    // Force at which an object is thrown

    public float throwingMultiplier = 1.5f;

    // To determine if object has been grabbed or cant be grabbed

    public string grabName = "Grabbing";

    // Gets the current position of the controller and its transform values

    public Transform _currentlyHeldObject
    {
        get { return _currentlyHeldObject; }
        set { _currentlyHeldObject = value; }
    }

    // Does as it says and stores the last frame and is used to work out velocity calculations

    private Vector3 _lastFramePosition;

    // Determines the transform values for the currently selected objects

    private Transform _currentObject;

    // bolean to determine if an object is currently being held

    private bool _isHeld;

    // *Hopefully gets the roation of the controllers and matches that to the roation of the item*
    // *private Transform _objectRotation;*

    // Use this for initialization

    void Start()
    {
        // Clearing all variables back to zero

        _currentObject = null;
        _lastFramePosition = transform.position;
        _isHeld = false;

        // Determining the tracking space available

        XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);

    }

    // Update is called once per frame

    void Update()
    {
        // rotation and position tracking. Updates every frame, should be 90fps

        transform.localPosition = InputTracking.GetLocalPosition(nodeType);
        transform.localRotation = InputTracking.GetLocalRotation(nodeType);

        // Check the object in hand variable. If hand is empty, check to see if we can pick anything up (Object highlight posibilites)

        if (_currentObject == null)
        {
            // Check to see if there are any colliders in the proximity of hands

            Collider[] colliders = Physics.OverlapSphere(transform.position, grabDistance);
            if (colliders.Length > 0)
            {
                // If there is a collision, then pickup the object if we press the grab button and give it the tag "Grabbing"

                if (Input.GetAxis(buttonName) >= 0.01F && colliders[0].transform.CompareTag(grabName))
                {
                    // Now we set to grabbing to true

                    if (_isHeld)
                    {
                        return;
                    }
                    _isHeld = true;

                    // Sets the current object to the object we have just picked up and binds it as a child object (this should allow all rotation to mimic the hands

                    colliders[0].transform.SetParent(transform);

                    // If there is no RigidBody attached to the current object, then assign it a RigidBody variable

                    if (colliders[0].GetComponent<Rigidbody>() == null)
                    {
                        colliders[0].gameObject.AddComponent<Rigidbody>();
                    }

                    // Disable all physics on the grabbed object until it is released (May need to change if we want to enable objects to be swung and hit other "live objects")

                    colliders[0].GetComponent<Rigidbody>().isKinematic = true;

                    // Save the current object for a reference later

                    _currentlyHeldObject = colliders[0].transform;

                    // If other hand grabs the object, then release the object

                    if (secondaryHandRef._currentlyHeldObject != null)
                    {
                        secondaryHandRef._currentlyHeldObject = null;
                    }
                }
            }
        }
        else
        // We must now have an object in our hand at this point. We now update its current position to the current hand poistion + the offset definded above

        {
            //If we release the grab button, we drop the object here
            if (Input.GetAxis(buttonName) < 0.01f)
            {

                // Return the value of the object back to Non-Kinematic and re-enable physics
                Rigidbody _objectRB = _currentObject.GetComponent<Rigidbody>();
                _objectRB.isKinematic = false;
                _objectRB.collisionDetectionMode = CollisionDetectionMode.Continuous;

                // Fun stuff starts here, we now calculate the hands current velocity
                Vector3 Velocity = (transform.position - _lastFramePosition) / Time.deltaTime;

                // We now set that object velocity to the current velocity of the hand (I may adjust this to improve physics simulation)
                _objectRB.velocity = Velocity * throwingMultiplier;

                // Kill the reference and revert the hands back to being empty and remove its parent
                _currentObject.SetParent(null);
                _currentObject = null;
            }
        }

        //Release the grab function
        if (Input.GetAxis(buttonName) < 0.01f && _isHeld)
        {
            _isHeld = false;
        }

        // Store the current position of the hand for velocity calculation in the upcoming frame
        _lastFramePosition = transform.position;

        //Tying off all loose ends here
    }
}

