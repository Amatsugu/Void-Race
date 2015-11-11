using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class WaypointReg : MonoBehaviour {

	// Use this for initialization
	public bool debug = true;
	public List<GameObject> wayPoints = new List<GameObject>();
	private List<GameObject> rawWayPoints = new List<GameObject>();
	void Start () 
	{
		rawWayPoints = GameObject.FindGameObjectsWithTag("WP").OrderBy(go => go.name).ToList();
		Sort();
	}
	void Sort()
	{	
		wayPoints.Capacity = rawWayPoints.Count;
		for(int i = 0; i < rawWayPoints.Count; i++)
		{
			wayPoints.Add(null);
		}
		foreach(GameObject g in rawWayPoints)
		{
			string n = g.name;
			n = n.Replace("wp", "");
			wayPoints[int.Parse(n)] = g;
		}
	}
	public GameObject[] GetWayPoints()
	{
		if(wayPoints == null)
			Start();
		return wayPoints.ToArray();
	}
	void Update()
	{
		if(debug)
		{
			foreach(GameObject g in wayPoints)
			{
				int i = wayPoints.IndexOf(g);
				if(i+1 < wayPoints.Count)
					Debug.DrawLine(g.transform.position, wayPoints[i+1].transform.position, Color.green);
				else
					Debug.DrawLine(g.transform.position, wayPoints[0].transform.position, Color.green);
			}
		}else
			return;
	}
}
