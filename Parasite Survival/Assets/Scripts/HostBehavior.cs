using UnityEngine;
using System.Collections;

public class HostBehavior : MonoBehaviour 
{

	private ActionMode mode;

	public float walkSpeed = 1.0f;

	private float arrivedTolerance = 0.01f;

	void Start () 
	{
		mode = ActionMode.Rest;
	}
	
	void Update () 
	{
		if (Input.GetKey (KeyCode.Space)) {
			mode = ActionMode.Walk;
			GoTo (new Vector3 (-6,0,3));
		}
		else{
			mode = ActionMode.Rest;
		}
	}

	void GoTo(Vector3 pos)
	{
		Vector3 diff = pos - transform.position;

		Vector3 step = diff.normalized * walkSpeed * Time.deltaTime;

		if (diff.magnitude > step.magnitude) {
			transform.position += step;
		} else {
			transform.position = pos;
		}

		if ((transform.position - pos).magnitude <= arrivedTolerance) 
		{
			Arrive ();
		}
	}

	void Arrive()
	{
		mode = ActionMode.Rest;
	}

}


public enum ActionMode 
{
	Rest,
	Walk,
	Shake
}