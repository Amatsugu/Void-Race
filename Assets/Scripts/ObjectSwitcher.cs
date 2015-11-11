using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSwitcher : MonoBehaviour {
	public string ButtonText;
	public List<GameObject> Objects = new List<GameObject>();
	private int curObject;
	private ControlMap _Controls;
	void Start()
	{
		_Controls = GameObject.Find("Controls").GetComponent<ControlMap>();
	}
	void Awake()
	{
		
		foreach(GameObject g in Objects)
		{
			g.SetActive(false);
		}
		Objects[curObject].SetActive(true);
	}
	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width - ButtonText.Length * 7, 5, ButtonText.Length * 7, 25), ButtonText) || Input.GetKeyUp(_Controls.GetKey("ChangeShip")))
		{
			foreach(GameObject g in Objects)
			{
				g.SetActive(false);
			}
			curObject++;
			if(curObject > Objects.Count -1)
				curObject = 0;
			Objects[curObject].SetActive(true);
		}
	}
}
