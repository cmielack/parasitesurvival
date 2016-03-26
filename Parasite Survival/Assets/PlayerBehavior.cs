using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour {

	float baseSpeed = 2f;

	float latchRange = 2f;

	float forceConstant = 100f;

	HostManager hostManager;

	HostBehavior currentHost;

	Rigidbody rigidbody;

	float health;
	float healthSuckRate = 5f;
	float healthDecayRate = 1f;
	float healthGainRatio = 0.2f;

	// Use this for initialization
	void Start () {
		hostManager = FindObjectOfType<HostManager> ();
		currentHost = null;
		rigidbody = GetComponent<Rigidbody> ();
		health = 20f;
	}
	
	// Update is called once per frame
	void Update () {
		ProcessAxisInput ();	
		ProcessGrabbing ();

		AddLatchForce ();

		UpdateHealth ();
	}


	void ProcessAxisInput(){
		var inputVector = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		transform.position = transform.position + inputVector.normalized * baseSpeed * Time.deltaTime;
	}

	void ProcessGrabbing(){
		if (currentHost == null) {
			if (Input.GetButton ("Fire1")) {
				var nearestHost = GetNearestHost ();
				if ((transform.position - nearestHost.transform.position).magnitude <= latchRange) {
					LatchOnTo (nearestHost);
				}
			}
		} else { // no current host
			Debug.DrawLine (transform.position, currentHost.transform.position);

			if (!Input.GetButton ("Fire1")) {
				Release ();
			}
		}
	}

	void LatchOnTo(HostBehavior host)
	{
		currentHost = host;
		rigidbody.isKinematic = false;
		currentHost.isBeingLatched = true;
	}

	void Release(){
		currentHost.isBeingLatched = false;
		currentHost = null;
		Invoke ("BackToWalkMode", 1);
	}

	void BackToWalkMode()
	{
		rigidbody.isKinematic = true;

	}

	HostBehavior GetNearestHost(){
		float closestDistance = 10000f;
		HostBehavior closest = null;

		foreach (HostBehavior host in hostManager.hosts) {
			var diff = host.transform.position - transform.position;
			if (diff.magnitude < closestDistance) {
				closest = host;
				closestDistance = diff.magnitude;
			}
		}

		return closest;
	}


	public void AddLatchForce()
	{
		if (currentHost != null) {
			var diff = currentHost.transform.position + Vector3.up - transform.position;

			var force = diff * forceConstant;

			rigidbody.AddForce (force);
		}
	}

	public void UpdateHealth()
	{
		if (currentHost != null) {
			var healthSucked = Mathf.Min (Time.deltaTime * healthSuckRate, currentHost.health);
			currentHost.health -= healthSucked;
			health += healthSucked * healthGainRatio;
		} else {
			health -= healthDecayRate * Time.deltaTime;
		}
	}
}
