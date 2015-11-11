using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
[Serializable]
public class EmblemCreator : MonoBehaviour {
	[Serializable]
	public class Layer
	{
		public Texture2D texture;
		public float posX;
		public float posY;
		public string name;
		public bool enabled = true;
		public bool blend = true;
	}
	public class LayerMove
	{
		public bool isMove = false;
		public Layer layer;
		public int to;
	}
	public class LayerDup
	{
		public bool isDup = false;
		public Layer layer;
		public int to;
	}
	//public vars
	public enum Tools
	{
		Eraser,
		Brush,
		Move,
		Shape,
		Select
	};
	public float MinBrushSize = 1;
	public float MaxBrushSize = 256;
	public float ToolTipCharLenght = 6;
	public Rect EmblemSize;
	public GUIStyle ActiveLayerStyle;
	public Color SelectionColor = new Color(0,0,1, 0.4f);
	public GUISkin skin;
	public Emblem FinalEmblem;
	public float AutoSaveInterval = 60f;
	public int MaxLayers = 16;
	//private vars
	private int ActiveLayer = 0;
	private List<Layer> Layers = new List<Layer>();
	private int curBrushSize = 10;
	private int curEraserSize = 10;
	private Color curBrushColor = Color.black;
	private Rect windowRect;
	private Vector2 LayersPanelScrollPos;
	private Tools curTool = Tools.Select;
	private LayerMove Move;
	private Layer LayerToDelete;
//	private LayerDup DupLayer;
	private bool rectSet;
	private float TargetPos;
	private Rect CaptureRect;
	private Vector2 MousePos;
	private string ToolTip = "";
	private Rect ToolTipRect;
	private Texture2D UILayer;
	private Color Eraser = new Color(0,0,0,0);
	private Rect SelectionRect;
	private Vector2 MovePoint;
	private	Vector2 offset;
	private bool captureIMG = false;
	private float nextAutoSave;
	private bool isSaving = false;
	private bool showLoadWin = false;
	private bool LoadWinClosing = false;
	private bool loadWinRectSet = false;
	private Rect LoadwinRect;
	private bool showExportWin = false;
	private bool ExportWinClosing = false;
	private bool ExportWinRectSet = false;
	private Rect ExportWinRect;
	private bool DataLoaded = false;
	private string LoadedSave;
	private string LoadedAutoSave;
	private Emblem AutoSave;
	private	Emblem Save;
	private List<Layer> AutoSaveLayer = new List<Layer>();
	private List<Layer> SaveLayer = new List<Layer>();
	private bool isLoading = false;
	private Color texCol = new Color(0,0,0,0f);
	private Texture flatComp;
	//private Vector2 LoadWinScrollPos;
	// Use this for initialization
	void Start () 
	{
		Move = new LayerMove();
		//DupLayer = new LayerDup();
		AutoSave = new Emblem();
		Save = new Emblem();
		AddNewLayer();
	}
	void AddNewLayer() //Adds a new Layer
	{
		Layer newLayer = new Layer();
		newLayer.name = "Layer_"+Layers.Count;
		newLayer.posX = 0;
		newLayer.posY = 0;
		newLayer.texture = new Texture2D((int)EmblemSize.width, (int)EmblemSize.height);
		newLayer.texture = SetPixels(newLayer.texture, new Rect(0, 0, EmblemSize.width, EmblemSize.height), Eraser, true);
		newLayer.texture.Apply();
		Layers.Add(newLayer);
	}
	void Update()
	{
		if(captureIMG)
		{
			FinalEmblem = new Emblem();
			FinalEmblem.Set(Layers);
		}
	}
	Texture2D SetPixels(Texture2D tex, Rect pixels, Color color, bool ignoreSelection)
	{
		int row = (int)pixels.x;
		int col = (int)pixels.y;
		Texture2D newTex = tex;
		if(ignoreSelection)
		{
			for(col = (int)pixels.x; col < pixels.x + pixels.width; col++)
			{
				for(row = (int)pixels.y; row < pixels.y + pixels.height; row++)
				{
					newTex.SetPixel(col, row, color);
				}
			}
		}else
		{
			newTex = SetPixels(tex, pixels, color);
		}
		newTex.Apply();
		return newTex;
	}
	Texture2D SetPixels(Texture2D tex, Rect pixels, Color color)
	{
		int row = (int)pixels.x;
		int col = (int)pixels.y;
		Texture2D newTex = tex;
		for(col = (int)pixels.x; col < pixels.x + pixels.width; col++)
		{
			for(row = (int)pixels.y; row < pixels.y + pixels.height; row++)
			{
				if(SelectionRect.width > 0 && SelectionRect.height > 0)
				{
					if(col <= SelectionRect.x + SelectionRect.width -Layers[ActiveLayer].posX -1 && col > SelectionRect.x -Layers[ActiveLayer].posX -1 && row > (tex.height - (SelectionRect.y + SelectionRect.height-Layers[ActiveLayer].posY)) -1 && row <= (tex.height - SelectionRect.y+Layers[ActiveLayer].posY -1) )
					{
						newTex.SetPixel(col, row, color);
					}
				}else
				{
					newTex.SetPixel(col, row, color);
				}
			}
		}
		newTex.Apply();
		return newTex;
	}
	void OnGUI() 
	{
		MousePos.x = Input.mousePosition.x;
		MousePos.y = Screen.height - Input.mousePosition.y;
		if(ActiveLayer > Layers.Count -1)
			ActiveLayer = -1;
		
		Emblem tmp;
		DisplayEmblemEditor(new Vector2((Screen.width/2) - (700/2), (Screen.height/2) - (600/2)), skin, out tmp, 16);
	}
	public bool DisplayEmblemEditor(Vector2 pos, GUISkin GUIskin, out Emblem EmblemData, int LayerMax)
	{
		return EmblemEditor(pos, GUIskin, out EmblemData, LayerMax);
	}
	public bool DisplayEmblemEditor(Vector2 pos, GUISkin GUIskin, out Emblem EmblemData, int LayerMax, Emblem emblem)
	{
		Layers = emblem.GetLayers();
		return EmblemEditor(pos, GUIskin, out EmblemData, LayerMax);
	}
	bool EmblemEditor(Vector2 pos, GUISkin GUIskin, out Emblem EmblemData, int LayerMax)
	{
		GUI.skin = GUIskin;
		if(!rectSet)
		{
			windowRect = new Rect(pos.x, pos.y, 700, 600);
			TargetPos = windowRect.x;
			windowRect.x = -700;
			MaxLayers = LayerMax;
			nextAutoSave = Time.time + AutoSaveInterval;
			rectSet = true;
		}
		if(rectSet)
			windowRect.x = Mathf.Lerp(windowRect.x, TargetPos, Time.deltaTime*2);
		CaptureRect = new Rect(windowRect.x + 20, windowRect.y + 55, EmblemSize.width, EmblemSize.height);
		if(showLoadWin)
		{
			GUI.enabled = false;
		}else
		{
			GUI.enabled = true;
		}
		GUI.Window(0, windowRect, EmblemEditorWindow, "Emblem Editor");
		if(!showLoadWin)
			GUI.FocusWindow(0);
		else
			GUI.UnfocusWindow();
		if(Time.time >= nextAutoSave)
		{
			SaveData(true);
			nextAutoSave = Time.time + AutoSaveInterval;
		}
		if(captureIMG && FinalEmblem != null)
		{
			EmblemData = FinalEmblem;
			return true;
		}else
		{
			EmblemData = null;
			return false;
		}
	}
	void EmblemEditorWindow(int windowID)
	{
		GUI.BeginGroup(new Rect(0,0, windowRect.width, windowRect.height));
		RenderContent();
		RenderLayersPanel();
		RenderToolsPanel();
		RenderPropertiesPanel();
		GUI.EndGroup();
		if(!showLoadWin)
			RenderUITools();
		RenderControls();
		ToolTip = GUI.tooltip;
		if(ToolTip.Length >0)
		{
			ToolTip = ToolTip + ".";
		}
		ToolTipRect = new Rect((MousePos.x + 15) - windowRect.x, (MousePos.y + 15) - windowRect.y, ToolTipCharLenght * ToolTip.Length, 25);
		if(ToolTipRect.x + ToolTipRect.width > windowRect.width)
		{
			ToolTipRect.x -= ((ToolTipRect.x + ToolTipRect.width) - windowRect.width)+5;
		}
		if(ToolTipRect.y + ToolTipRect.height > windowRect.height)
		{
			ToolTipRect.y -= ((ToolTipRect.y + ToolTipRect.height) - windowRect.height) + 25;
		}
		if(ToolTip.Length > 0)
			GUI.Box(ToolTipRect, ToolTip);
	}
	void RenderControls()
	{
		Rect curRect = new Rect(5, windowRect.height - 25, windowRect.width -10, 20);
		GUI.BeginGroup(curRect);
		if(GUI.Button(new Rect(curRect.width/2 - 310, 0, 150, 20), new GUIContent("Save & Exit", "Saves the current data and exits")))
		{
			captureIMG = true;
		}
		//if(!showLoadWin)
		//	GUI.enabled = false;
		if(GUI.Button(new Rect(curRect.width/2 - 155, 0, 150, 20), new GUIContent("Export", "Saves the current Image to your computer")))
		{
			showExportWin = true;
		}
		//if(!showLoadWin)
		//	GUI.enabled = true;
		GUIContent saveBtn = new GUIContent("Save", "Saves the editable data, Ctrl + S. Saved " + (int)(nextAutoSave-Time.time) +"s ago");
		if(!showLoadWin)
		{
			if(isSaving)
			{
				GUI.enabled = false;
				saveBtn.text = "Saving...";
			}else
			{
				GUI.enabled = true;
				saveBtn.text = "Save";
				
			}
		}
		if(GUI.Button(new Rect(curRect.width/2, 0, 150, 20), saveBtn))
		{
			SaveData(false);
		}
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.S))
		{
			SaveData(false);
		}
		
		if(GUI.Button(new Rect(curRect.width/2 + 155, 0, 150, 20), new GUIContent("Load File", "Load an image to a layer, or load a previously saved editable")))
		{
			showLoadWin = true;
			DataLoaded = false;
		}
		if(!showLoadWin)
			GUI.enabled = true;
		GUI.EndGroup();
		if(showLoadWin)
		{
			ShowLoadWindow();
		}
		if(showExportWin)
		{
			ShowExportWin();
		}
	}
	void ShowExportWin()
	{
		GUI.enabled = true;
		if(!ExportWinClosing)
		{
			if(!ExportWinRectSet)
			{
				ExportWinRect = new Rect( Screen.width/2 - ((EmblemSize.width * 2)+ 50)/2, -500, ((EmblemSize.width * 2)+ 50), 500);
				ExportWinRectSet = true;
			}
			ExportWinRect.y = Mathf.Lerp(ExportWinRect.y, Screen.height/2 -(250), Time.deltaTime*2);
			texCol.a = Mathf.Lerp(texCol.a, .5f, Time.deltaTime);
		}else
		{
			Debug.Log("Export Win: Closing");
			ExportWinRect.y = Mathf.Lerp(ExportWinRect.y, -501, Time.deltaTime*2);
			if(ExportWinRect.y <= -500)
			{
				ExportWinRectSet = false;
				showExportWin = false;
				ExportWinClosing = false;
			}
			texCol.a = Mathf.Lerp(texCol.a, 0, Time.deltaTime);
			
		}
		
		Texture2D tex = new Texture2D(1,1);
		tex.SetPixel(0,0, texCol);
		tex.Apply();
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), tex, ScaleMode.StretchToFill);
		
		GUI.Window(2,ExportWinRect , ExportWindow, "Export Image");
		GUI.BringWindowToFront(2);
		//GUI.BringWindowToBack(0);
		GUI.FocusWindow(2);
	}
	void ExportWindow(int windowID)
	{
		if(GUI.Button(new Rect(ExportWinRect.width - 25, 5, 20, 20), "X"))
		{
			ExportWinClosing = true;
			flatComp = null;
		}
		if(flatComp == null)
		{
			GUI.Label(new Rect(10, 25, 500, 20), "Flattening layers, please wait...");
		}
		if(ExportWinRect.y >= Screen.height/2 -(250)-1)
		{
			if(flatComp == null)
			{
				Emblem tmpE = new Emblem();
				tmpE.Set(Layers);
				flatComp = tmpE.GetFlatTex();
			}else
			{
				GUI.Label(new Rect(10, 25, 100, 20), "Your current Image");
				GUI.BeginGroup(new Rect(10, 45, EmblemSize.width, EmblemSize.height));
				foreach(Layer l in Layers)
				{
					GUI.DrawTexture(new Rect( l.posX, l.posY, EmblemSize.width, EmblemSize.height), l.texture, ScaleMode.StretchToFill);
				}
				GUI.EndGroup();
				GUI.Label(new Rect(ExportWinRect.width - 100 - 10, 25, 100, 20), "Your Flattened Image");
				GUI.DrawTexture(new Rect(ExportWinRect.width - EmblemSize.width - 10, 45, EmblemSize.width, EmblemSize.height), flatComp, ScaleMode.StretchToFill);
			}
		}
	}
	void ShowLoadWindow()
	{
		GUI.enabled = true;
		if(!LoadWinClosing)
		{
			if(!loadWinRectSet)
			{
				LoadwinRect = new Rect( Screen.width/2 - 200, -500, 400, 500);
				loadWinRectSet = true;
			}
			LoadwinRect.y = Mathf.Lerp(LoadwinRect.y, Screen.height/2 - 250, Time.deltaTime*2);
			texCol.a = Mathf.Lerp(texCol.a, .5f, Time.deltaTime);
			
		}else
		{
			LoadwinRect.y = Mathf.Lerp(LoadwinRect.y, -501, Time.deltaTime*2);
			if(LoadwinRect.y <= -500)
			{
				loadWinRectSet = false;
				showLoadWin = false;
				LoadWinClosing = false;
				DataLoaded = false;
			}
			texCol.a = Mathf.Lerp(texCol.a, 0, Time.deltaTime);
		}
		Texture2D tex = new Texture2D(1,1);
		tex.SetPixel(0,0, texCol);
		tex.Apply();
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), tex, ScaleMode.StretchToFill);
		
		GUI.Window(1,LoadwinRect , LoadWindow, "Load Data");
		GUI.BringWindowToFront(1);
		GUI.FocusWindow(1);
	}
	void LoadWindow(int windowID)
	{
		if(GUI.Button(new Rect(375, 5, 20, 20), "X"))
		{
			LoadWinClosing = true;
			DataLoaded = false;
		}
		GUI.BeginGroup(new Rect(5, 35, 390, 460));
		
		if(!DataLoaded)
		{
			isLoading = true;
			LoadedAutoSave = PlayerPrefs.GetString("AutoSavedData", "NODATA");
			LoadedSave = PlayerPrefs.GetString("SavedData", "NODATA");
			if(LoadedAutoSave != "NODATA" && LoadedAutoSave != null)
			{
				AutoSave.Set(LoadedAutoSave);
				AutoSaveLayer = AutoSave.GetLayers();
			}
			if(LoadedSave != "NODATA" && LoadedSave != null)
			{
				Save.Set(LoadedSave);
				SaveLayer = Save.GetLayers();
			}
			isLoading = false;
			DataLoaded = true;
		}
		
		if(!isLoading && DataLoaded)
		{
			GUI.Label(new Rect(0,0, 100, 20), "Saved Data:");
			
			GUI.BeginGroup(new Rect(0, 25, 390, 100));
			GUI.Box(new Rect(0,0, 390, 100), "AutoSave");
			if(LoadedAutoSave != "NODATA")
			{
				foreach(Layer l in AutoSaveLayer)
				{
					GUI.DrawTexture(new Rect(5, 5, 90, 90), l.texture, ScaleMode.ScaleToFit);
				}
				if(GUI.Button(new Rect(365, 40, 20, 20), new GUIContent("X","Deletes this save, forever.")))
				{
					PlayerPrefs.SetString("AutoSavedData", "NODATA");
					DataLoaded = false;
				}
				GUI.Label(new Rect(105, 35, 150, 20), AutoSave.GetLayers().Count + " Layers");
				GUI.Label(new Rect(105, 50, 150, 20), AutoSave.GetByteCount() + " Bytes");
				if(GUI.Button(new Rect(210, 40, 100, 20), "Load"))
				{
					Layers = AutoSave.GetLayers();
					LoadWinClosing = true;
				}
			}else
				GUI.Label(new Rect((390/2) - 55, 50 - 10, 150, 20), "No Auto Saved Data");
			GUI.EndGroup();
			
			GUI.BeginGroup(new Rect(0, 130, 390, 100));
			GUI.Box(new Rect(0,0, 390, 100), "Save");
			if(LoadedSave != "NODATA")
			{
				
				foreach(Layer l in SaveLayer)
				{
					GUI.DrawTexture(new Rect(5, 5, 90, 90), l.texture, ScaleMode.ScaleToFit);
				}
				if(GUI.Button(new Rect(365, 40, 20, 20), new GUIContent("X","Deletes this save, forever.")))
				{
					PlayerPrefs.SetString("SavedData", "NODATA");
					DataLoaded = false;
				}
				GUI.Label(new Rect(105, 35, 150, 20), Save.GetLayers().Count + " Layers");
				GUI.Label(new Rect(105, 50, 150, 20), Save.GetByteCount() + " Bytes");
				if(GUI.Button(new Rect(210, 40, 100, 20), "Load"))
				{
					Layers = Save.GetLayers();
					LoadWinClosing = true;
				}
			}else
				GUI.Label(new Rect((390/2) - 40, 50 - 10, 100, 20), "No Saved Data");
			GUI.EndGroup();
		}else
		{
			GUI.BeginGroup(new Rect(0, 80, 390, 50));
			GUI.Box(new Rect(0,0, 390, 50), "Loading Data...");
			
			GUI.EndGroup();
		}
		GUI.EndGroup();
	}
	void SaveData(bool isAuto)
	{
		isSaving = true;
		Emblem tmp = new Emblem();
		tmp.Set(Layers);
		if(!isAuto)
			PlayerPrefs.SetString("SavedData", tmp.GetBinary());
		else
			PlayerPrefs.SetString("AutoSavedData", tmp.GetBinary());
		nextAutoSave = Time.time + AutoSaveInterval;
		isSaving = false;
	}
	void RenderUITools()
	{
		if(showLoadWin)
			GUI.enabled = false;
		else
			GUI.enabled = true;
		if(CaptureRect.Contains(MousePos) && !showExportWin && !showLoadWin)
		{
			//Brush Tool
			if(curTool == Tools.Brush && ActiveLayer != -1)
			{
				GUI.BeginGroup(new Rect(20f, 55f, EmblemSize.width, EmblemSize.height));
				Texture2D cursor = new Texture2D(1,1);
				cursor.SetPixel(0,0, curBrushColor);
				cursor.SetPixel(0,0, curBrushColor);
				cursor.Apply();
				Rect BrushRect = new Rect(((MousePos.x-CaptureRect.x)-(curBrushSize/2)), ((MousePos.y-CaptureRect.y)-(curBrushSize/2)), curBrushSize, curBrushSize);
				if(BrushRect.x + BrushRect.width > CaptureRect.width)
				{
					BrushRect.x -= (BrushRect.x + BrushRect.width) - CaptureRect.width;
				}
				if(BrushRect.x < 0)
				{
					BrushRect.x = 0;
				}
				if(BrushRect.y + BrushRect.height > CaptureRect.height)
				{
					BrushRect.y -= (BrushRect.y + BrushRect.height) - CaptureRect.height;
				}
				if(BrushRect.y < 0)
				{
					BrushRect.y = 0;
				}
				GUI.DrawTexture(BrushRect , cursor, ScaleMode.ScaleToFit);
				GUI.EndGroup();
				if(Input.GetKey(KeyCode.Mouse0))
				{
					Layers[ActiveLayer].texture = SetPixels(Layers[ActiveLayer].texture, 
						new Rect(BrushRect.x -Layers[ActiveLayer].posX, ((Layers[ActiveLayer].texture.height - (int)BrushRect.y)- curBrushSize)+Layers[ActiveLayer].posY,
						curBrushSize, curBrushSize), curBrushColor);
				}
			}
			
			//Eraser Tool
			if(curTool == Tools.Eraser && ActiveLayer != -1)
			{
				GUI.BeginGroup(new Rect(20f, 55f, EmblemSize.width, EmblemSize.height));
				Texture2D cursor = new Texture2D(1,1);
				Color cursorColor = Layers[ActiveLayer].texture.GetPixel((int)(MousePos.x-CaptureRect.x), (int)(Layers[ActiveLayer].texture.height -(MousePos.y-CaptureRect.y)));
				cursorColor = InvertColor(cursorColor, false);
				cursorColor.a = 1;
				cursor.SetPixel(0,0, cursorColor);
				cursor.Apply();
				Rect BrushRect = new Rect(((MousePos.x-CaptureRect.x)-(curEraserSize/2)), ((MousePos.y-CaptureRect.y)-(curEraserSize/2)), curEraserSize, curEraserSize);
				if(BrushRect.x + BrushRect.width > CaptureRect.width)
				{
					BrushRect.x -= (BrushRect.x + BrushRect.width) - CaptureRect.width;
				}
				if(BrushRect.x < 0)
				{
					BrushRect.x = 0;
				}
				if(BrushRect.y + BrushRect.height > CaptureRect.height)
				{
					BrushRect.y -= (BrushRect.y + BrushRect.height) - CaptureRect.height;
				}
				if(BrushRect.y < 0)
				{
					BrushRect.y = 0;
				}
					
				GUI.DrawTexture(BrushRect , cursor, ScaleMode.ScaleToFit);
				GUI.EndGroup();
				if(Input.GetKey(KeyCode.Mouse0))
				{
					Layers[ActiveLayer].texture = SetPixels(Layers[ActiveLayer].texture, new Rect(BrushRect.x-Layers[ActiveLayer].posX, ((Layers[ActiveLayer].texture.height - (int)BrushRect.y)- curEraserSize)+Layers[ActiveLayer].posY, curEraserSize, curEraserSize), Eraser);
				}
			}
			
			//Move Tool
			if(curTool == Tools.Move && ActiveLayer != -1)
			{
				if(Input.GetKeyDown(KeyCode.Mouse0))
				{
					MovePoint = new Vector2((MousePos.x - CaptureRect.x), (MousePos.y - CaptureRect.y));
					offset = new Vector2(Layers[ActiveLayer].posX + MovePoint.x, Layers[ActiveLayer].posY + MovePoint.y);
					Debug.Log("Move Point: " + MovePoint + " | " + "Offset: " + offset);
				}
				if(Input.GetKey(KeyCode.Mouse0))
				{
					Layers[ActiveLayer].posX = MovePoint.x + offset.x;
					Layers[ActiveLayer].posY = MovePoint.y + offset.y;
					//Debug.Log("Layer_"+ActiveLayer+"'s pos: "+  Layers[ActiveLayer].posX + ",);
				}
				if(Input.GetKeyUp(KeyCode.Mouse0))
				{
					Layers[ActiveLayer].posX = MovePoint.x + offset.x;
					Layers[ActiveLayer].posY = MovePoint.y + offset.y;
					MovePoint = Vector2.zero;
					offset = Vector2.zero;
				}
			}
			
			//Select Tool
			if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.A))
				SelectionRect.Set(0,0, EmblemSize.width, EmblemSize.height);
			if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.D))
				SelectionRect.Set(0,0,0,0);
			if(curTool == Tools.Select)
			{
				Vector2 curMousePos = new Vector2((MousePos.x - CaptureRect.x), (MousePos.y - CaptureRect.y));
				if(Input.GetKeyDown(KeyCode.Mouse0))
				{
					SelectionRect.x = MousePos.x - CaptureRect.x;
					SelectionRect.y = MousePos.y - CaptureRect.y;
				}
				if(Input.GetKey(KeyCode.Mouse0))
				{
					SelectionRect.width = (MousePos.x - CaptureRect.x) - SelectionRect.x;
					SelectionRect.height = (MousePos.y - CaptureRect.y) - SelectionRect.y;
				}
				if(Input.GetKeyUp(KeyCode.Mouse0))
				{
					if(SelectionRect.x < curMousePos.x)
					{
						SelectionRect.width = (MousePos.x - CaptureRect.x) - SelectionRect.x;
					}else if(SelectionRect.x > curMousePos.x)
					{
						SelectionRect.width = SelectionRect.x - curMousePos.x;
						SelectionRect.x = curMousePos.x;
					}
					if(SelectionRect.y < curMousePos.y)
					{
						SelectionRect.height = (MousePos.y - CaptureRect.y) - SelectionRect.y;
					}else if(SelectionRect.y > curMousePos.y)
					{
						SelectionRect.height = SelectionRect.y - curMousePos.y;
						SelectionRect.y = curMousePos.y;
					}
				}
				if(Input.GetKeyDown(KeyCode.Mouse1))
				{
					SelectionRect = new Rect(0,0,0,0);
				}
				
			}
		}
		if(curTool == Tools.Select)
		{
			if(SelectionRect.width > 0 && SelectionRect.height > 0 && ActiveLayer != -1)
			{
				if(Input.GetKeyDown(KeyCode.Delete))
					Layers[ActiveLayer].texture = SetPixels(Layers[ActiveLayer].texture, new Rect(0,0, EmblemSize.width, EmblemSize.height), Eraser);
				if(Input.GetKeyDown(KeyCode.Backspace))
					Layers[ActiveLayer].texture = SetPixels(Layers[ActiveLayer].texture, new Rect(0,0, EmblemSize.width, EmblemSize.height), curBrushColor);
			}
		}
		GUI.BeginGroup(new Rect(CaptureRect.x - windowRect.x, CaptureRect.y - windowRect.y, CaptureRect.width, CaptureRect.height));
		Texture2D selectionGrid = new Texture2D(1,1);
		selectionGrid.SetPixel(0,0, SelectionColor);
		selectionGrid.Apply();
		GUI.DrawTexture(SelectionRect, selectionGrid, ScaleMode.StretchToFill);
		GUI.EndGroup();
	}
	void RenderPropertiesPanel()
	{
		if(showLoadWin)
			GUI.enabled = false;
		else
			GUI.enabled = true;
		GUI.BeginGroup(new Rect(5, 440, 690, 130));
		GUI.Box(new Rect(0,0, 690, 130), curTool.ToString() +" Properties");
		if(curTool == Tools.Brush)
		{
			GUI.Label(new Rect(5, 15, 150, 20), "Brush Size: " + curBrushSize + "px");
			curBrushSize = (int)GUI.HorizontalSlider(new Rect(5, 35, 200, 20), curBrushSize, MinBrushSize, MaxBrushSize);
			GUI.Label(new Rect(215, 15, 100, 20), "Brush Color: ");
			Texture2D curColor = new Texture2D(1, 1);
			curColor.SetPixel(0,0, curBrushColor);
			curColor.SetPixel(1,1, curBrushColor);
			curColor.Apply();
			GUI.DrawTexture(new Rect(215, 40, 75, 75), curColor, ScaleMode.ScaleToFit);
			GUI.Label(new Rect(295, 30, 270, 20), new GUIContent("R: ", "The red value, adds red to the color"));
			curBrushColor.r  = GUI.HorizontalSlider(new Rect(315, 35, 200, 20), curBrushColor.r, 0, 1);
			float.TryParse(GUI.TextField(new Rect(525, 30, 100, 20), curBrushColor.r.ToString()), out curBrushColor.r); 
			GUI.Label(new Rect(295, 55, 270, 20), new GUIContent("G: ", "The green value, adds green to the color"));
			curBrushColor.g  = GUI.HorizontalSlider(new Rect(315, 60, 200, 20), curBrushColor.g, 0, 1);
			float.TryParse(GUI.TextField(new Rect(525, 55, 100, 20), curBrushColor.g.ToString()), out curBrushColor.g); 
			GUI.Label(new Rect(295, 80, 270, 20), new GUIContent("B: ", "The blue value, adds blue to the color"));
			curBrushColor.b  = GUI.HorizontalSlider(new Rect(315, 85, 200, 20), curBrushColor.b, 0, 1);
			float.TryParse(GUI.TextField(new Rect(525, 80, 100, 20), curBrushColor.b.ToString()), out curBrushColor.b); 
			GUI.Label(new Rect(295, 105, 270, 20), new GUIContent("A: ", "The alpha value, controls transparency"));
			curBrushColor.a  = GUI.HorizontalSlider(new Rect(315, 110, 200, 20), curBrushColor.a, 0, 1);
			float.TryParse(GUI.TextField(new Rect(525, 105, 100, 20), curBrushColor.a.ToString()), out curBrushColor.a); 
		}
		if(curTool == Tools.Eraser)
		{
			GUI.Label(new Rect(5, 15, 150, 20), "Eraser Size: " + curEraserSize + "px");
			curEraserSize = (int)GUI.HorizontalSlider(new Rect(5, 35, 200, 20), curEraserSize, MinBrushSize, MaxBrushSize);
		}
		GUI.EndGroup();
	}
	void RenderToolsPanel()
	{
		if(showLoadWin)
			GUI.enabled = false;
		else
			GUI.enabled = true;
		GUI.BeginGroup(new Rect(5, 326, EmblemSize.width + 30, 109));
		GUI.Box(new Rect(0,0, EmblemSize.width + 30, 109), "Tools");
		GUIContent btn = new GUIContent();
		btn.text = "Brush";
		btn.tooltip = "The Brush Tool";
		if(GUI.Button(new Rect(5, 35, 50, 20), btn))
		{
			curTool = Tools.Brush;
		}
		btn.text = "Eraser";
		btn.tooltip = "The Eraser Tool";
		if(GUI.Button(new Rect(60, 35, 55, 20), btn))
		{
			curTool = Tools.Eraser;
		}
//		btn.text = "Move";
//		btn.tooltip = "The Move Tool";
//		if(GUI.Button(new Rect(120, 35, 50, 20), btn))
//		{
//			curTool = Tools.Move;
//		}
		btn.text = "Select";
		btn.tooltip = "The selection Tool";
		if(GUI.Button(new Rect(120, 35, 55, 20), btn))
		{
			curTool = Tools.Select;
		}
		GUI.EndGroup();
	}
	void RenderLayersPanel()
	{
		if(showLoadWin)
			GUI.enabled = false;
		else
			GUI.enabled = true;
		GUI.BeginGroup(new Rect(windowRect.width - 405, 35, 400, 400));
		GUI.Box(new Rect(0,0, 400, 400), "Layers");
		LayersPanelScrollPos = GUI.BeginScrollView(new Rect(0, 25, 395, 340), LayersPanelScrollPos, new Rect(0,0, 375, 100 * Layers.Count + 5 * Layers.Count));
		foreach(Layer l in Layers)
		{
			if(!showLoadWin)
				GUI.enabled = l.enabled;
			GUI.BeginGroup(new Rect(0, 100 * Layers.IndexOf(l) + 5 * Layers.IndexOf(l), 400, 100));
			if(ActiveLayer == Layers.IndexOf(l))
			{
				GUI.Box(new Rect(10,0, 365, 100), "", ActiveLayerStyle);
			}else
			{
				GUI.Box(new Rect(10,0, 365, 100), "");
			}
			GUI.BeginGroup(new Rect(15, 5, 90, 90));
			GUI.DrawTexture(new Rect(l.posX *(90/EmblemSize.width), l.posY *(90/EmblemSize.height), 90, 90), l.texture, ScaleMode.ScaleToFit, l.blend);
			GUI.EndGroup();
			l.name = GUI.TextField(new Rect(115, 10, 150, 20), l.name);
//			if(GUI.Button(new Rect(115, 40, 150, 20), new GUIContent("Duplicate Layer","Duplicate the current layer")))
//			{
//				DupLayer = new LayerDup();
//				DupLayer.isDup = true;
//				DupLayer.layer = l;
//				DupLayer.to = Layers.IndexOf(l);
//			}
			GUI.Label(new Rect(115, 35, 20, 20), "X: ");
			float.TryParse(GUI.TextField(new Rect(135, 35, 50, 20), l.posX.ToString()), out l.posX);
			GUI.Label(new Rect(195, 35, 20, 20), "Y: ");
			float.TryParse(GUI.TextField(new Rect(215, 35, 50, 20), l.posY.ToString()), out l.posY);
			if(Layers.IndexOf(l) == ActiveLayer)
			{
				if(GUI.Button(new Rect( 115, 65, 150, 20), new GUIContent("Deselect Layer", "Deselects this layer as the active layer to edit")))
				{
					ActiveLayer = -1;
				}
			}else
			{
				if(GUI.Button(new Rect( 115, 65, 150, 20), new GUIContent("Select Layer","Selects this layer as the active layer to edit")))
				{
					ActiveLayer = Layers.IndexOf(l);
				}
			}
			if(GUI.Button(new Rect(270, 50-25,55, 20), new GUIContent("Delete", "Deletes this layer")))
			{
				LayerToDelete = l;
			}
			l.blend = GUI.Toggle(new Rect(270, 55,55, 20), l.blend, new GUIContent("Alpha?", "Enable/Disable Alpha transparency for this layer"));
			if(!showLoadWin)
			{
				if(Layers.IndexOf(l) == 0)
				{
					GUI.enabled = false;
				}else	
				{
					GUI.enabled = l.enabled;
				}
			}
			if(GUI.Button(new Rect(365 - 35, 50- 30, 25, 25), new GUIContent("^", "Moves this layer up by 1")))
			{
				int index = Layers.IndexOf(l);
				if(ActiveLayer == index)
				{
					ActiveLayer = index - 1;
				}
				Move = new LayerMove();
				Move.isMove = true;
				Move.layer = l;
				Move.to = index - 1;
			}
			if(!showLoadWin)
			{
				if(Layers.IndexOf(l) == Layers.Count-1)
				{
					GUI.enabled = false;
				}else	
				{
					GUI.enabled = l.enabled;
				}
			}
			if(GUI.Button(new Rect(365 - 35, 55, 25, 25), new GUIContent("v", "Moves this layer down by 1")))
			{
				int index = Layers.IndexOf(l);
				if(ActiveLayer == index)
				{
					ActiveLayer = index + 1;
				}
				Move = new LayerMove();
				Move.isMove = true;
				Move.layer = l;
				Move.to = index + 1;
			}
			if(!showLoadWin)
				GUI.enabled = true;
			l.enabled = GUI.Toggle(new Rect(365 - 10, 50-10, 20, 20), l.enabled, new GUIContent("", "Enable/Disable this layer"));
			if(!showLoadWin)
				GUI.enabled = l.enabled;
			GUI.EndGroup();
		}
		if(Move.isMove)
		{
			Layers.Remove(Move.layer);
			Layers.Insert(Move.to, Move.layer);
			Move = new LayerMove();
		}
//		if(DupLayer.isDup)
//		{
//			Layers.Insert(DupLayer.to + 1, DupLayer.layer);
//			DupLayer = new LayerDup();
//		}
		if(LayerToDelete != null)
		{
			Layers.Remove(LayerToDelete);
			LayerToDelete = null;
			
		}
		GUI.EndScrollView();
		if(!showLoadWin)
		{
			if(Layers.Count >= MaxLayers)
			{
				GUI.enabled = false;
			}else
			{
				GUI.enabled = true;
			}
		}
		if(GUI.Button(new Rect(10, 400 - 25, 20, 20), new GUIContent("+", "Adds a new layer")))
		{
			AddNewLayer();
			LayersPanelScrollPos.y = 1000000f;
			ActiveLayer = Layers.Count -1;
		}
		GUI.enabled = true;
		if(!showLoadWin)
		{
			if(ActiveLayer == -1)
			{
				GUI.enabled = false;
			}else
			{
				GUI.enabled = true;
			}
		}
		if(GUI.Button(new Rect(40, 400 - 25, 20, 20),new GUIContent("-", "Removes the curretly selected layer")))
		{
			Layers.RemoveAt(ActiveLayer);
			ActiveLayer = -1;
		}
		GUI.Label(new Rect(70, 400-25, 100, 20), new GUIContent(Layers.Count + " of " + MaxLayers + " layers.","The total number of layers of the maximum ammount of layers"));
		if(!showLoadWin)
			GUI.enabled = true;
		GUI.EndGroup();
	}
	void RenderContent()
	{
		GUI.Box(new Rect(5, 35, EmblemSize.width + 30, EmblemSize.height + 30), "Preview");
		GUI.BeginGroup(new Rect(20f, 55f, EmblemSize.width, EmblemSize.height));
		foreach(Layer l in Layers)
		{
			if(l.enabled)
				GUI.DrawTexture(new Rect(l.posX, l.posY, EmblemSize.width, EmblemSize.height), l.texture, ScaleMode.ScaleAndCrop, l.blend);
		}
		GUI.EndGroup();
	}
	Color InvertColor(Color color, bool invertAplha)
	{
		Color newColor;
		newColor.r = 1 - color.r;
		newColor.g = 1 - color.g;
		newColor.b = 1 - color.b;
		if(invertAplha)
			newColor.a = 1- color.a;
		else
			newColor.a = color.a;
		return newColor;
	}
}
