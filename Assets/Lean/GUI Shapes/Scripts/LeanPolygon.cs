using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Lean.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lean.Gui
{
	/// <summary>This component allows you to create UI elements with a custom polygon shape.</summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanPolygon")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Polygon")]
	public class LeanPolygon : MaskableGraphic
	{
		/// <summary>This allows you to set the blur radius in local space.</summary>
		public float Blur { set { blur = value; } get { return blur; } } [SerializeField] private float blur = 0.5f;

		/// <summary>This allows you to set the thickness of the border in local space.</summary>
		public float Thickness { set { thickness = value; } get { return thickness; } } [SerializeField] private float thickness = -1.0f;

		/// <summary>This list stores all polygon points in local space.</summary>
		public List<Vector2> Points { get { if (points == null) points = new List<Vector2>(); return points; } } [SerializeField] private List<Vector2> points;

		private static Texture2D blurTexture;

		private static bool blurTextureSet;

		private static UIVertex vert = UIVertex.simpleVert;

		private static List<Vector2> positions = new List<Vector2>();

		private static List<Vector2> normals = new List<Vector2>();

		public override Texture mainTexture
		{
			get
			{
				return LeanGuiSprite.CachedTexture;
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			LeanGuiSprite.UpdateCache();

			vh.Clear();

			if (CalculateNormals() == true)
			{
				vert.color = color;
				vert.uv0   = LeanGuiSprite.CachedSolid;

				if (thickness < 0.0f)
				{
					if (blur > 0.0f)
					{
						WriteVertexRing(vh, -blur); WriteTriangleDisc(vh);

						vert.uv0 = LeanGuiSprite.CachedClear;

						WriteVertexRing(vh, blur); WriteTriangleRing(vh, 0, LeanTriangulation.Points.Count);
					}
					else
					{
						WriteVertexRing(vh, 0.0f); WriteTriangleDisc(vh);
					}
				}
				else if (thickness > 0.0f)
				{
					if (blur > 0.0f)
					{
						WriteVertexRing(vh, blur - thickness); // Inner
						WriteVertexRing(vh, -blur); // Outer

						vert.uv0 = LeanGuiSprite.CachedClear;

						WriteVertexRing(vh, blur); // Outer Blur Edge
						WriteVertexRing(vh, -blur - thickness); // Inner Blur Edge

						WriteTriangleRing(vh, 0, points.Count);
						WriteTriangleRing(vh, points.Count, points.Count * 2);
						WriteTriangleRing(vh, points.Count * 3, 0);
					}
					else
					{
						WriteVertexRing(vh, -thickness);
						WriteVertexRing(vh, 0.0f);

						WriteTriangleRing(vh, 0, points.Count);
					}
				}
			}
		}

		private void WriteTriangleDisc(VertexHelper vh)
		{
			if (LeanTriangulation.Calculate(positions) == true)
			{
				for (var i = 0; i < LeanTriangulation.Triangles.Count; i++)
				{
					var triangle = LeanTriangulation.Triangles[i]; vh.AddTriangle(triangle.IndexA, triangle.IndexB, triangle.IndexC);
				}
			}
		}

		private void WriteTriangleRing(VertexHelper vh, int innerO, int outerO)
		{
			var innerA = innerO;
			var outerA = outerO;

			for (var i = points.Count - 1; i >= 0; i--)
			{
				var innerB = i + innerO;
				var outerB = i + outerO;

				vh.AddTriangle(innerA, innerB, outerA);
				vh.AddTriangle(outerB, outerA, innerB);

				innerA = innerB;
				outerA = outerB;
			}
		}

		private void WriteVertexRing(VertexHelper vh, float distance)
		{
			positions.Clear();

			for (var i = 0; i < points.Count; i++)
			{
				var position = points[i] + distance * normals[i];

				positions.Add(position);

				vert.position = position;

				vh.AddVert(vert);
			}
		}

		private bool CalculateNormals()
		{
			if (points != null && points.Count > 2)
			{
				normals.Clear();

				var count   = points.Count;
				var normalA = CalculateNormal(points[count - 1] - points[0]);

				for (var i = 0; i < points.Count; i++)
				{
					var normalB = CalculateNormal(points[i] - points[(i + 1) % count]);
					var inset   = normalA + normalB;
					var mag     = inset.sqrMagnitude;
					var direction = Vector2.zero;

					if (mag > 0.0f)
					{
						mag = Mathf.Sqrt(mag);

						direction = (inset / mag) / mag;
					}

					normals.Add(-direction);

					normalA = normalB;
				}

				return true;
			}

			return false;
		}

		private static Vector2 CalculateNormal(Vector2 vector)
		{
			return new Vector2(-vector.y, vector.x).normalized;
		}

#if UNITY_EDITOR
		[MenuItem("GameObject/Lean/GUI/Polygon", false, 1)]
		public static void CreatePolygon()
		{
			Selection.activeObject = LeanHelper.CreateElement<LeanPolygon>(Selection.activeTransform);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Lean.Gui
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanPolygon))]
	public class LeanPolygon_Editor : LeanInspector<LeanPolygon>
	{
		private bool editPoints;

		private Tool editTool;

		private int draggingPoint = -1;

		private static GUIStyle downStyle;

		public GUIStyle EditStyle
		{
			get
			{
				if (editPoints == true)
				{
					if (downStyle == null)
					{
						downStyle = new GUIStyle(EditorStyles.miniButton);

						downStyle.normal.background = downStyle.onActive.background;
					}

					return downStyle;
				}

				return EditorStyles.miniButton;
			}
		}

		protected override void DrawInspector()
		{
			DrawEdit();
			Draw("m_Color", "This allows you to set the color of this element.");
			Draw("m_Material", "This allows you to specify a custom material for this element.");
			Draw("m_RaycastTarget", "Should UI pointers interact with this element?");
			Draw("blur", "This allows you to set the blur radius in local space.");
			Draw("thickness", "This allows you to set the thickness of the border in local space.");
			Draw("points", "");
		}

		protected virtual void OnDisable()
		{
			EndEdit();
		}

		private void BeginEdit()
		{
			editPoints = true;

			editTool = Tools.current;

			Tools.current = Tool.None;
		}

		private void EndEdit()
		{
			editPoints = false;

			Tools.current = editTool;
		}

		private void DrawEdit()
		{
			var rect = Reserve(); rect.xMin += EditorGUIUtility.labelWidth;

			if (GUI.Button(rect, "Edit Points", EditStyle) == true)
			{
				if (editPoints == true)
				{
					EndEdit();
				}
				else
				{
					BeginEdit();
				}
			}
		}

		private Vector2 GetMouseLocalPoint()
		{
			var ray  = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			var dist = default(float);

			if (new Plane(Target.transform.forward, Target.transform.position).Raycast(ray, out dist) == true)
			{
				return Target.transform.InverseTransformPoint(ray.GetPoint(dist));
			}

			return default(Vector2);
		}

		private int GetClosestPoint(ref float foundDistance)
		{
			var localPoint   = GetMouseLocalPoint();
			var bestIndex    = -1;
			var bestDistance = float.PositiveInfinity;

			for (var i = 0; i < Target.Points.Count; i++)
			{
				var distance = Vector3.Distance(Target.Points[i], localPoint);

				if (distance < bestDistance)
				{
					bestDistance = distance;
					bestIndex    = i;
				}
			}

			foundDistance = bestDistance;

			return bestIndex;
		}

		private void DrawPoint(Vector2 localPoint)
		{
			var screenPoint = Camera.current.WorldToScreenPoint(Target.transform.TransformPoint(localPoint));
			var rect        = new Rect(0.0f, 0.0f, 7.0f, 7.0f); rect.center = new Vector2(screenPoint.x, Screen.height - screenPoint.y - 37.0f);

			GUI.DrawTexture(rect, Texture2D.whiteTexture);
		}

		private static float GetDistance(Vector2 a, Vector2 b, Vector2 p, ref Vector2 closest)
		{
			var ba   = b - a;
			var baba = Vector2.Dot(ba, ba);

			if (baba != 0.0f)
			{
				var d = Vector2.Dot(p - a, ba) / baba;

				if (d >= 0.0f && d <= 1.0f)
				{
					closest = a + ba * d;

					return Vector2.Distance(closest, p);
				}
			}

			return float.PositiveInfinity;
		}

		private int GetClosestEdge(ref float bestDistance, ref Vector2 bestPoint)
		{
			var localPoint = GetMouseLocalPoint();
			var bestIndex  = -1;
			
			bestDistance = float.PositiveInfinity;

			for (var i = 0; i < Target.Points.Count; i++)
			{
				var point    = default(Vector2);
				var distance = GetDistance(Target.Points[i], Target.Points[(i + 1) % Target.Points.Count], localPoint, ref point);

				if (distance < bestDistance)
				{
					bestDistance = distance;
					bestPoint    = point;
					bestIndex    = i;
				}
			}

			return bestIndex;
		}

		private void NotifyModified()
		{
			EditorUtility.SetDirty(Target);

			Target.SetVerticesDirty();
		}

		protected override void DrawScene()
		{
			if (Event.current.type == EventType.MouseUp)
			{
				draggingPoint = -1;
			}

			Handles.matrix = Target.transform.localToWorldMatrix;
			Handles.color  = new Color(0.5f, 1.0f, 0.5f);

			for (var i = 0; i < Target.Points.Count; i++)
			{
				var pointA = (Vector3)Target.Points[i];
				var pointB = (Vector3)Target.Points[(i + 1) % Target.Points.Count];

				Handles.DrawLine(pointA, pointB);
			}

			var control = Event.current.control;
			var down    = Event.current.type == EventType.MouseDown && Event.current.button == 0;

			if (editPoints == true)
			{
				if (Event.current.type == EventType.Layout)
				{
					HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
				}

				SceneView.currentDrawingSceneView.Repaint();

				Handles.BeginGUI();
				{
					var closestDist  = default(float);
					var closestIndex = GetClosestPoint(ref closestDist);

					// Remove
					if (control == true)
					{
						GUI.color = new Color(1.0f, 0.0f, 0.0f);

						if (closestDist < 20.0f)
						{
							DrawPoint(Target.Points[closestIndex]);

							if (Event.current.type == EventType.MouseDown)
							{
								Target.Points.RemoveAt(closestIndex); NotifyModified();
							}
						}
					}
					// Move/split
					else
					{
						GUI.color = new Color(0.0f, 1.0f, 0.0f);

						if (draggingPoint >= 0)
						{
							Target.Points[draggingPoint] = GetMouseLocalPoint(); NotifyModified();

							DrawPoint(Target.Points[draggingPoint]);
						}
						else
						{
							if (closestDist < 20.0f)
							{
								DrawPoint(Target.Points[closestIndex]);

								if (down == true)
								{
									draggingPoint = closestIndex;
								}
							}
							else
							{
								var createDist  = default(float);
								var createPoint = default(Vector2);
								var createIndex = GetClosestEdge(ref createDist, ref createPoint);

								if (createDist < 20.0f)
								{
									GUI.color = new Color(1.0f, 1.0f, 0.0f);

									DrawPoint(createPoint);

									if (down == true)
									{
										Target.Points.Insert(createIndex + 1, createPoint); NotifyModified();

										draggingPoint = createIndex + 1;
									}
								}
							}
						}
					}
				}
				Handles.EndGUI();
			}
		}
	}
}
#endif