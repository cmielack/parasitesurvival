using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour {

	float baseSpeed = 0.5f;

	float latchRange = 2f;

	float forceConstant = 100f;

	HostManager hostManager;

	HostBehavior currentHost;

	Rigidbody rigidbody;

	float health;
	float healthSuckRate = 10f;
	float healthDecayRate = 5f;
	float healthGainRatio = 0.3f;

	public AudioClip landingSound;
	public AudioClip grabbingSound;
	public AudioClip deathSound;

	public GameObject[] healthBar;


	// Use this for initialization
	void Start () {
		hostManager = FindObjectOfType<HostManager> ();
		currentHost = null;
		rigidbody = GetComponent<Rigidbody> ();
		health = 100f;

		healthBar = new GameObject[10];
		for (int i = 0; i < healthBar.Length; i++) {
			healthBar [i] = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			healthBar [i].transform.SetParent (transform);
			healthBar [i].transform.localPosition = new Vector3 (i * 0.1f - 0.5f, -0.10f, -0.8f);
			healthBar [i].transform.localScale = Vector3.one * 0.1f;
			healthBar [i].transform.GetComponent<MeshRenderer> ().material = Resources.Load ("Materials/HealthBarPlayer") as Material;
		}

	}
	
	// Update is called once per frame
	void Update () {
		ProcessAxisInput ();	
		ProcessGrabbing ();

		AddLatchForce ();

		UpdateHealth ();
		UpdateHealthBar ();
	}

	void UpdateHealthBar()
	{
		int blobs = (int) Mathf.Ceil(health / 10.0f);
		for (int i = 0; i < healthBar.Length; i++) {
			healthBar [i].GetComponent<MeshRenderer> ().enabled = i <= blobs;
		}
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

		GetComponent<AudioSource> ().PlayOneShot (grabbingSound);
	}

	void Release(){
		currentHost.isBeingLatched = false;
		currentHost = null;
	}

	void OnCollisionEnter(Collision col){
		if (col.other.GetComponent<HostBehavior> () == null) {
			Land ();
		}
	}

	void Land()
	{
		rigidbody.isKinematic = true;
		GetComponent<AudioSource> ().PlayOneShot (landingSound);

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
			var diff = currentHost.transform.position + Vector3.up*1.5f - transform.position;

			var force = diff * forceConstant;

			rigidbody.AddForce (force);

			if (currentHost.health > currentHost.startShakingHealth) {
				rigidbody.velocity *= 0.8f;
			}
		}
	}

	public void UpdateHealth()
	{
		if (currentHost != null) {
			var healthSucked = Mathf.Min (Time.deltaTime * healthSuckRate, currentHost.health);
			currentHost.health -= healthSucked;
			health += healthSucked * healthGainRatio;

			if (health > 100f)
				health = 100f;
		} else {
			health -= healthDecayRate * Time.deltaTime;
		}

		if (health <= 0) {
			Die ();
		}
	}


	public void Die()
	{
		GetComponent<AudioSource> ().PlayOneShot (deathSound);
	}
}
