using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent (typeof(AudioSource))]
public class SoundCollison : MonoBehaviour {
	class SoundCollisionData
	{
		public string ColliderTag;
		public AudioClip Sound;
		public bool hasEffect;
		public GameObject Effect;
	}
	public List<string> ColliderTags = new List<string>();
	public List<AudioClip> Sounds = new List<AudioClip>();
	public List<bool> hasEffects = new List<bool>();
	public List<GameObject> Effects = new List<GameObject>();
	public bool isCharController = false;
	private List<SoundCollisionData> SoundData = new List<SoundCollisionData>();
	// Use this for initialization
	void Start () 
	{
		foreach( string s in ColliderTags)
		{
			//Debug.Log(s);
			int index = ColliderTags.IndexOf(s);
			SoundCollisionData newNode = new SoundCollisionData();
			newNode.ColliderTag = s;
			newNode.Sound = Sounds[index];
			newNode.hasEffect = hasEffects[index];
			newNode.Effect = Effects[index];
			SoundData.Add(newNode);
		}
	}
	
	// Update is called once per frame
	void OnControllerColliderHit (ControllerColliderHit c)
	{
		if(isCharController)
		{
//			Debug.Log("Collided with: " + c.collider.gameObject.name + " : " + c.collider.tag);
			foreach(SoundCollisionData d in SoundData)
			{
				if(d.ColliderTag == c.collider.tag)
				{
					if(!audio.isPlaying)
					{
						audio.clip = d.Sound;
						audio.pitch = Random.Range(1, 3);
						audio.Play();
						if(d.hasEffect)
						{
							Instantiate(d.Effect, c.point, Quaternion.identity);
						}
					}
				}
			}
		}
		
	}
	void OnCollisionEnter (Collision c)
	{
//		Debug.Log("Collided with: " + c.collider.gameObject.name + " : " + c.collider.tag);
		foreach(SoundCollisionData d in SoundData)
		{
			if(d.ColliderTag == c.collider.tag)
			{
				if(!audio.isPlaying)
				{
					audio.clip = d.Sound;
					audio.pitch = Random.Range(1, 3);
					audio.Play();
					if(d.hasEffect)
					{
						Instantiate(d.Effect, c.contacts[Random.Range(0, c.contacts.Length -1)].point, Quaternion.identity);
					}
				}
			}
		}
		
	}
	void OnTriggerEnter(Collider c)
	{
//		Debug.Log("Passed with: " + c.name + " : " + c.tag);
		foreach(SoundCollisionData d in SoundData)
		{
			if(d.ColliderTag == c.tag)
			{
				if(!audio.isPlaying)
				{
					audio.clip = d.Sound;
					audio.pitch = Random.Range(1, 3);
					audio.Play();
					if(d.hasEffect)
					{
						Instantiate(d.Effect, new Vector3(transform.position.x, c.transform.position.y, transform.position.z), Quaternion.identity);
					}
				}
			}
		}
	}
}
