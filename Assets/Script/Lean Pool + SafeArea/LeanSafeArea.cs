using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Common;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Lean.Gui
{
    /// <summary>This component adds a safe area to your UI. This is mainly used to prevent UI elements from going through notches on mobile devices.
    /// This component should be added to a GameObject that is a child of your Canvas root.</summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanSafeArea")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Safe Area")]
    public class LeanSafeArea : UIBehaviour
    {
        /// <summary>Should you be able to drag horizontally?</summary>
        public bool Horizontal
        {
            set { horizontal = value; }
            get { return horizontal; }
        }

        [SerializeField] private bool horizontal = true;

        public Vector2 HorizontalRange
        {
            set { horizontalRange = value; }
            get { return horizontalRange; }
        }

        [Range(0.0f, 1.0f)] [SerializeField] private Vector2 horizontalRange = new Vector2(0.0f, 1.0f);

        /// <summary>Should you be able to drag vertically?</summary>
        public bool Vertical
        {
            set { vertical = value; }
            get { return vertical; }
        }

        [SerializeField] private bool vertical = true;

        public Vector2 VerticalRange
        {
            set { verticalRange = value; }
            get { return verticalRange; }
        }

        [Range(0.0f, 1.0f)] [SerializeField] private Vector2 verticalRange = new Vector2(0.0f, 1.0f);

        [System.NonSerialized] private RectTransform cachedRectTransform;

        [System.NonSerialized] private bool cachedRectTransformSet;

        [SerializeField]
        private bool isResize = false;
        public bool IsResize
        {
            set => isResize = value;
            get => isResize;
        }
        [SerializeField]
        private bool isScale = false;
        public bool IsScale
        {
            set => isScale = value;
            get => isScale;
        }

        void Start()
        {
            UpdateSafeArea();
        }

        /// <summary>This method will instantly update the safe area RectTransform.</summary>
        [ContextMenu("Update Safe Area")]
        public void UpdateSafeArea()
        {
            if (cachedRectTransformSet == false)
            {
                cachedRectTransform = GetComponent<RectTransform>();
                cachedRectTransformSet = true;
            }

            var safeRect = Screen.safeArea;
            var screenW = Screen.width;
            var screenH = Screen.height;
            var safeMin = safeRect.min;
            var safeMax = safeRect.max;

            if (horizontal == false)
            {
                safeMin.x = 0.0f;
                safeMax.x = screenW;
            }
            else
            {
                safeMin.x = Mathf.Max(safeMin.x, horizontalRange.x * screenW);
                safeMax.x = Mathf.Min(safeMax.x, horizontalRange.y * screenW);
            }

            if (vertical == false)
            {
                safeMin.y = 0.0f;
                safeMax.y = screenH;
            }
            else
            {
                safeMin.y = Mathf.Max(safeMin.y, verticalRange.x * screenH);
                safeMax.y = Mathf.Min(safeMax.y, verticalRange.y * screenH);
            }

            var safeX = (safeMax.x - safeMin.x) / screenW;
            var safeY = (safeMax.y - safeMin.y) / screenH;

            if (IsScale)
            {
                cachedRectTransform.localScale = new Vector3(Mathf.Min(safeX, safeY), Mathf.Min(safeX, safeY), 1f);
            }
            else
            {
                cachedRectTransform.localScale = Vector3.one;
            }

            if (IsResize)
            {
                cachedRectTransform.anchorMin = new Vector2(safeMin.x / screenW, safeMin.y / screenH);
                // cachedRectTransform.anchorMax = new Vector2(safeMax.x / screenW, safeMax.y / screenH);
                var anchorSafeX = safeMax.x / screenW;
                
                cachedRectTransform.anchorMax = new Vector2(Mathf.Max(anchorSafeX, 0.99f), safeMax.y / screenH);
                
            }
            else
            {
                cachedRectTransform.anchorMin = Vector2.zero;
                cachedRectTransform.anchorMax = Vector2.one;
            }
        }

        [Conditional("UNITY_EDITOR")]
        protected virtual void Update()
        {
            UpdateSafeArea();
        }
    }
}

#if UNITY_EDITOR
namespace Lean.Gui
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LeanSafeArea))]
    public class LeanSafeArea_Editor : LeanInspector<LeanSafeArea>
    {
        protected override void DrawInspector()
        {
            Draw("isScale", "Should you be able to scale?");
            if (Any(t => t.IsScale))
            {
            }

            Draw("isResize", "Should you be able to resize?");
            if (Any(t => t.IsResize))
            {
            }

            Draw("horizontal", "Should you be able to drag horizontally?");
            if (Any(t => t.Horizontal == true))
            {
                EditorGUI.indentLevel++;
                DrawMinMax("horizontalRange", 0.0f, 1.0f, "", "Range");
                EditorGUI.indentLevel--;
            }

            Draw("vertical", "Should you be able to drag vertically?");
            if (Any(t => t.Vertical == true))
            {
                EditorGUI.indentLevel++;
                DrawMinMax("verticalRange", 0.0f, 1.0f, "", "Range");
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif