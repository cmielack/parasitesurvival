using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour {

	float baseSpeed = 2f;

	float latchRange = 2f;

	HostManager hostManager;

	HostBehavior currentHost;

	// Use this for initialization
	void Start () {
		hostManager = FindObjectOfType<HostManager> ();
		currentHost = null;
	}
	
	// Update is called once per frame
	void Update () {
		ProcessAxisInput ();	
		ProcessGrabbing ();
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
				currentHost = null;
			}
		}
	}

	void LatchOnTo(HostBehavior host)
	{
		currentHost = host;

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
}
