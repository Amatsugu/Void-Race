using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HaloGram : MonoBehaviour {
	public Vector3 MinArea;
	public Vector3 MaxArea;
	public float Speed = 10f;
	public float offset = 2f;
	public float TrailLenght = 1f;
	public float TrailWidth = .1f;
	public GameObject ProjectionLines;
	public int MaxProjectors;
	private bool showOptions = false;
	private bool showAdvanced = false;
	private bool xDisp = false;
	private bool yDisp = true;
	private bool zDisp = false;
	private float cubicArea = 20;
	private List<GameObject> Projectors = new List<GameObject>();
	// Use this for initialization
	void Start () 
	{
		for( int i = MaxProjectors; i > 0; i--)
		{
			Projectors.Add(Instantiate(ProjectionLines, 
				new Vector3(Random.Range(MinArea.x+offset, MaxArea.x-offset), 
				Random.Range(MinArea.y-offset, MaxArea.y+offset), 
				Random.Range(MinArea.z-offset, MaxArea.z+offset)), 
				Quaternion.identity) as GameObject);
		}
		SetProjectors();
	}
	void OnGUI()
	{
		showOptions = GUILayout.Toggle(showOptions, "Show Options");
		if(showOptions)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginVertical();
			GUILayout.Space(10);
			GUILayout.Label("Speed:");
			float.TryParse(GUILayout.TextField(Speed.ToString()), out Speed);
			GUILayout.Label("Trail Lenght:");
			float.TryParse(GUILayout.TextField(TrailLenght.ToString()), out TrailLenght);
			GUILayout.Label("Trail Width:");
			float.TryParse(GUILayout.TextField(TrailWidth.ToString()), out TrailWidth);
			GUILayout.Label("Offset");
			float.TryParse(GUILayout.TextField(offset.ToString()), out offset);
			showAdvanced = GUILayout.Toggle(showAdvanced, "Advanced Options");
			if(showAdvanced)
			{
				GUILayout.Label("Min Area");
				GUILayout.BeginHorizontal();
				GUILayout.Label("X: ");
				float.TryParse(GUILayout.TextField(MinArea.x.ToString()), out MinArea.x);
				GUILayout.Label("Y: ");
				float.TryParse(GUILayout.TextField(MinArea.y.ToString()), out MinArea.y);
				GUILayout.Label("Z: ");
				float.TryParse(GUILayout.TextField(MinArea.z.ToString()), out MinArea.z);
				GUILayout.EndHorizontal();
				GUILayout.Label("Max Area");
				GUILayout.BeginHorizontal();
				GUILayout.Label("X: ");
				float.TryParse(GUILayout.TextField(MaxArea.x.ToString()), out MaxArea.x);
				GUILayout.Label("Y: ");
				float.TryParse(GUILayout.TextField(MaxArea.y.ToString()), out MaxArea.y);
				GUILayout.Label("Z: ");
				float.TryParse(GUILayout.TextField(MaxArea.z.ToString()), out MaxArea.z);
				GUILayout.EndHorizontal();
				GUILayout.Label("Dispersal Mode:");
				GUILayout.BeginHorizontal();
				xDisp = GUILayout.Toggle(xDisp, "X Dispersal");
				yDisp = GUILayout.Toggle(yDisp, "Y Dispersal");
				zDisp = GUILayout.Toggle(zDisp, "Z Dispersal");
				GUILayout.EndHorizontal();
			}else
			{
				GUILayout.Label("Cubic Area:");
				float.TryParse(GUILayout.TextField(cubicArea.ToString()), out cubicArea);
			}
			if(GUILayout.Button("Set"))
			{
				if(!showAdvanced)
				{
					MinArea = new Vector3(-cubicArea/2, -cubicArea/2, -cubicArea/2);
					MaxArea = new Vector3(cubicArea/2, cubicArea/2, cubicArea/2);
				}else
				{
					float MinAvg = (MinArea.x + MinArea.y + MinArea.z)/3;
					if(MinAvg < 0)
						MinAvg *= -1;
					float MaxAvg = (MaxArea.x + MaxArea.y + MaxArea.z)/3;
					if(MaxAvg <  0)
						MaxAvg *= -1;
					cubicArea = (MinAvg + MaxAvg)/2;
					cubicArea *= 2;
					if(cubicArea < 0)
					{
						cubicArea *= -1;
					}
				}
				SetProjectors();
			}
			GUILayout.Label("Number of Lines:");
			int.TryParse(GUILayout.TextField(MaxProjectors.ToString()), out MaxProjectors);
			if(GUILayout.Button("Reset"))
			{
				Reset();
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}
	void Reset()
	{
		foreach(GameObject g in Projectors)
		{
			Destroy(g);
		}
		Projectors.Clear();
		for( int i = MaxProjectors; i > 0; i--)
		{
			Projectors.Add(Instantiate(ProjectionLines, 
				new Vector3(Random.Range(MinArea.x+offset, MaxArea.x-offset), 
				Random.Range(MinArea.y-offset, MaxArea.y+offset), 
				Random.Range(MinArea.z-offset, MaxArea.z+offset)), 
				Quaternion.identity) as GameObject);
		}
		SetProjectors();
	}
	void SetProjectors()
	{
		//Camera.main.GetComponent<MouseOrbit>().MaxDist = cubicArea * 5;
		foreach( GameObject g in Projectors)
		{
			g.GetComponent<ProjectorMotion>().SetArea(MinArea, MaxArea, Speed, offset, TrailLenght, TrailWidth, xDisp, yDisp, zDisp);
		}
	}
}
