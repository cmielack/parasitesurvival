using UnityEngine;
using System.Collections.Generic;

public class HostBehavior : MonoBehaviour 
{

	public float walkSpeed = 1.0f;

	private List<Command> commandQueue;

	public float health;
	public float startShakingHealth;
	public float healthRegenRate = 1.0f;

	public GameObject[] healthBar;

	public bool isBeingLatched;

	void Start () 
	{
		commandQueue = new List<Command> ();

		health = 100.0f;
		startShakingHealth = 50.0f;

		healthBar = new GameObject[10];
		for (int i = 0; i < healthBar.Length; i++) {
			healthBar [i] = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			healthBar [i].transform.SetParent (transform);
			healthBar [i].transform.localPosition = new Vector3 (i * 0.1f - 0.5f, -0.10f, -0.8f);
			healthBar [i].transform.localScale = Vector3.one * 0.1f;
			healthBar [i].transform.GetComponent<MeshRenderer> ().material = Resources.Load ("Materials/HealthBar") as Material;
		}

		isBeingLatched = false;
		
	}
	
	void Update () 
	{
		ProcessInput ();
		UpdateHealth ();
		UpdateHealthBar ();
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
			if (health < startShakingHealth && isBeingLatched) {
				Shake ();
			} else {
				Idle ();
			}
		}
	}

	void ProcessInput(){
		/*if (Input.GetKeyDown (KeyCode.Space)) {
			Shake ();
		}*/
	}

	void Idle()
	{
		commandQueue.Add (new Command ("walk", new Vector3 (Random.Range (-7.5f, 7.5f), 0, Random.Range (-4.0f, 4.0f)),
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
		float slowSpeed = 1.0f;
		float fastSpeed = 50.0f;

		Vector3 startPosition = transform.position;

		commandQueue.Clear ();

		commandQueue.Add (new Command ("walk", startPosition - direction * distance, slowSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition + direction * distance, fastSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition - direction * distance, slowSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition + direction * distance, fastSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition - direction * distance, slowSpeed, 0));
		commandQueue.Add (new Command ("walk", startPosition + direction * distance, fastSpeed, 0));

		commandQueue.Add (new Command ("wait", Vector3.zero, 0, 3.0f));

	}

	void UpdateHealth()
	{
		if (!isBeingLatched && health != 100.0f) {
			health += Time.deltaTime * healthRegenRate;
		}

		if (health > 100.0f) {
			health = 100.0f;
		}
	}

	void UpdateHealthBar()
	{
		int blobs = (int) Mathf.Ceil(health / 10.0f);
		for (int i = 0; i < healthBar.Length; i++) {
			healthBar [i].GetComponent<MeshRenderer> ().enabled = i <= blobs;
		}
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