using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class main : MonoBehaviour
{
	public int window;
	private Rect windowRect = new Rect (10, 10, Screen.width - 20, Screen.height - 20);
	private Rect labelRect = new Rect (15, 35, 150, 30);
	private Rect scoreRect = new Rect (15, 85, 150, 30);
	private Rect shapeFieldRect = new Rect (20, 150, 350, 20);
	private LineRenderer line;
	private List<Vector3> mousePointList;
	private List<Vector3> tempDrawList;
	//	private List<Vector3> drawingShape;
	
	private Color mainColor;
	private Color grantedColor;
	private Color deniedColor;

	private bool isMousePress;
	private bool isAddShape;
	private Vector3 mousePos;
	private Vector2 pos;
	private Vector2 size;
	public float timerMax = 30.0f;
	private float timer = 30.0f;
	private float timerDiff = 5.0f;
	public Texture2D progressBarEmpty;
	public Texture2D progressBarFull;
	private bool drawState;
	private GUIStyle labelStyle;
	private GUIStyle scoreStyle;
	private GUIStyle gameOverStyle;
	private int score;
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

		public bool Equals (isStateChange obj)
		{
			return 
				(this.isXDiff == obj.isXDiff &&
				this.isXDiffSignChange == obj.isXDiffSignChange &&
				this.isYDiff == obj.isYDiff &&
				this.isYDiffSignChange == obj.isYDiffSignChange);
		}

	};


	void Start ()
	{
		mainColor = Color.cyan;
		grantedColor = Color.green;
		deniedColor = Color.red;
		
		line = gameObject.AddComponent<LineRenderer> ();
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.SetVertexCount (0);
		line.SetWidth (0.1f, 0.1f);
		line.SetColors (mainColor, mainColor);
		line.useWorldSpace = true;

		mousePointList = new List<Vector3> ();
		tempDrawList = new List<Vector3> ();

		window = 1;
		isMousePress = false;
		isAddShape = false;

		drawState = false;
		
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

		score = 0;
		
		gameOverStyle = new GUIStyle ();
		gameOverStyle.fontSize = 50;
		gameOverStyle.normal.textColor = Color.red;
		
		numLevel = 0;
		
		drawShapeScrypt = GameObject.Find ("GameObject").GetComponent<shapeScript> ();
	}
	
	void OnGUI ()
	{
		GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
		if (window == 1) {
			if (GUI.Button (new Rect (10, 30, 180, 30), "Play"))
				window = 2;
			if (GUI.Button (new Rect (10, 70, 180, 30), "Shape Options"))
				window = 6;
			if (GUI.Button (new Rect (10, 110, 180, 30), "Exit"))
				window = 5;
		}
		if (window == 3) {
			drawState = false;

			if (GUI.Button (new Rect (10, 30, 180, 30), "Resume"))
				window = 2;
			if (GUI.Button (new Rect (10, 70, 180, 30), "Shape Options"))
				window = 7;
			if (GUI.Button (new Rect (10, 110, 180, 30), "Exit"))
				window = 5;
		}
		GUI.EndGroup ();
		
		
		if (window == 4) 
		{
			drawState = false;
			line.enabled = false;
			drawShapeScrypt.enableLineRenderer (false);

			GUI.BeginGroup (new Rect (Screen.width / 2 - 300, Screen.height / 2 - 100, 1000, 400));
			GUI.Label (new Rect (150.0f, 0.0f, 400.0f, 80.0f), "GAME OVER", gameOverStyle);
			GUI.Label (new Rect (180.0f, 80.0f, 300.0f, 50.0f), "Your score:" + score, scoreStyle);
			if (GUI.Button (new Rect (200, 150, 200, 30), "Restart")) 
			{
				timer = timerMax;
				timerDiff = 5.0f;
				score = 0;
				line.SetVertexCount (0);
				mousePointList.Clear ();
				window = 2;
			}
			if (GUI.Button (new Rect (200, 190, 200, 30), "Shape Options"))
				window = 6;
			if (GUI.Button (new Rect (200, 230, 200, 30), "Exit"))
				window = 5;
			GUI.EndGroup ();
		}

		if (window == 5)
			Application.Quit ();

		if (window == 6) 
		{
			GUI.Label (new Rect (shapeFieldRect.x, shapeFieldRect.y - shapeFieldRect.height - 30, shapeFieldRect.width, 60), shapeFieldDescription);
			shapeField = GUI.TextField (shapeFieldRect, "0.0, 0.0, 2.0, 0.0, 0.0, 2.0, 0.0, 0.0,");
			if (GUI.Button (new Rect (shapeFieldRect.x + shapeFieldRect.width / 2 - 100, shapeFieldRect.y + shapeFieldRect.height + 10, 200, 30), "Add shape"))
				drawShapeScrypt.addPoints (shapeField);
			
			if (GUI.Button (new Rect (10, 30, 180, 30), "Back"))
				window = 1;
			
			drawState = true;
			if (GUI.Button (new Rect (Screen.width / 2 + 100, Screen.height / 2, 300, 60), "Add drawing shape"))
				isAddShape = true;
		}

		if (window == 7) 
		{
			GUI.Label (new Rect (shapeFieldRect.x, shapeFieldRect.y - shapeFieldRect.height - 30, shapeFieldRect.width, 60), shapeFieldDescription);
			shapeField = GUI.TextField (shapeFieldRect, shapeField);
			if (GUI.Button (new Rect (shapeFieldRect.x + shapeFieldRect.width / 2 - 100, shapeFieldRect.y + shapeFieldRect.height + 10, 200, 30), "Add shape"))
				drawShapeScrypt.addPoints (shapeField);
			
			if (GUI.Button (new Rect (10, 30, 180, 30), "Resume"))
				window = 2;

			drawState = true;
			if (GUI.Button (new Rect (Screen.width / 2 + 100, Screen.height / 2, 300, 60), "Add drawing shape"))
				isAddShape = true;
		}

		
		
		if (window == 2) 
		{
			windowRect = GUI.Window (0, windowRect, WindowFunction, "");
			drawState = true;
			drawShapeScrypt.enableLineRenderer (true);

			if ((numLevel >= 0) && (numLevel < drawShapeScrypt.shapesCount ()))
				drawShapeScrypt.drawShape (numLevel);
		}

	}

	void WindowFunction (int windowId)
	{
		GUI.Label (labelRect, "Timer", labelStyle);
		
		GUI.BeginGroup (new Rect (windowRect.x, windowRect.y, size.x + labelRect.width, size.y + labelRect.height));
		GUI.DrawTexture (new Rect (pos.x + labelRect.x, pos.y, size.x, size.y), progressBarEmpty);
		GUI.DrawTexture (new Rect (pos.x + labelRect.x, pos.y, size.x * (timer / timerMax), size.y), progressBarFull);
		GUI.EndGroup ();
		
		GUI.Label (scoreRect, "Score:", labelStyle);
		GUI.Label (new Rect (scoreRect.x + scoreRect.width, scoreRect.y, 50.0f, scoreRect.height), score.ToString (), scoreStyle);
		
		
		
		
		if (GUI.Button (new Rect (15.0f, windowRect.height - 50, 180, 30), "Pause")) {
			if (window == 2) {
				drawShapeScrypt.enableLineRenderer (false);
				window = 3;
			} else {
				drawShapeScrypt.enableLineRenderer (true);
				window = 2;
			}
		}
		if (GUI.Button (new Rect (15.0f, windowRect.height - 100, 180, 30), "Change shape")) {
			numLevel = UnityEngine.Random.Range (0, drawShapeScrypt.shapesCount ());
			timer = timerMax;
		}

	}

	void Update ()
	{
		if (window == 2) 
		{
			line.enabled=true;

			if (timer <= 0.0f) {
				Debug.Log ("Time is up");
				window = 4;
			} else 
				timer -= Time.deltaTime;

			if (!isMousePress && isAddShape) 
			{
				if (pointsAnalyze (mousePointList)) 
				{
					numLevel = UnityEngine.Random.Range (0, drawShapeScrypt.shapesCount ());

					line.SetColors (grantedColor,grantedColor);

					timer = timerMax - timerDiff;
					timerDiff += 5;

					score++;
				}
				else
					line.SetColors (deniedColor,deniedColor);

				isAddShape = false;
			}

		}

		if (window == 7 || window == 6) 
		{
			if (!isMousePress && isAddShape) 
			{

				List<Vector3> tempList = new List<Vector3> ();
			
				if (mousePointList.Count >= 10) {
					tempList = figureApproximation (mousePointList);

					line.SetVertexCount (tempList.Count + 1);
					for (int i=0; i<tempList.Count; i++)
						line.SetPosition (i, tempList [i]);
					line.SetPosition (tempList.Count, tempList [0]);

					translateFigureToZero (tempList);
					drawShapeScrypt.addPoints (tempList);
					isAddShape = false;
				}
			}

		}


		if (drawState == true)
			mousePaint (mousePointList);


		
	}

	public void mousePaint (List<Vector3> inputShapeList)
	{


		if (Input.GetMouseButtonDown (0)) 
		{
			isMousePress = true;
			inputShapeList.Clear ();
			line.SetVertexCount (0);
			line.SetColors (mainColor, mainColor);
		} else if (Input.GetMouseButtonUp (0)) 
		{
			isMousePress = false;
			isAddShape = true;
		}
		
		
		if (isMousePress) {
			mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePos.z = 0.0f;
			inputShapeList.Add (mousePos);
			
			line.SetVertexCount (inputShapeList.Count);
			line.SetPosition (inputShapeList.Count - 1, inputShapeList [inputShapeList.Count - 1]);
		}

	}

	public bool pointsAnalyze (List<Vector3> inputPointList)
	{
		List<Vector3> tempDrawList;

		if (inputPointList.Count <= 10)
			return false;

		tempDrawList = figureApproximation (inputPointList);



		line.SetVertexCount (tempDrawList.Count + 1);
		for (int i=0; i<tempDrawList.Count; i++)
			line.SetPosition (i, tempDrawList [i]);
		line.SetPosition (tempDrawList.Count, tempDrawList [0]);
		
		float scaleRatio = scaleRatioLeftToRightFigures (drawShapeScrypt.currentFigure (), tempDrawList);
		scaleFigure (tempDrawList, scaleRatio);
		translateFigureToZero (tempDrawList);


		if (figuresCompare (drawShapeScrypt.currentFigure (), tempDrawList, 5)) 
			return true;

		return false;
	}

	public void translateFigureToZero (List<Vector3> inputFigure)
	{
		if (inputFigure.Count <= 0)
			return;
/*
		Debug.Log ("before translation");
		foreach (Vector3 i in inputFigure)
			Debug.Log (i);
*/
		int minXIndex = 0;
		int minYIndex = 0;

		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [minXIndex].x > inputFigure [i].x)
				minXIndex = i;
		}
		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [minYIndex].y > inputFigure [i].y)
				minYIndex = i;
		}


		Vector3 minPoint = new Vector3 (inputFigure [minXIndex].x, inputFigure [minYIndex].y, inputFigure [minXIndex].z);
		Debug.Log ("minXIndex" + minXIndex + "minYIndex" + minYIndex + " minPoint" + minPoint);

		for (int i=0; i<inputFigure.Count; i++) {
			inputFigure [i] = inputFigure [i] - minPoint;

		}

/*		Debug.Log ("after translation");
		foreach (Vector3 i in inputFigure)
			Debug.Log (i);
*/

	}
	public void translateFigure (List<Vector3> inputFigure, Vector3 translatePoint)
	{
		if (inputFigure.Count <= 0)
			return;

		Debug.Log ("Translate Point" + translatePoint);
/*		Debug.Log ("Before translation");
		foreach (Vector3 i in inputFigure)
			Debug.Log (i);
*/

		for (int i=0; i<inputFigure.Count; i++) 
		{
			inputFigure [i] = inputFigure [i] + translatePoint;	
		}
		

/*		Debug.Log ("after translation");
		foreach (Vector3 i in inputFigure)
			Debug.Log (i);

*/		
	}

	public void scaleFigure (List<Vector3> inputFigure, float ratio)
	{
		if (inputFigure.Count <= 0)
			return;

		Debug.Log ("Scale ratio" + ratio);

/*		Debug.Log ("Before scale");
		foreach (Vector3 i in inputFigure)
			Debug.Log (i);
*/
		for (int i=0; i<inputFigure.Count; i++)
			inputFigure [i] = inputFigure [i] * ratio;

/*		Debug.Log ("After scale");
		foreach (Vector3 i in inputFigure)
			Debug.Log (i);
*/
	}

	private float scaleRatioLeftToRightFigures (List<Vector3> leftFigure, List<Vector3> rightFigure)
	{
		if (leftFigure.Count <= 0)
			return 0.0f;

		if (rightFigure.Count <= 0)
			return 0.0f;

		float leftFigureLenght = figureMagnitude (leftFigure);
		float rightFigureLenght = figureMagnitude (rightFigure);

		float scaleRatio = leftFigureLenght / rightFigureLenght;

		return scaleRatio;
	}

	private float figureMagnitude (List<Vector3> inputFigure)
	{
		if (inputFigure.Count <= 0)
			return 0.0f;

		int minXIndex = 0;
		int minYIndex = 0;
		
		int maxXIndex = 0;
		int maxYIndex = 0;

		Vector3 maxPoint;
		Vector3 minPoint;

		float figureLenght = 0.0f;

		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [maxXIndex].x < inputFigure [i].x)
				maxXIndex = i;
		}
		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [maxYIndex].x < inputFigure [i].x)
				maxYIndex = i;
		}


		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [minXIndex].x > inputFigure [i].x)
				minXIndex = i;
		}
		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [minYIndex].y > inputFigure [i].y)
				minYIndex = i;
		}

		maxPoint = new Vector3 (inputFigure [maxXIndex].x, inputFigure [maxYIndex].y, inputFigure.First ().z);
		minPoint = new Vector3 (inputFigure [minXIndex].x, inputFigure [minYIndex].y, inputFigure.First ().z);

		figureLenght = Vector3.Magnitude (maxPoint - minPoint);

		return figureLenght;
	}

	public bool figuresCompare (List<Vector3> leftFigure, List<Vector3> rightFigure, float pointDiff)
	{
		if (leftFigure.Count <= 0 || rightFigure.Count <= 0)
			return false;

		List<Vector3> tempLeftList = new List<Vector3> (leftFigure);
		List<Vector3> tempRightList = new List<Vector3> (rightFigure);



		float maxX = 0, maxY = 0;
		float minX = 0, minY = 0;
		
		float pogreshnostX = 0.0f, pogreshnostY = 0.0f;
		
		maxX = minX = leftFigure.First ().x;
		maxY = minY = leftFigure.First ().y;
		foreach (Vector3 x in leftFigure) {
			maxX = Mathf.Max (maxX, x.x);
			maxY = Mathf.Max (maxY, x.y);
			minX = Mathf.Min (minX, x.x);
			minY = Mathf.Min (minY, x.y);
		}
		
		pogreshnostX = Mathf.Abs ((maxX - minX)) / pointDiff;
		pogreshnostY = Mathf.Abs ((maxY - minY)) / pointDiff;

		Debug.Log ("Compare pogreshnostX:" + pogreshnostX);
		Debug.Log ("Compare pogreshnostY:" + pogreshnostY);



		/*
		Debug.Log ("LeftFigure");
		foreach (Vector3 i in leftFigure)
			Debug.Log (i);

		Debug.Log ("rightFigure");
		foreach (Vector3 i in rightFigure)
			Debug.Log (i);
*/

		for (int i=0; i<tempLeftList.Count; i++) {
			int index;
			index = tempRightList.FindIndex (x
			                        =>
			{
				if (tempLeftList [i].x + pogreshnostX >= x.x && tempLeftList [i].x - pogreshnostX <= x.x) {
					if (tempLeftList [i].y + pogreshnostY >= x.y && tempLeftList [i].y - pogreshnostY <= x.y) {
						Debug.Log ("compare" + tempLeftList [i] + ":" + x);
						return true;
					} else {
						Debug.Log (" not compare second" + tempLeftList [i] + ":" + x);
						return false;
					}
				} else {
					Debug.Log (" not compare first" + tempLeftList [i] + ":" + x);
					return false;
				}
			});
//			if (index >= 0 && index<tempRightList.Count)
//				tempRightList.RemoveAt (i);
//			else
//				return false;

			if (index == -1)
				return false;
		}
		return true;

	}

	private List<Vector3> figureApproximation (List<Vector3> incomigFigure)
	{
		List<Vector3> outputFigure = new List<Vector3> ();
		float maxX = 0, maxY = 0;
		float minX = 0, minY = 0;
		
		float pogreshnostX = 0.0f, pogreshnostY = 0.0f;
		
		maxX = minX = incomigFigure.First ().x;
		maxY = minY = incomigFigure.First ().y;
		foreach (Vector3 x in incomigFigure) {
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
		pogreshnostX = Mathf.Abs ((maxX - minX)) / 25.0f;
		pogreshnostY = Mathf.Abs ((maxY - minY)) / 25.0f;
		
		Debug.Log ("pogreshnostX:" + pogreshnostX);
		Debug.Log ("pogreshnostY:" + pogreshnostY);


		int currIndexPoint = 0;
		int indexStep = Convert.ToInt32 ((Mathf.Abs (maxX - minX) + Mathf.Abs (maxY - minY)) / 3);
		Debug.Log ("indexStep:" + indexStep);
		
		float Xdiff = 0.0f;
		float predXdiff = 0.0f;
		float Ydiff = 0.0f;
		float predYdiff = 0.0f;
		
		isStateChange shapeChange = new isStateChange ();
		isStateChange shapeChangeBefore = new isStateChange ();
		

		for (currIndexPoint=0; currIndexPoint<incomigFigure.Count; currIndexPoint+=indexStep) {
			if (incomigFigure.Count > currIndexPoint + indexStep) {
				Xdiff = incomigFigure [currIndexPoint].x - incomigFigure [currIndexPoint + indexStep].x;
				if (Mathf.Abs (Xdiff) > pogreshnostX) {
					shapeChange.isXDiff = true;
					if (Mathf.Sign (Xdiff) != Mathf.Sign (predXdiff))
						shapeChange.isXDiffSignChange = true;
					else
						shapeChange.isXDiffSignChange = false;
				} else
					shapeChange.isXDiff = false;
				
				
				Ydiff = incomigFigure [currIndexPoint].y - incomigFigure [currIndexPoint + indexStep].y;
				if (Mathf.Abs (Ydiff) > pogreshnostY) {
					shapeChange.isYDiff = true;
					if (Mathf.Sign (Ydiff) != Mathf.Sign (predYdiff))
						shapeChange.isYDiffSignChange = true;
					else
						shapeChange.isYDiffSignChange = false;
				} else
					shapeChange.isYDiff = false;
				
				if (!shapeChange.Equals (shapeChangeBefore))
					outputFigure.Add (incomigFigure [currIndexPoint + indexStep / 2]);
				
				predXdiff = Xdiff;
				predYdiff = Ydiff;
				shapeChangeBefore = shapeChange;
			}
		}
		
		Debug.Log ("Figure points:" + outputFigure.Count);
		return outputFigure;

	}
	
}
