using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour {

	public GameObject text;
	public float theTime;

	public bool timerRun = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (timerRun)
			theTime = Mathf.Round (Time.realtimeSinceStartup);
		text.GetComponent<UnityEngine.UI.Text> ().text = theTime.ToString();
	}
}
