using UnityEngine;
using System.Collections;

public class DestroyKeep : MonoBehaviour {
	
	public GameObject[] This;
	public GameObject[] That;
	public bool isRandom;
	public string message;
	private bool isActive =	true;
	private int curItem;
	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width + 5 - message.Length * 7, 5, message.Length * 7, 20), message))
		{
			if(isActive)
			{
				if(isRandom)
				{
					This[Random.Range(0,This.Length)].SetActive(false);
					That[Random.Range(0,That.Length)].SetActive(true);
				}else
				{
					if(curItem >= That.Length)
						curItem = 0;
					foreach(GameObject g in This)
					{
						g.SetActive(false);
					}
					That[curItem].SetActive(true);
					curItem++;
					if(curItem >= That.Length)
						curItem = 0;
				}
			}else
			{
				foreach(GameObject g in This)
				{
					g.SetActive(true);
				}
				foreach(GameObject g in That)
				{
					g.SetActive(false);
				}
			}
			isActive = !isActive;
		}
	}
}
