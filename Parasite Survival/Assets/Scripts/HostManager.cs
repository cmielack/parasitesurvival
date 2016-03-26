using UnityEngine;
using System.Collections;

public class HostManager : MonoBehaviour 
{

	public HostBehavior[] hosts;

	void Start () 
	{
		hosts = FindObjectsOfType<HostBehavior> ();
	}

}
