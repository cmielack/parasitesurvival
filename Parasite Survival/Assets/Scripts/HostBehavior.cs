using UnityEngine;
using System.Collections.Generic;

public class HostBehavior : MonoBehaviour 
{

	private ActionMode mode;

	public float walkSpeed = 1.0f;
	public float waitTime = 4.0f;

	private float arrivedTolerance = 0.01f;

	private Vector3 currentGoal;

	private Shake currentShake;

	private List<Command> commandQueue;

	void Start () 
	{
		mode = ActionMode.Shake;
		currentGoal = Vector3.zero;
		currentShake = null;
	}
	
	void Update () 
	{
		ProcessInput ();

		if (mode == ActionMode.Walk) {
			GoTo (currentGoal);
		} 
		else if (mode == ActionMode.Shake) {
			DoShake ();
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
		currentGoal = new Vector3 (Random.Range (-8.5f, 8.5f), 0, Random.Range (-4.0f, 4.0f));
	}


	void Arrive()
	{
		mode = ActionMode.Rest;
		Invoke ("SetInMotion", waitTime);
	}

	void SetInMotion(){
		mode = ActionMode.Walk;
		PickRandomGoal ();
	}


	void DoShake()
	{
		if (currentShake == null) {
			// initialise
			currentShake = new Shake ();
			currentShake.Randomize ();
			currentShake.Start (transform.position);
		} else {
			currentShake.Update ();
			transform.position = currentShake.position;

			if (currentShake.IsDone ())
				currentShake = null;
		}
	}


}


public enum ActionMode 
{
	Rest,
	Walk,
	Shake
}


public class Shake {
	public Vector3 direction;
	public float amplitude;
	public float progress;
	public float length;

	public float duration;
	public float timeStarted;

	public Vector3 startPoint;
	public Vector3 endPoint;
	public Vector3 position;

	public int countRemaining;

	public void Randomize()
	{
		float angle = Random.Range (0, 2.0f * Mathf.PI);
		direction = new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle));
		amplitude = 20.0f;
		progress = 0;
		length = Random.Range (1.0f, 2.0f);
		timeStarted = -1.0f;
		duration = 0.3f;
		countRemaining = 3;
	}

	public void Start(Vector3 pos)
	{
		timeStarted = Time.realtimeSinceStartup;
		startPoint = pos - direction * length*0.5f;
		endPoint   = pos + direction * length*0.5f;
	}

	public void Update()
	{
		progress = (Time.realtimeSinceStartup - timeStarted) / duration;
		position = startPoint + (endPoint - startPoint) * progress;

		if (progress > 1.0f)
			countRemaining -= 1;
	}

	public bool IsDone(){
		return countRemaining <= 0;
	}

}


public class Command
{
	private string type;
	private float startTime;
	private float duration;
	private Vector3 goal;
	private float speed;

}