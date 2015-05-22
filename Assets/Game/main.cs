using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class main : MonoBehaviour {
	public int window;
	
	
	private Rect windowRect=new Rect(10,10,Screen.width-20,Screen.height-20);
	private Rect labelRect=new Rect(15,35,150,30);
	private Rect scoreRect=new Rect(15,85,150,30);
	private Rect shapeFieldRect = new Rect (20, 150, 350, 20);
	
	private LineRenderer line;
	
	private List<Vector3> pointList;
	
	private Color c1;
	private Color c2;
	private bool isMousePress;
	private Vector3 mousePos;
	
	private Vector2 pos;
	private Vector2 size;
	
	public float timerMax = 30.0f;
	private float timer = 30.0f;
	private float timerDiff = 5.0f;
	
	public Texture2D progressBarEmpty;
	public Texture2D progressBarFull;
	
	private GUIStyle labelStyle;
	private GUIStyle scoreStyle;
	private GUIStyle gameOverStyle;
	
	private string score;
	public string shapeField;
	public static string shapeFieldDescription = "Введите координаты фигуры в виде x0,y0,x1,y1, (-10,+10) Разделитель запятая. \n После последней координаты тоже нужен разделитель!";
	
	private int numLevel;
	private shapeScript drawShapeScrypt;
	
	
	
	
	void Start()
	{
		c1=Color.cyan;
		c2=Color.green;
		
		line = gameObject.AddComponent<LineRenderer> ();
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.SetVertexCount (0);
		line.SetWidth (0.1f, 0.1f);
		line.SetColors (c1, c2);
		line.useWorldSpace = true;
		pointList = new List<Vector3> ();
		
		window = 1;
		isMousePress = false;
		
		
		pos = new Vector2 (100.0f, 35.0f);
		size = new Vector2 (150.0f, 15.0f);
		
		progressBarEmpty = Texture2D.blackTexture;
		progressBarFull = Texture2D.whiteTexture;
		
		
		labelStyle = new GUIStyle ();
		labelStyle.fontSize = 30;
		labelStyle.normal.textColor = Color.white;
		
		
		scoreStyle = new GUIStyle ();
		scoreStyle.fontSize = 40;
		scoreStyle.normal.textColor = new Color (1.0f, 0.1f, 0.1f);
		score = "0";
		
		gameOverStyle = new GUIStyle ();
		gameOverStyle.fontSize = 50;
		gameOverStyle.normal.textColor = Color.red;
		
		numLevel = 0;
		
		drawShapeScrypt = GameObject.Find ("GameObject").GetComponent<shapeScript> ();
	}
	
	void OnGUI()
	{
		GUI.BeginGroup (new Rect(Screen.width/2-100,Screen.height/2-100,200,200));
		if (window == 1)
		{
			if(GUI.Button(new Rect(10,30,180,30),"Play"))
				window=2;
			if(GUI.Button(new Rect(10,70,180,30),"Shape Options"))
				window=6;
			if(GUI.Button(new Rect(10,110,180,30),"Exit"))
				window=5;
		}
		if (window == 3)
		{
			if(GUI.Button(new Rect(10,30,180,30),"Resume"))
				window=2;
			if(GUI.Button(new Rect(10,70,180,30),"Shape Options"))
				window=7;
			if(GUI.Button(new Rect(10,110,180,30),"Exit"))
				window=5;
		}
		GUI.EndGroup ();
		
		
		if (window == 4)
		{
			GUI.BeginGroup (new Rect(Screen.width/2-300,Screen.height/2-100,1000,400));
			GUI.Label(new Rect(150.0f,0.0f,400.0f,80.0f),"GAME OVER",gameOverStyle);
			GUI.Label(new Rect(180.0f,80.0f,300.0f,50.0f),"Your score:"+score,scoreStyle);
			if(GUI.Button(new Rect(200,150,200,30),"Restart"))
			{
				timer=timerMax;
				score="0";
				window=2;
			}
			if(GUI.Button(new Rect(200,190,200,30),"Shape Options"))
				window=6;
			if(GUI.Button(new Rect(200,230,200,30),"Exit"))
				window=5;
			GUI.EndGroup ();
		}
		
		if (window == 7)
		{
			GUI.Label(new Rect (shapeFieldRect.x,shapeFieldRect.y-shapeFieldRect.height-30,shapeFieldRect.width,60),shapeFieldDescription);
			shapeField=GUI.TextField(shapeFieldRect,shapeField);
			if(GUI.Button(new Rect(shapeFieldRect.x+shapeFieldRect.width/2-100 ,shapeFieldRect.y+shapeFieldRect.height+10,200,30),"Add shape"))
				drawShapeScrypt.addPoints (shapeField);
			
			if(GUI.Button(new Rect(10,30,180,30),"Resume"))
				window=2;
		}
		if (window == 6)
		{
			GUI.Label(new Rect (shapeFieldRect.x,shapeFieldRect.y-shapeFieldRect.height-30,shapeFieldRect.width,60),shapeFieldDescription);
			shapeField=GUI.TextField(shapeFieldRect,"0.0, 0.0, 2.0, 0.0, 0.0, 2.0, 0.0, 0.0,");
			if(GUI.Button(new Rect(shapeFieldRect.x+shapeFieldRect.width/2-100 ,shapeFieldRect.y+shapeFieldRect.height+10,200,30),"Add shape"))
				drawShapeScrypt.addPoints (shapeField);
			
			if(GUI.Button(new Rect(10,30,180,30),"Back"))
				window=1;
		}
		
		
		
		if (window == 2)
			windowRect = GUI.Window (0, windowRect, WindowFunction,"");
		
		if(window==5)
			Application.Quit ();
		
	}
	void WindowFunction(int windowId)
	{
		GUI.Label (labelRect, "Timer", labelStyle);
		
		GUI.BeginGroup (new Rect (windowRect.x,windowRect.y,size.x+labelRect.width,size.y+labelRect.height));
		GUI.DrawTexture (new Rect (pos.x+labelRect.x, pos.y, size.x, size.y), progressBarEmpty);
		GUI.DrawTexture (new Rect (pos.x+labelRect.x, pos.y, size.x * (timer/timerMax), size.y), progressBarFull);
		GUI.EndGroup ();
		
		GUI.Label (scoreRect, "Score:", labelStyle);
		GUI.Label (new Rect(scoreRect.x+scoreRect.width, scoreRect.y,50.0f,scoreRect.height), score, scoreStyle);
		
		
		
		
		if (GUI.Button (new Rect (15.0f, windowRect.height - 50, 180, 30), "Pause"))
		{
			if(window==2)
				window = 3;
			else
				window = 2;
		}
		if (GUI.Button (new Rect (15.0f, windowRect.height - 100, 180, 30), "Change shape")) 
		{
			numLevel= Random.Range(0,drawShapeScrypt.shapesCount());
			timer=timerMax;
		}
		
		if(numLevel>0 && numLevel<drawShapeScrypt.shapesCount())
			drawShapeScrypt.drawShape (numLevel);
		
	}
	void Update()
	{
		if (window == 2) 
		{
			if(timer<=0.0f)
			{
				Debug.Log ("Time is up");
				window=4;
			}
			else
			{
				timer -= Time.deltaTime;
			}
			
			if (Input.GetMouseButtonDown (0)) 
			{
				isMousePress = true;
				pointList.RemoveRange (0, pointList.Count);
				line.SetVertexCount (0);
			} 	else
				if (Input.GetMouseButtonUp (0))
			{
				isMousePress = false;
				if(pointsAnalyze(pointList))
				{
					numLevel++;
					timer=timerMax-timerDiff;
					timerDiff+=5;
				}
			}
			
			
			if (isMousePress) 
			{
				mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				mousePos.z = 0.0f;
				pointList.Add (mousePos);
				
				line.SetVertexCount (pointList.Count);
				line.SetPosition (pointList.Count - 1, pointList [pointList.Count - 1]);
			}
		}
		
	}
	public bool pointsAnalyze(List<Vector3> inputList)
	{
		return false;
	}
	
}
