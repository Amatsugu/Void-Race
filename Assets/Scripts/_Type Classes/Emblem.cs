using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Emblem
{
	[Serializable]
	public class Layer
	{
		public byte[] TextureData;
		public float posX;
		public float posY;
		public string name;
		public bool enabled = true;
		public bool blend = true;
	}
	private List<Layer> EmblemData = new List<Layer>();
	private Rect LayerSize;
	public void Set(List<EmblemCreator.Layer> Layers)
	{
		foreach(EmblemCreator.Layer l in Layers)
		{
			byte[] TexBytes = l.texture.EncodeToPNG();
			Layer EmblemLayer = new Layer();
			EmblemLayer.TextureData = TexBytes;
			EmblemLayer.posX = l.posX;
			EmblemLayer.posY = l.posY;
			EmblemLayer.name = l.name;
			EmblemLayer.enabled = l.enabled;
			EmblemLayer.blend = l.blend;
			EmblemData.Add(EmblemLayer);
			LayerSize = new Rect(0,0, l.texture.width, l.texture.height);
		}
	}
	public List<EmblemCreator.Layer> GetLayers()
	{
		List<EmblemCreator.Layer> LayerList = new List<EmblemCreator.Layer>();
		foreach(Layer l in EmblemData)
		{
			Texture2D Tex = new Texture2D(256, 256);
			Tex.LoadImage(l.TextureData);
			Tex.Apply();
			EmblemCreator.Layer EmblemLayer = new EmblemCreator.Layer();
			EmblemLayer.texture = Tex;
			EmblemLayer.posX = l.posX;
			EmblemLayer.posY = l.posY;
			EmblemLayer.name = l.name;
			EmblemLayer.enabled = l.enabled;
			EmblemLayer.blend = l.blend;
			LayerList.Add(EmblemLayer);
		}
		return LayerList;
	}
	public void Set(string binaryData)
	{
		BinaryFormatter bFormat = new BinaryFormatter();
		MemoryStream emblemBinary = new MemoryStream(Convert.FromBase64String(binaryData));
		EmblemData = (List<Layer>)bFormat.Deserialize(emblemBinary);
		LayerSize = new Rect(0,0, GetLayers()[0].texture.width, GetLayers()[0].texture.height);
	}
	public string GetBinary()
	{
		MemoryStream stream = new MemoryStream();
		BinaryFormatter bFormat = new BinaryFormatter();
		bFormat.Serialize(stream, EmblemData);
		string Binary = Convert.ToBase64String(stream.GetBuffer());
		return Binary;
	}
	public int GetByteCount()
	{
		int ByteCount = 0;
		foreach( Layer l in EmblemData)
		{
			ByteCount += l.TextureData.Length;
		}
		return ByteCount;
	}
	public Texture2D GetFlatTex()
	{
		Texture2D Flat =  new Texture2D((int)LayerSize.width, (int)LayerSize.height);
		Flat.wrapMode = TextureWrapMode.Clamp;
		for(int row = 0; row < LayerSize.height; row++)
		{
			for(int col = 0; col < LayerSize.width; col++)
			{
				Flat.SetPixel(col, row, new Color(0,0,0,0));
			}
		}
		Flat.SetPixels((int)GetLayers()[0].posX,
			(int)(LayerSize.height - GetLayers()[0].posX),
			(int)LayerSize.width-(int)GetLayers()[0].posX,
			(int)LayerSize.height-(int)(LayerSize.height - GetLayers()[0].posX),
			GetLayers()[0].texture.GetPixels(0,0,(int)LayerSize.width,
			(int)LayerSize.height));
//		foreach(EmblemCreator.Layer l in GetLayers())
//		{
//			Color[] curFlatColors = Flat.GetPixels();
//			Color[] finalFlatColors = Flat.GetPixels();
//			Color[] texColors = l.texture.GetPixels();
//			Vector2 offset = new Vector2(l.posX,LayerSize.height - l.posY);
//			int index = 0;
//			foreach(Color c in texColors)
//			{
//				if(c.a < 1)
//				{
//					finalFlatColors[index].a = c.a + curFlatColors[index].a;
//					finalFlatColors[index].r = (c.r + curFlatColors[index].r)/2;
//					finalFlatColors[index].g = (c.g + curFlatColors[index].g)/2;
//					finalFlatColors[index].b = (c.b + curFlatColors[index].b)/2;
//				}else
//				{
//					finalFlatColors[index] = c;
//				}
//				index++;
//			}
//			finalFlatColors = texColors;
//			Flat.SetPixels((int)offset.x, (int)offset.y, (int)LayerSize.width-(int)offset.x, (int)LayerSize.height-(int)offset.y, finalFlatColors);
//			Flat.Apply();
//		}
		Flat.Apply();
		return Flat;
	}
}
