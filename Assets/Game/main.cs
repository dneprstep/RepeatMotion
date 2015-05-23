using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class main : MonoBehaviour {
	public int window;
	
	
	private Rect windowRect=new Rect(10,10,Screen.width-20,Screen.height-20);
	private Rect labelRect=new Rect(15,35,150,30);
	private Rect scoreRect=new Rect(15,85,150,30);
	private Rect shapeFieldRect = new Rect (20, 150, 350, 20);
	
	private LineRenderer line;
	
	private List<Vector3> pointList;
	private List<Vector3> drawingShape;
	
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
	
	struct isStateChange
	{
		public bool isXDiff { get; set; }
		public bool isXDiffSignChange{ get; set; }
		public bool isYDiff{ get; set; }
		public bool isYDiffSignChange{ get; set; }


		public bool Equals(isStateChange obj)
		{
			return 
				(this.isXDiff == obj.isXDiff &&
				this.isXDiffSignChange == obj.isXDiffSignChange &&
				this.isYDiff == obj.isYDiff &&
				this.isYDiffSignChange == obj.isYDiffSignChange);
		}

	};


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
		drawingShape = new List<Vector3> ();
		
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
			numLevel= UnityEngine.Random.Range(0,drawShapeScrypt.shapesCount());
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
				drawingShape.Clear ();
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
		if (pointList.Count <= 1)
			return false;

		float maxX=0,maxY=0;
		float minX=0,minY=0;
		float maxXmaxY=0, maxXminY=0;
		float minXmaxY=0, minXminY=0;

		float maxYmaxX=0, maxYminX=0;
		float minYmaxX=0, minYminX=0;

		List<Vector3> maxXList;
		List<Vector3> maxYList;
		List<Vector3> minXList;
		List<Vector3> minYList;

		float pogreshnostX = 0.0f, pogreshnostY = 0.0f;
		float pogreshnostDelit = 1.0f;

		int pointsCount = 0;

		maxX = minX = pointList.First ().x;
		maxY = minY = pointList.First ().y;
		foreach (Vector3 x in pointList) 
		{
			maxX = Mathf.Max (maxX, x.x);
			maxY = Mathf.Max (maxY, x.y);
			minX = Mathf.Min (minX, x.x);
			minY = Mathf.Min (minY, x.y);
		}
/*		Debug.Log ("maxX:" + maxX);
		Debug.Log ("maxY:" + maxY);
		Debug.Log ("minX:" + minX);
		Debug.Log ("minY:" + minY);
*/		
		pogreshnostX = Mathf.Abs((maxX - minX)) / 10.0f;
		pogreshnostY = Mathf.Abs((maxY - minY)) / 10.0f;

		Debug.Log ("pogreshnostX:" + pogreshnostX);
		Debug.Log ("pogreshnostY:" + pogreshnostY);


		int currIndexPoint = 0;
		int indexStep = Convert.ToInt32((Mathf.Abs(maxX - minX)+Mathf.Abs(maxY - minY))/3);
		Debug.Log ("indexStep:" + indexStep);

		float Xdiff = 0.0f;
		float predXdiff = 0.0f;
		float Ydiff = 0.0f;
		float predYdiff = 0.0f;

		isStateChange shapeChange=new isStateChange();
		isStateChange shapeChangeBefore=new isStateChange();

	/*	shapeChange.isXDiff = false;
		shapeChange.isXDiffSignChange = false;
		shapeChange.isYDiff = false;
		shapeChange.isYDiffSignChange = false;

		shapeChangeBefore.isXDiff = false;
		shapeChangeBefore.isXDiffSignChange = false;
		shapeChangeBefore.isYDiff = false;
		shapeChangeBefore.isYDiffSignChange = false;
*/
//		Vector2 tempVec2 = new Vector2 ();

//		float pogreshnost = 0.5f;
//		List<Vector3> drawingShape = new List<Vector3> ();

//		int minIndexPoint = 0;
	

	/*	//index minimal x point
		for (int i=0; i<pointList.Count;i++) 
		{
			if(pointList[minIndexPoint].x>pointList[i].x)
				minIndexPoint=i;
		}
*/

		for (currIndexPoint=0; currIndexPoint<pointList.Count; currIndexPoint+=indexStep) 
		{
			if(pointList.Count>currIndexPoint+indexStep)
			{
				Xdiff= pointList[currIndexPoint].x-pointList[currIndexPoint+indexStep].x;
				if(Mathf.Abs(Xdiff)>pogreshnostX )
				{
					shapeChange.isXDiff=true;
					if(Mathf.Sign (Xdiff)!=Mathf.Sign (predXdiff))
						shapeChange.isXDiffSignChange=true;
					else
						shapeChange.isXDiffSignChange=false;
				}
				else
					shapeChange.isXDiff=false;


				Ydiff= pointList[currIndexPoint].y-pointList[currIndexPoint+indexStep].y;
				if(Mathf.Abs(Ydiff)>pogreshnostY)
				{
					shapeChange.isYDiff=true;
					if(Mathf.Sign (Ydiff)!=Mathf.Sign (predYdiff))
						shapeChange.isYDiffSignChange=true;
					else
						shapeChange.isYDiffSignChange=false;
				}

				else
					shapeChange.isYDiff=false;

				if(!shapeChange.Equals (shapeChangeBefore))
					drawingShape.Add (pointList[currIndexPoint+indexStep/2]);

				predXdiff=Xdiff;
				predYdiff=Ydiff;
				shapeChangeBefore=shapeChange;
			}
		}

		if (drawingShape.Count <= 0)
			return false;
		line.SetVertexCount (drawingShape.Count+1);
		for(int i=0;i<drawingShape.Count;i++)
			line.SetPosition (i, drawingShape[i]);
		line.SetPosition (drawingShape.Count, drawingShape[0]);



		/*
		foreach (Vector3 x in pointList) 
		{
			maxX = Mathf.Max (maxX, x.x);
			maxY = Mathf.Max (maxY, x.y);
			minX = Mathf.Min (minX, x.x);
			minY = Mathf.Min (minY, x.y);
		}
		Debug.Log ("maxX:" + maxX);
		Debug.Log ("maxY:" + maxY);
		Debug.Log ("minX:" + minX);
		Debug.Log ("minY:" + minY);

		pogreshnostX = Mathf.Abs((maxX - minX)) / 3.0f;
		pogreshnostY = Mathf.Abs((maxY - minY)) / 3.0f;

		Debug.Log ("pogreshnostX:" + pogreshnostX);
		Debug.Log ("pogreshnostY:" + pogreshnostY);




		maxXList = pointList.FindAll (
				(x) => 
			 {
			if(maxX+pogreshnostX >= x.x && maxX-pogreshnostX<=x.x ) 
					return true;
				else return false;
			}
		);
		if (maxXList.Count > 0) 
		{
			maxXmaxY = maxXList [0].y;
			maxXminY = maxXList [0].y;
			foreach (Vector3 x in maxXList) {
				maxXmaxY = Mathf.Max (maxXmaxY, x.y);
				maxXminY = Mathf.Min (maxXminY, x.y);
			}
			if (maxXminY + pogreshnostX / pogreshnostDelit >= maxXmaxY && maxXmaxY - pogreshnostX / pogreshnostDelit <= maxXminY) 
				maxXmaxY = maxXminY = maxX;
			else
				pointsCount++;
			Debug.Log ("MaxXmaxY:" + maxXmaxY);
			Debug.Log ("MaxXminY:" + maxXminY);
		}



		
		minXList = pointList.FindAll (
			(x) => 
			{
			if(minX+pogreshnostX >= x.x && minX-pogreshnostX<=x.x ) 
				return true;
			else return false;
		}
		);

		if (minXList.Count > 0) 
		{
			minXmaxY = minXList [0].y;
			minXminY = minXList [0].y;
			foreach (Vector3 x in minXList) {
				minXmaxY = Mathf.Max (minXmaxY, x.y);
				minXminY = Mathf.Min (minXminY, x.y);
			}
			if (minXminY + pogreshnostX / pogreshnostDelit >= minXmaxY && minXmaxY - pogreshnostX / pogreshnostDelit <= minXminY) 
				minXmaxY = minXminY = minX;
			else
				pointsCount++;
				
			Debug.Log ("MinXmaxY:" + minXmaxY);
			Debug.Log ("MinXminY:" + minXminY);
		}













		maxYList = pointList.FindAll (
			(y) => 
			{
			if(maxY+pogreshnostY >= y.y && maxY-pogreshnostY<=y.y ) 
				return true;
			else return false;
		}
		);

		if (maxYList.Count > 0) 
		{
			maxYmaxX = maxYList [0].x;
			maxYminX = maxYList [0].x;
			foreach (Vector3 y in maxYList) {
				maxYmaxX = Mathf.Max (maxYmaxX, y.x);
				maxYminX = Mathf.Min (maxYminX, y.x);
			}
			if (maxYminX + pogreshnostY / pogreshnostDelit >= maxYmaxX && maxYmaxX - pogreshnostY / pogreshnostDelit <= maxYminX) 
				maxYmaxX = maxYminX = maxY;
			else
				pointsCount++;

			Debug.Log ("MaxYmaxX:" + maxYmaxX);
			Debug.Log ("MaxYminX:" + maxYminX);
		}



		minYList = pointList.FindAll (
			(y) => 
			{
			if(minY+pogreshnostY >= y.y && minY-pogreshnostY<=y.y ) 
				return true;
			else return false;
		}
		);
		if (minYList.Count > 0) 
		{
			minYmaxX = minYList [0].x;
			minYminX = minYList [0].x;
			foreach (Vector3 y in minYList) {
				minYmaxX = Mathf.Max (minYmaxX, y.x);
				minYminX = Mathf.Min (minYminX, y.x);
			}
			if (minYminX + pogreshnostY / pogreshnostDelit >= minYmaxX && minYmaxX - pogreshnostY / pogreshnostDelit <= minYminX) 
				minYmaxX = minYminX = minY;
			else
				pointsCount++;

			Debug.Log ("minYmaxX:" + minYmaxX);
			Debug.Log ("minYminX:" + minYminX);
		}


		Debug.Log (maxYminX + ":" + minXmaxY + "#" + maxYmaxX + ":" + maxXmaxY);
		Debug.Log (minYminX + ":" + minXminY + "#" + minYmaxX + ":" + maxXminY);





		
		


		Vector3 point1 = new Vector3 (maxYminX, minXmaxY, 0.0f);
		Vector3 point2 = new Vector3 (minYminX, minXminY, 0.0f);
		Vector3 point3 = new Vector3 (minYmaxX, maxXminY, 0.0f);
		Vector3 point4 = new Vector3 (maxYmaxX, maxXmaxY, 0.0f);

		
		line.SetVertexCount (5);
		line.SetPosition(0,point1);
		line.SetPosition(1,point2);
		line.SetPosition(2,point3);
		line.SetPosition(3,point4);
		line.SetPosition(4,point1);

*/		
		

		return false;
	}
	
}
