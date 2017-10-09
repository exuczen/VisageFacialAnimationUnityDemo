using UnityEngine;
using System.Collections;

public class VideoPreview : MonoBehaviour
{
	public VisageTracker Tracker;
	public bool PreviewResults;
	public Vector2 Offset;
	public float DesiredScreenWidth;
	[HideInInspector]
	public float
		Width;
	[HideInInspector]
	public float
		Height;
	public Material BGRMaterial;
	public Material LineMaterial;
	
	// Update is called once per frame
	void Update ()
	{
	}

	void OnRenderObject ()
	{   
		float scale = DesiredScreenWidth * Screen.width / Tracker.ImageWidth;
		Width = DesiredScreenWidth * Screen.width;
		Height = Tracker.ImageHeight * scale;

		if (Tracker.TrackerStatus != TrackStatus.Off) {
            
			if (Tracker.Frame != null && Camera.current != Camera.main) {
				Rect rect = new Rect (Offset.x, Screen.height - Offset.y, Width, -Height);

				GL.PushMatrix ();
				GL.LoadPixelMatrix ();
				#if UNITY_ANDROID
				Graphics.DrawTexture (rect, Tracker.Frame, new Rect (0f, (float)Tracker.ImageHeight / Tracker.TexHeight, (float)Tracker.ImageWidth / Tracker.TexWidth, -(float)Tracker.ImageHeight / Tracker.TexHeight), 0, 0, 0, 0);
				#else
				Graphics.DrawTexture (rect, Tracker.Frame, new Rect (0f, (float)Tracker.ImageHeight / Tracker.TexHeight, (float)Tracker.ImageWidth / Tracker.TexWidth, -(float)Tracker.ImageHeight / Tracker.TexHeight), 0, 0, 0, 0, BGRMaterial);
				#endif
				GL.PopMatrix ();
			}

		}

		if (Tracker.TrackerStatus != TrackStatus.Ok || !PreviewResults)
			return;

		// get chin points
		float[] chinPoints = GetFeaturePoints (new int[] {
            2,	14,
			2,	12,
			2,	1,
			2,	11,
			2,	13,
        });


		// draw chin
		DrawFeatureLines (chinPoints, Color.green);

		// get inner lip points
		float[] innerLipPoints = GetFeaturePoints (new int[] {
            2,	2,
			2,	6,
			2,	4,
			2,	8,
			2,	3,
			2,	9,
			2,	5,
			2,	7,
            2,	2,
        });

		// draw inner lip
		DrawFeatureLines (innerLipPoints, Color.green);

		// get outer lip
		float[] outerLipPoints = GetFeaturePoints (new int[] {
            8,	1,
			8,	10,
			8,	5,
			8,	3,
			8,	7,
			8,	2,
			8,	8,
			8,	4,
			8,	6,
			8,	9,
            8,	1,
        });

		// draw outer lip
		DrawFeatureLines (outerLipPoints, Color.green);

		// get nose part one lines
		float[] nose1Points = GetFeaturePoints (new int[] {
            9,	15,
            9,	4,
			9,	2,
			9,	3,
			9,	1,
			9,	5,
            9,	15,
        });

		// get nose part two lines
		float[] nose2Points = GetFeaturePoints (new int[] {
            9,	6,
			9,	7,
			9,	13,
			9,	12,
			9,	14,
            9,	6,
        });

		// get nose part three lines
		float[] nose3Points = GetFeaturePoints (new int[] {
            9,	14,
			9,	2,
			9,	13,
			9,	1,
        });

		// draw nose
		DrawFeatureLines (nose1Points, Color.green);
		DrawFeatureLines (nose2Points, Color.green);
		DrawFeatureLines (nose3Points, Color.green);

		// get eye part one lines
		float[] eye1Points = GetFeaturePoints (new int[] {
            3,	12,
			3,	14,
			3,	8,
			3,	10,
            3,  12,
        });


		// get eye part three lines
		float[] eye3Points = GetFeaturePoints (new int[] {
            3,	11,
			3,	13,
			3,	7,
			3,	9,
            3,	11,
        });

		// draw eyes
		Color leyeColor = Color.red;
		if (Tracker.GetActionUnit ("au_leye_closed").Value < 0.3) {
			leyeColor = Color.green;
			float[] lIrisPoint = GetFeaturePoints (new int[] {
                3,	5,
            });
			DrawFeaturePoints (lIrisPoint, Color.white);
		}

		Color reyeColor = Color.red;
		if (Tracker.GetActionUnit ("au_reye_closed").Value < 0.3) {
			reyeColor = Color.green;
			float[] rIrisPoint = GetFeaturePoints (new int[] {
		        3,	6,
            });
			DrawFeaturePoints (rIrisPoint, Color.white);
		}

		DrawFeatureLines (eye3Points, reyeColor);
		DrawFeatureLines (eye1Points, leyeColor);

		// draw gaze


		// get eyebrows
		float[] eyebrowPoints = GetFeaturePoints (new int[] {
            4,	1,
			4,	2,
			4,	3,
			4,	4,
			4,	5,
			4,	6,
        });



		float[] eyebrowLine1Points = GetFeaturePoints (new int[] {
            4,	6,
			4,	4,
			4,	4,
			4,	2,
        });

		float[] eyebrowLine2Points = GetFeaturePoints (new int[] {
        	4,	1,
			4,	3,
			4,	3,
			4,	5,
        });

		// draw eyebrows
		DrawFeaturePoints (eyebrowPoints, Color.green);
		DrawFeatureLines (eyebrowLine1Points, Color.green);
		DrawFeatureLines (eyebrowLine2Points, Color.green);

		// get hair points
		float[] hairPoints = GetFeaturePoints (new int[] {
			11,	2,
			11,	1,
			11,	1,
			11,	3,
        });

		// draw hair
		DrawFeatureLines (hairPoints, Color.green);
	}

	void DrawFeaturePoints (float[] points, Color color)
	{
		// set material
		GL.PushMatrix ();
		LineMaterial.SetPass (0);
		GL.LoadOrtho ();

		// begin drawing
		GL.Begin (GL.LINES);

		// draw
		GL.Color (color);
		Vector3 outterPixelSize = new Vector3 (1f / Screen.width, 1f / Screen.height);
		Vector3 bottomLeft = new Vector3 (Offset.x * outterPixelSize.x, 1f - Offset.y * outterPixelSize.y - Height * outterPixelSize.y, 0f);
		Vector3 screenSize = new Vector3 (Width * outterPixelSize.x, Height * outterPixelSize.y, 0f);
		for (int i = 0; i < points.Length / 2; i++) {
			Vector3 point = new Vector3 (bottomLeft.x + points [i * 2] * screenSize.x, bottomLeft.y + points [i * 2 + 1] * screenSize.y, 0f);

			GL.Vertex (point + new Vector3 (-outterPixelSize.x, -outterPixelSize.y, 0f) * 2f);
			GL.Vertex (point + new Vector3 (outterPixelSize.x, outterPixelSize.y, 0f) * 2f);

			GL.Vertex (point + new Vector3 (-outterPixelSize.x, outterPixelSize.y, 0f) * 2f);
			GL.Vertex (point + new Vector3 (outterPixelSize.x, -outterPixelSize.y, 0f) * 2f);
		}

		// finish drawing
		GL.End ();
		GL.PopMatrix ();
	}

	void DrawFeatureLines (float[] points, Color color)
	{
		// set material
		GL.PushMatrix ();
		LineMaterial.SetPass (0);
		GL.LoadOrtho ();

		// begin drawing
		GL.Begin (GL.LINES);

		// draw
		GL.Color (color);
		Vector3 outterPixelSize = new Vector3 (1f / Screen.width, 1f / Screen.height);
		Vector3 bottomLeft = new Vector3 (Offset.x * outterPixelSize.x, 1f - Offset.y * outterPixelSize.y - Height * outterPixelSize.y, 0f);
		Vector3 screenSize = new Vector3 (Width * outterPixelSize.x, Height * outterPixelSize.y, 0f);

		for (int i = 0; i < points.Length / 2 - 1; i++) {
			Vector3 point = new Vector3 (bottomLeft.x + points [i * 2] * screenSize.x, bottomLeft.y + points [i * 2 + 1] * screenSize.y, 0f);
			Vector3 next = new Vector3 (bottomLeft.x + points [(i + 1) * 2] * screenSize.x, bottomLeft.y + points [(i + 1) * 2 + 1] * screenSize.y, 0f);

			GL.Vertex (point);
			GL.Vertex (next);
		}

		// finish drawing
		GL.End ();
		GL.PopMatrix ();
	}

	float[] GetFeaturePoints (int[] points)
	{
		int number = points.Length / 2;
		int[] groups = new int[number];
		int[] indices = new int[number];
		for (int i = 0; i < number; i++) {
			groups [i] = points [i * 2];
			indices [i] = points [i * 2 + 1];
		}

		float[] positions = new float[number * 2];
		VisageTrackerNative._getFeaturePoints2D (number, groups, indices, positions);
		return positions;
	}
}
