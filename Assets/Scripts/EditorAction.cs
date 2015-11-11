using UnityEngine;
using System.Collections;

public class EditorAction : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		foreach(GameObject g in GameObject.FindObjectsOfType(typeof(GameObject)))
		{
			if(g.name == "Track")
				g.layer = 2;
		}
	}
}
