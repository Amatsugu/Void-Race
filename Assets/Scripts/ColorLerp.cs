using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorLerp : MonoBehaviour {
	
	public int matIndex;
	public Gradient colors;
	public float lerpRate = 1f;
	public bool RandomStartingColor = true;
	public float StartColor = 0;
	private MeshRenderer Mrenderer;
	private float curColor;
	// Use this for initialization
	void Start () {
		Mrenderer = GetComponent<MeshRenderer>();
		Mrenderer.materials[matIndex].SetColor("_Color",colors.Evaluate(0));
		if(RandomStartingColor)
			curColor = Random.Range(Random.Range(0, .5f), Random.Range(0.5f, 1));
		else
			curColor = StartColor;
	}
	
	// Update is called once per frame
	void Update () {
		if(curColor > .999f)
		{
			curColor = 0;
		}
		Mrenderer.materials[matIndex].SetColor("_Color", colors.Evaluate(curColor));
		curColor += Time.deltaTime * lerpRate;
	}
}
