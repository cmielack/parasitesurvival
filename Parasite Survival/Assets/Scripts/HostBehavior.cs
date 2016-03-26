using UnityEngine;
using System.Collections.Generic;

public class HostBehavior : MonoBehaviour 
{

	public float walkSpeed = 1.0f;

	private List<Command> commandQueue;

	void Start () 
	{
		commandQueue = new List<Command> ();
		
	}
	
	void Update () 
	{
		ProcessInput ();
		if (commandQueue.Count > 0) {
			var currentCommand = commandQueue [0];

			currentCommand.Activate ();

			if (currentCommand.type == "walk") {
				GoTo (currentCommand);
				if ((currentCommand.goal - transform.position).magnitude < 0.01f) {
					commandQueue.RemoveAt (0);
				}
			}
			else if (currentCommand.type == "wait") {
				// do nothing
				if (Time.realtimeSinceStartup - currentCommand.startTime > currentCommand.duration) {
					commandQueue.RemoveAt (0);
				}
			}

		} else {
			Idle ();
		}
	}

	void ProcessInput(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			Shake ();
		}
	}

	void Idle()
	{
		commandQueue.Add (new Command ("walk", new Vector3 (Random.Range (-8.5f, 8.5f), 0, Random.Range (-4.0f, 4.0f)),
			walkSpeed, 0));
		commandQueue.Add (new Command ("wait", Vector3.zero, 0, 1.0f));
	}

	void GoTo(Command comm)
	{
		Vector3 diff = comm.goal - transform.position;

		Vector3 step = diff.normalized * comm.speed * Time.deltaTime;

		if (diff.magnitude > step.magnitude) {
			transform.position += step;
		} else {
			transform.position = comm.goal;
		}
	}

	void Shake()
	{
		float angle = Random.Range (0f, Mathf.PI);
		Vector3 direction = new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle));
		float distance = 1f;
		float slowSpeed = 2.0f;
		float fastSpeed = 25.0f;

		Vector3 startPosition = transform.position;

		commandQueue.Clear ();

		commandQueue.Add (new Command ("walk", startPosition - direction * distance, slowSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition + direction * distance, fastSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition - direction * distance, slowSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition + direction * distance, fastSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition - direction * distance, slowSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition + direction * distance, fastSpeed, 0));

		commandQueue.Add (new Command ("wait", Vector3.zero, 0, 2));

	}
}


public enum ActionMode 
{
	Rest,
	Walk,
	Shake
}




public class Command
{
	public string type;
	public float startTime;
	public float duration;
	public Vector3 goal;
	public float speed;

	private bool isActivated;	

	public Command(string theType, Vector3 theGoal, float theSpeed, float theDuration)
	{
		type = theType;

		isActivated = false;
		speed = theSpeed;
		duration = theDuration;
		goal = theGoal;
	}

	public void Activate(){
		if (!isActivated) {
			startTime = Time.realtimeSinceStartup;
			isActivated = true;
		}
	}
}