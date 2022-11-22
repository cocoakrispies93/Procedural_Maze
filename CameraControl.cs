using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public GameObject player;

	private Vector3 offset, storeOffset, storeRotation;

    public bool trackballOn;

    bool mouseDrag = false;
    Vector3 mouseStart;

	// Use this for initialization
	void Start () {
		offset = transform.position - player.transform.position;
	}
	
	// LateUpdate is called once per frame after all other updates have been processed,
	// such as the player's position. 
	void LateUpdate () {
		transform.position = player.transform.position + offset;
	}

    // Trackball function implemented here
    void Update()
    {
        Vector3 mouseCurrent, currentRotation;
        float deltay;
        float scaleAngle = 0.05f;
        if (trackballOn)
        { 
            // use scroll to zoom in and out
            if (Input.mouseScrollDelta.y < 0)
                offset.Scale(new Vector3(1.1f, 1.1f, 1.1f));
            else if (Input.mouseScrollDelta.y > 0)
                offset.Scale(new Vector3(0.9f, 0.9f, 0.9f));

            if (Input.GetMouseButtonDown(0)) // left mouse button down
            {
                mouseDrag = true; // start dragging the mouse
                mouseStart = Input.mousePosition;
                storeOffset = offset; // store the initial offset
                storeRotation = new Vector3(0f, 0f, 0f); // no previous rotation yet
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseDrag = false; // end dragging the mouse
            }
            else if (mouseDrag) { // we're currently dragging the mouse around
                mouseCurrent = Input.mousePosition;
                // find the vertical difference in mouse position
                deltay = mouseCurrent.y - mouseStart.y; 
                //rotate around x proportional with the mouse delta
                currentRotation = new Vector3(-deltay* scaleAngle, 0f, 0f); 
                // apply it to the initial offset when we started dragging the mouse
                offset = Quaternion.Euler(currentRotation) * storeOffset;
                transform.Rotate(storeRotation);        // reset the camera rotation
                transform.Rotate(currentRotation);      // then rotate it by the new angle
                storeRotation = currentRotation * (-1); // update the reset rotation
            }
        }
    }
}
