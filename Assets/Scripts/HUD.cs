using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	//HUD
	/*public bool HUD_Active = true;
	public bool Flight = true;
	public bool Stats = true;
	public bool Targeting = true;
	//Stats HUD
	public int FontSize = 16;
	public int FontSize2 = 24;
	public Texture2D HUDFrame;
	public Texture2D HUDHealth;
	public Texture2D HUDEnergy;
	private GUIStyle TextFont;
	private GUIStyle TextFont2;
	private Vector2 screenCenter;
	private Movement Movement;
	private EnergyManager Energy;
	//Targeting HUD
	public Vector2 targetPos;
	public float scale;
	public Texture2D TexBlue1;
	public Texture2D TexRed1;
	public Texture2D TexBlue2;
	public Texture2D TexRed2;
	private bool hasHit;
	private bool isActive;
	private bool isTarget;
	private float TargetingRange = 4000f;
	public Transform thisTransform;
	private ControlMap _Controls;
	private float curRotation;
	//Flight HUD
	public Texture2D BaseTex;
	public Texture2D[] IndicatorTex;
	private InputController InputC;
	void Start()
	{
		TextFont = new GUIStyle();
		TextFont.fontSize = FontSize;
		TextFont2 = new GUIStyle();
		TextFont2.fontSize = FontSize2;
		Movement = GetComponent<Movement>();
		Energy = GetComponent<EnergyManager>();
		screenCenter = new Vector2(Screen.width/2, Screen.height/2);
		_Controls = GameObject.Find("Controls").GetComponent<ControlMap>();
		InputC = GetComponent<InputController>();

	}
	void OnEnable()
	{
		Start();
	}
	void Update()
	{
		if(Targeting)
			TargetingHUDLogic();
	}
	void OnGUI()
	{
		if(HUD_Active)
		{
			if(Stats)
				StatsHUD();
			if(Flight)
				FlightHUD();
			if(Targeting)
				TargetingHUD();
		}
	}
	//Stats HUD
	void StatsHUD()
	{
		if(TextFont == null || TextFont2 == null)
			Start();
		GUI.DrawTexture(new Rect(0, Screen.height - HUDFrame.height, HUDFrame.width, HUDFrame.height), HUDFrame);
		//GUI.DrawTexture(new Rect(0, Screen.height - HUDHealth.height, HUDHealth.width * Movement.HealthRatio, HUDHealth.height), HUDHealth);
		GUI.DrawTexture(new Rect(0, Screen.height - HUDEnergy.height, HUDEnergy.width * Energy.curEnergyRatio, HUDEnergy.height), HUDEnergy);
	}
	//Targeting HUD
	void TargetingHUDLogic()
	{
		RaycastHit hit;
		Physics.Raycast(thisTransform.position, thisTransform.forward,out hit, TargetingRange);
		if(hit.point != Vector3.zero)
		{
			//targetPos = GetComponent<Movement>().thisCam.WorldToScreenPoint(hit.point);
			hasHit = true;
		}else
		{
			hasHit = false;
		}
		targetPos.y = Screen.height - targetPos.y;
		scale = Vector3.Distance(hit.point, thisTransform.position)/TargetingRange;
		scale = 1-scale;
		Debug.DrawLine(thisTransform.position, hit.point);
		if(hit.collider != null)
		{
			if(hit.collider.tag == "Player" || hit.collider.tag == "Ignore")
			{
				isTarget = true;
			}else
			{
				isTarget = false;
			}
		}
		isActive = Input.GetKey(_Controls.GetKey("TargetingMode"));
	}
	void TargetingHUD()
	{
		if(isActive)
		{
			if(hasHit)
			{
				GUIUtility.ScaleAroundPivot(new Vector2(.5f, .5f) * scale, targetPos);
				GUIUtility.RotateAroundPivot(curRotation, targetPos);
				if(isTarget)
				{
					GUI.DrawTexture(new Rect(targetPos.x - TexRed1.width/2, targetPos.y - TexRed1.height/2, TexRed1.width, TexRed1.height), TexRed1);
				}else
				{
					GUI.DrawTexture(new Rect(targetPos.x - TexBlue1.width/2, targetPos.y - TexBlue1.height/2, TexBlue1.width, TexBlue1.height), TexBlue1);
				}
				GUIUtility.RotateAroundPivot(curRotation*-2, targetPos);
				if(isTarget)
				{
					GUI.DrawTexture(new Rect(targetPos.x - TexRed2.width/2, targetPos.y - TexRed2.height/2, TexRed2.width, TexRed2.height), TexRed2);
				}else
				{
					GUI.DrawTexture(new Rect(targetPos.x - TexBlue2.width/2, targetPos.y - TexBlue2.height/2, TexBlue2.width, TexBlue2.height), TexBlue2);
				}
				if(curRotation != 360)
					curRotation += 90 * Time.deltaTime;
				else
					curRotation = 0;
			}
		}
	}
	//Flight HUD
	void FlightHUD()
	{
		float steerMagnitude =  Mathf.Sqrt((InputC.TorqueVector.x * InputC.TorqueVector.x)+(InputC.TorqueVector.y * InputC.TorqueVector.y));
		if(InputC == null)
			Start();
		if(InputC.Inst == null)
			InputC.Inst = GetComponent<Movement>();
		if(!InputC.Inst.isGrounded)
		{
			float vectorAngle = 0;
			if(InputC.TorqueVector != Vector2.zero)
			{
				vectorAngle = Mathf.Atan(InputC.TorqueVector.y/InputC.TorqueVector.x);
			}else
			{
				vectorAngle = 0;
			}
			vectorAngle *= Mathf.Rad2Deg;
			if(InputC.TorqueVector.x < 0)
				vectorAngle*= -1;
			else if(InputC.TorqueVector.y < 0 && InputC.TorqueVector.x > 0)
				vectorAngle-= 180;
			else if(InputC.TorqueVector.y > 0 && InputC.TorqueVector.x > 0)
				vectorAngle-= 180;
			if(InputC.TorqueVector.x > 0 && InputC.TorqueVector.y > 0 && vectorAngle < 0)
				vectorAngle*=-1;
			if(InputC.TorqueVector.x > 0 && InputC.TorqueVector.y < 0 && vectorAngle < 0)
				vectorAngle*=-1;
			if(vectorAngle > 360)
				vectorAngle = 360;
			if(vectorAngle < -360)
				vectorAngle = -360;
			//Debug.Log(vectorAngle + "x: " + InputC.TorqueVector.x + " y: " + InputC.TorqueVector.y);
			GUIUtility.RotateAroundPivot(vectorAngle, screenCenter);
			GUI.DrawTexture(new Rect((Screen.width/2) - (BaseTex.width/2), (Screen.height/2) - (BaseTex.height/2), BaseTex.width, BaseTex.height), BaseTex);
			//80%
			if(steerMagnitude > InputC.FlightSteerClamp * .75)
			{
				GUI.DrawTexture(new Rect(Screen.width/2 - IndicatorTex[3].width/2, Screen.height/2 - IndicatorTex[3].height/2, IndicatorTex[3].width, IndicatorTex[3].height), IndicatorTex[3]);
			}
			//75%
			if(steerMagnitude <= InputC.FlightSteerClamp * .75 && steerMagnitude > InputC.FlightSteerClamp * .5)
			{
				GUI.DrawTexture(new Rect(Screen.width/2 - IndicatorTex[2].width/2, Screen.height/2 - IndicatorTex[2].height/2, IndicatorTex[2].width, IndicatorTex[2].height), IndicatorTex[2]);
			}
			//50%
			if(steerMagnitude <= InputC.FlightSteerClamp * .5 && steerMagnitude > InputC.FlightSteerClamp * .25)
			{
				GUI.DrawTexture(new Rect(Screen.width/2 - IndicatorTex[1].width/2, Screen.height/2 - IndicatorTex[1].height/2, IndicatorTex[1].width, IndicatorTex[1].height), IndicatorTex[1]);
			}
			//25%
			if(steerMagnitude <= InputC.FlightSteerClamp * .25 && steerMagnitude > 0)
			{
				GUI.DrawTexture(new Rect(Screen.width/2 - IndicatorTex[0].width/2, Screen.height/2 - IndicatorTex[0].height/2, IndicatorTex[0].width, IndicatorTex[0].height), IndicatorTex[0]);
			}
			GUIUtility.RotateAroundPivot(-vectorAngle, screenCenter);
		}
	}*/
}