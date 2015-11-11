using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DataMap : DefaultData {

	public List<string> DataName = new List<string>();
	public List<string> DataType = new List<string>();
	public List<string> DataValue = new List<string>();
	
	public void RegisterData(string Name, bool data)
	{
		if(DataName.Contains(Name))
		{
			int index = DataName.IndexOf(Name);
			DataType[index] = "Bool";
			DataValue[index] = data.ToString();
		}else
		{
			DataName.Add(Name);
			DataType.Add("Bool");
			DataValue.Add(data.ToString());
		}
	}
	public void RegisterData(string Name, int data)
	{
		if(DataName.Contains(Name))
		{
			int index = DataName.IndexOf(Name);
			DataType[index] = "Int";
			DataValue[index] = data.ToString();
		}else
		{
			DataName.Add(Name);
			DataType.Add("Int");
			DataValue.Add(data.ToString());
		}
	}
	public void RegisterData(string Name, float data)
	{
		if(DataName.Contains(Name))
		{
			int index = DataName.IndexOf(Name);
			DataType[index] = "Float";
			DataValue[index] = data.ToString();
		}else
		{
			DataName.Add(Name);
			DataType.Add("Float");
			DataValue.Add(data.ToString());
		}
	}
	public void RegisterData(string Name, string data)
	{
		if(DataName.Contains(Name))
		{
			int index = DataName.IndexOf(Name);
			DataType[index] = "String";
			DataValue[index] = data;
		}else
		{
			DataName.Add(Name);
			DataType.Add("String");
			DataValue.Add(data);
		}
	}
	public void Clear()
	{
		DataName.Clear();
		DataType.Clear();
	}
	public void Reset()
	{
		DataMap tmp = RegisterDefaultData(this);
		DataName = tmp.DataName;
		DataType = tmp.DataType;
	}
	public bool GetBool(string Name)
	{
		bool data;
		if(!DataName.Contains(Name))
		{
			Reset();
		}
		bool.TryParse(DataValue[DataName.IndexOf(Name)], out data);
		return data;
	}
	public int GetInt(string Name)
	{
		int data;
		if(!DataName.Contains(Name))
		{
			Reset();
		}
		int.TryParse(DataValue[DataName.IndexOf(Name)], out data);
		return data;
	}
	public float GetFloat(string Name)
	{
		float data;
		if(!DataName.Contains(Name))
		{
			Reset();
		}
		float.TryParse(DataValue[DataName.IndexOf(Name)], out data);
		return data;
	}
	public string GetString(string Name)
	{
		string data;
		if(!DataName.Contains(Name))
		{
			Reset();
		}
		data = DataValue[DataName.IndexOf(Name)];
		return data;
	}
	public void LoadMap(string[] map)
	{
		Debug.Log("Loading Data Map");
		foreach(string i in map)
		{
			if(i.Contains(":") && !i.Contains("#"))
			{
				string[] data = i.Split(':');
				switch(data[1])
				{
					case "Bool":
					{
						bool Value = false;
						bool.TryParse(data[2], out Value);
						RegisterData(data[0], Value);
						break;
					}
					case "Int":
					{
						int Value = 0;
						int.TryParse(data[2], out Value);
						RegisterData(data[0], Value);
						break;
					}
					case "Float":
					{
						float Value = 0;
						float.TryParse(data[2], out Value);
						RegisterData(data[0], Value);
						break;
					}
					case "String":
					{
						string Value = data[2];
						RegisterData(data[0], Value);
						break;
					}
					default:
					{
						break;
					}
				}
			}
		}
	}
	public string[] GetMap()
	{
		ArrayList map = new ArrayList();
		map.Add("#Game Settings Data");
		map.Add("#Format:");
		map.Add("#Identifer:Type:Value");
		map.Add("#Where the types are: Float(decimal), Bool(true/false), String(text), Int(whole number)");
		foreach(string s in DataName)
		{
			int index = DataName.IndexOf(s);
			string Value = s+":"+DataType[index];
			switch(DataType[index])
			{
				case "Bool":
				{
					Value += ":"+DataValue[index];
					break;
				}
				case "Float":
				{
					Value += ":"+DataValue[index];
					break;
				}
				case "Int":
				{
					Value += ":"+DataValue[index];
					break;
				}
				case "String":
				{
					Value += ":"+DataValue[index];
					break;
				}
				default:
				{
					break;
				}
			}
			map.Add(Value);
		}
		return (string[])map.ToArray(typeof(string));
	}
}
