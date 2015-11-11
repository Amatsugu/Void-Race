using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	public Texture2D SplashImage;
	public Texture2D WaterMark;
	public float Delay = 2;
	public float WaterMarkScale = .2f;
	public float curScale = 1;
	private float startTime;
	// Use this for initialization
	void Start () 
	{
		if(SplashImage == null)
		{
			Debug.LogWarning("No Splash Image, Generating One...");
			SplashImage = new Texture2D(Screen.width, Screen.height);
		}
		if(WaterMark == null)
		{
			Debug.LogWarning("No Watermark Image, Generating One...");
			WaterMark = new Texture2D(Screen.width, Screen.height);
		}
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void OnGUI() 
	{
		if(curScale > WaterMarkScale)
			GUI.DrawTexture(new Rect(0,Screen.height - (Screen.height * curScale),Screen.width * curScale, Screen.height * curScale), SplashImage, ScaleMode.ScaleAndCrop, true);
		if(startTime + Delay <= Time.time)
		{
			curScale = Mathf.Lerp(curScale, WaterMarkScale, Time.deltaTime);
			if(curScale -.001f <= WaterMarkScale)
				curScale = WaterMarkScale;
			if(curScale <= WaterMarkScale)
				GUI.DrawTexture(new Rect(0,Screen.height - (Screen.height * WaterMarkScale),Screen.width * WaterMarkScale, Screen.height * WaterMarkScale), WaterMark, ScaleMode.ScaleAndCrop, true);
		}
	}
}
