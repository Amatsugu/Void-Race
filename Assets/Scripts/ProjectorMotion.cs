using UnityEngine;
using System.Collections;

public class ProjectorMotion : MonoBehaviour {
	private Vector3 MinArea;
	private Vector3 MaxArea;
	private float Speed;
	private float offset;
	private Transform ThisTransform;
	private bool xDisp, yDisp, zDisp;
	void Start()
	{
		ThisTransform = transform;
	}
	public void SetArea(Vector3 min, Vector3 max, float speed, float off, float lenght, float width, bool xd, bool yd, bool zd)
	{
		MinArea = min;
		MaxArea = max;
		Speed = speed;
		offset = off;
		GetComponent<TrailRenderer>().time = lenght;
		GetComponent<TrailRenderer>().startWidth = width;
		Bounce();
		xDisp = xd;
		yDisp = yd;
		zDisp = zd;
	}
	void Update()
	{
		if(ThisTransform.position.x > MaxArea.x)
		{
			Bounce();
		}
		if(ThisTransform.position.y > MaxArea.y)
		{
			Bounce();
		}
		if(ThisTransform.position.z > MaxArea.z)
		{
			Bounce();
		}
		if(ThisTransform.position.x < MinArea.x)
		{
			Bounce();
		}
		if(ThisTransform.position.y < MinArea.y)
		{
			Bounce();
		}
		if(ThisTransform.position.z < MinArea.z)
		{
			Bounce();
		}
		ThisTransform.Translate(transform.forward * Speed * Time.deltaTime);
	}
	void Bounce()
	{
		if(ThisTransform == null)
			ThisTransform = transform;
		if(xDisp && !yDisp && !zDisp)
			ThisTransform.LookAt(new Vector3(Random.Range(MinArea.x+offset, MaxArea.x-offset), 0, 0));
		if(!xDisp && yDisp && !zDisp)
			ThisTransform.LookAt(new Vector3(0, Random.Range(MinArea.y+offset, MaxArea.y-offset), 0));
		if(!xDisp && !yDisp && zDisp)
			ThisTransform.LookAt(new Vector3(0, 0, Random.Range(MinArea.z+offset, MaxArea.z-offset)));
		
		if(xDisp && yDisp && !zDisp)
			ThisTransform.LookAt(new Vector3(Random.Range(MinArea.x+offset, MaxArea.x-offset), Random.Range(MinArea.y+offset, MaxArea.y-offset), 0));
		if(!xDisp && yDisp && zDisp)
			ThisTransform.LookAt(new Vector3(0, Random.Range(MinArea.y+offset, MaxArea.y-offset), Random.Range(MinArea.z+offset, MaxArea.z-offset)));
		if(xDisp && !yDisp && zDisp)
			ThisTransform.LookAt(new Vector3(Random.Range(MinArea.x+offset, MaxArea.x-offset), 0, Random.Range(MinArea.z+offset, MaxArea.z-offset)));
		
		if(xDisp && yDisp && zDisp)
			ThisTransform.LookAt(new Vector3(Random.Range(MinArea.x+offset, MaxArea.x-offset), Random.Range(MinArea.y+offset, MaxArea.y-offset), Random.Range(MinArea.z+offset, MaxArea.z-offset)));
		
	}
}
