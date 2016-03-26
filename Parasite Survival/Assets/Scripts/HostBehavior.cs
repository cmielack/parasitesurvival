using UnityEngine;
using System.Collections;

public class HostBehavior : MonoBehaviour 
{

	private ActionMode mode;

	public float walkSpeed = 1.0f;

	private float arrivedTolerance = 0.01f;

	private Vector3 currentGoal;

	void Start () 
	{
		mode = ActionMode.Rest;
		currentGoal = Vector3.zero;
	}
	
	void Update () 
	{
		ProcessInput ();

		if (mode == ActionMode.Walk) {
			GoTo (currentGoal);
		}
		else if (mode == ActionMode.Rest){
			//...
		}
	}

	void ProcessInput(){
		if (mode == ActionMode.Rest) {
			PickRandomGoal ();
			if(Input.GetKey(KeyCode.Space)){
				mode = ActionMode.Walk;
			}

		} else if (mode == ActionMode.Walk) {

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

	void PickRandomGoal ()
	{
		currentGoal = new Vector3 (Random.Range (-8.5f, 8.5f), 0, Random.Range (-4.5f, 4.5f));
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