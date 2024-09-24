using UnityEngine;
using UnityEditor;

namespace EditorTools
{
    [InitializeOnLoad]
    public class CameraBasedGridEditor : EditorWindow
    {
        private static Camera targetCamera;
        private static bool showGrid = true;
        private static Color gridColor = new Color(1f, 1f, 1f, 0.5f);
        private static bool snapToGrid = false;
        private static int gridExtent = 10;

        private const string PrefsKeyPrefix = "CameraGrid_";

        static CameraBasedGridEditor()
        {
            LoadPrefs();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        [MenuItem("Tools/Camera Based Scene Grid")]
        public static void ShowWindow()
        {
            GetWindow<CameraBasedGridEditor>("Camera Grid");
        }

        void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            targetCamera = (Camera)EditorGUILayout.ObjectField("Target Camera", targetCamera, typeof(Camera), true);
            showGrid = EditorGUILayout.Toggle("Show Grid", showGrid);
            gridColor = EditorGUILayout.ColorField("Grid Color", gridColor);
            snapToGrid = EditorGUILayout.Toggle("Snap to Grid", snapToGrid);
            gridExtent = EditorGUILayout.IntSlider("Grid Extent", gridExtent, 1, 50);

            if (EditorGUI.EndChangeCheck())
            {
                SavePrefs();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Refresh Grid"))
            {
                SceneView.RepaintAll();
            }

            if (targetCamera == null)
            {
                EditorGUILayout.HelpBox("Please assign a camera to generate the grid.", MessageType.Warning);
            }
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            if (!showGrid || targetCamera == null) return;

            Vector3 cameraPosition = targetCamera.transform.position;
            float orthographicSize = targetCamera.orthographicSize;
            float aspectRatio = targetCamera.aspect;

            float gridWidth = orthographicSize * 2 * aspectRatio;
            float gridHeight = orthographicSize * 2;

            Handles.color = gridColor;

            // 计算网格的起始和结束位置，确保网格中心与摄像机位置对齐
            float startX = Mathf.Floor(cameraPosition.x / gridWidth) * gridWidth - gridWidth * gridExtent;
            float startY = Mathf.Floor(cameraPosition.y / gridHeight) * gridHeight - gridHeight * gridExtent;
            float endX = startX + gridWidth * (gridExtent * 2 + 1);
            float endY = startY + gridHeight * (gridExtent * 2 + 1);

            // 绘制垂直线
            for (float x = startX; x <= endX; x += gridWidth)
            {
                Handles.DrawLine(new Vector3(x, startY, 0), new Vector3(x, endY, 0));
            }

            // 绘制水平线
            for (float y = startY; y <= endY; y += gridHeight)
            {
                Handles.DrawLine(new Vector3(startX, y, 0), new Vector3(endX, y, 0));
            }

            // 绘制摄像机视野范围
            Handles.color = Color.yellow;
            Vector3 topLeft = cameraPosition + new Vector3(-gridWidth/2, gridHeight/2, 0);
            Vector3 topRight = cameraPosition + new Vector3(gridWidth/2, gridHeight/2, 0);
            Vector3 bottomLeft = cameraPosition + new Vector3(-gridWidth/2, -gridHeight/2, 0);
            Vector3 bottomRight = cameraPosition + new Vector3(gridWidth/2, -gridHeight/2, 0);
            Handles.DrawLine(topLeft, topRight);
            Handles.DrawLine(topRight, bottomRight);
            Handles.DrawLine(bottomRight, bottomLeft);
            Handles.DrawLine(bottomLeft, topLeft);

            if (snapToGrid)
            {
                SnapSelectedObjectsToGrid(gridWidth, gridHeight, new Vector3(startX, startY, 0));
            }
        }

        static void SnapSelectedObjectsToGrid(float gridWidth, float gridHeight, Vector3 gridOrigin)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Vector3 position = obj.transform.position;
                position.x = Mathf.Round((position.x - gridOrigin.x) / gridWidth) * gridWidth + gridOrigin.x;
                position.y = Mathf.Round((position.y - gridOrigin.y) / gridHeight) * gridHeight + gridOrigin.y;
                obj.transform.position = position;
            }
        }

        static void SavePrefs()
        {
            EditorPrefs.SetString(PrefsKeyPrefix + "TargetCamera", targetCamera ? targetCamera.name : "");
            EditorPrefs.SetBool(PrefsKeyPrefix + "ShowGrid", showGrid);
            EditorPrefs.SetFloat(PrefsKeyPrefix + "GridColorR", gridColor.r);
            EditorPrefs.SetFloat(PrefsKeyPrefix + "GridColorG", gridColor.g);
            EditorPrefs.SetFloat(PrefsKeyPrefix + "GridColorB", gridColor.b);
            EditorPrefs.SetFloat(PrefsKeyPrefix + "GridColorA", gridColor.a);
            EditorPrefs.SetBool(PrefsKeyPrefix + "SnapToGrid", snapToGrid);
            EditorPrefs.SetInt(PrefsKeyPrefix + "GridExtent", gridExtent);
        }

        static void LoadPrefs()
        {
            string cameraName = EditorPrefs.GetString(PrefsKeyPrefix + "TargetCamera", "");
            if (!string.IsNullOrEmpty(cameraName))
            {
                targetCamera = GameObject.Find(cameraName)?.GetComponent<Camera>();
            }
            showGrid = EditorPrefs.GetBool(PrefsKeyPrefix + "ShowGrid", true);
            gridColor = new Color(
                EditorPrefs.GetFloat(PrefsKeyPrefix + "GridColorR", 1f),
                EditorPrefs.GetFloat(PrefsKeyPrefix + "GridColorG", 1f),
                EditorPrefs.GetFloat(PrefsKeyPrefix + "GridColorB", 1f),
                EditorPrefs.GetFloat(PrefsKeyPrefix + "GridColorA", 0.5f)
            );
            snapToGrid = EditorPrefs.GetBool(PrefsKeyPrefix + "SnapToGrid", false);
            gridExtent = EditorPrefs.GetInt(PrefsKeyPrefix + "GridExtent", 10);
        }
    }
}