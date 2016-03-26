using UnityEngine;
using System.Collections;

public class HostManager : MonoBehaviour 
{

	public HostBehavior[] hosts;

	void Start () 
	{
		UpdateHosts ();
	}


	public void UpdateHosts()
	{
		hosts = FindObjectsOfType<HostBehavior> ();

	}

}
