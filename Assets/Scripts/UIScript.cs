using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

	public GameObject Inverter;
    public Slider slider;
    public Camera cam;
    public Transform Target;

    Vector3 unUsedInvertorPosition;
    Vector3 dirVec;
    float radius;

    private void Start()
    {
        //Initialize Sun's directionVector and radius from the current camera position
        radius = Vector3.Distance(cam.transform.position, Target.transform.position);
        dirVec = (cam.transform.position - Target.transform.position).normalized;
        unUsedInvertorPosition = new Vector3(-11, 98.2f, 30);
    }

    public void SliderValue()
    {
        //Change light position on slider value change
        float angle = 180 * slider.value;
        cam.transform.position = Target.transform.position + (Quaternion.AngleAxis(angle, Vector3.right) * dirVec) * radius;
        cam.transform.LookAt(Target);
        cam.Render();
    }
	
    public void AddInverter ()
	{
        //Add Inverter 
        Instantiate(Inverter, unUsedInvertorPosition, new Quaternion(0,0,0,0));
	}
}

//Explanation
//In Unity a way to implement light is to actually get a camera to perform as the light.
//On slider change the sun position changes from sunrise till sunset because of the 180 degree multiplication.
//The camera and the shadowmap generated from the camera are not updated every frame but only on the slide value change.
