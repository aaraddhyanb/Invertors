using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public float rotationSpeed;
	public Transform target;

    Vector3 dirVec;
    float radius;

    private void Start()
    {
        //Initialize Main camera's direction and radius from the current camera position
        radius = Vector3.Distance(transform.position, target.transform.position);
        dirVec = (transform.position - target.transform.position).normalized;

        //Set the camera once to this direction and radius to avoid snapping 
        transform.position = target.transform.position + (dirVec * radius);
        dirVec = (transform.position - target.transform.position).normalized;
        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 axisToRotateAround = Vector3.Cross(Vector3.up, dirVec).normalized;
            float dot = Vector3.Dot(dirVec, Vector3.up);
         
            //clamp movement from getting exactly on top of the target and also below the surface
            if( (dot < 0.99f || Input.GetAxis("Mouse Y") > 0) && (dot > 0.01f || Input.GetAxis("Mouse Y") < 0))
            {
                transform.position = target.transform.position + (Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * rotationSpeed, axisToRotateAround) * dirVec) * radius;
                dirVec = (transform.position - target.transform.position).normalized;
            }
            transform.position = target.transform.position + (Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up) * dirVec) * radius;
            dirVec = (transform.position - target.transform.position).normalized;  

            transform.LookAt(target);
        }
    }
}

//Explanation
//I felt there were multiple ways to implement the camera movement from the description given in the test.
//Out of all those implementations, the most intuitive and simple way was to rotate camera around the house taking mouse drag into consideration.
