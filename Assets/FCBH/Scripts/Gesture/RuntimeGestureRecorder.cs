using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PDollarGestureRecognizer;

namespace FCBH
{
    public class RuntimeGestureRecorder : MonoBehaviour
    {
        public string gestureName = "MyGesture";
        public KeyCode saveKey = KeyCode.S;
        public Transform gestureLinePrefab;

        [Range(0f, 1f)] public float drawAreaX = 0.1f;
        [Range(0f, 1f)] public float drawAreaY = 0.1f;
        [Range(0f, 1f)] public float drawAreaWidth = 0.8f;
        [Range(0f, 1f)] public float drawAreaHeight = 0.8f;

        private List<Point> points = new List<Point>();
        private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
        private LineRenderer currentLineRenderer;
        private int strokeId = -1;
        private int vertexCount = 0;
        private Rect drawArea;

        private const string EDITOR_PRE_TRAINING_GESTURE_PATH = "GestureSets";
        private const string INDEX_FILENAME = "index.txt";

        void Start()
        {
            drawArea = GestureUtility.GetDrawArea(drawAreaX, drawAreaY, drawAreaWidth, drawAreaHeight);
        }

        void Update()
        {
            drawArea = GestureUtility.GetDrawArea(drawAreaX, drawAreaY, drawAreaWidth, drawAreaHeight);

            Vector3 inputPosition = Vector3.zero;
            bool isPressed = false;

#if UNITY_EDITOR || UNITY_STANDALONE
            inputPosition = Input.mousePosition;
            isPressed = Input.GetMouseButton(0);
            if (Input.GetMouseButtonDown(0)) BeginNewStroke();
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                inputPosition = Input.GetTouch(0).position;
                isPressed = true;
                if (Input.GetTouch(0).phase == TouchPhase.Began) BeginNewStroke();
            }
#endif

            if (drawArea.Contains(inputPosition) && isPressed)
            {
                points.Add(new Point(inputPosition.x, -inputPosition.y, strokeId));
                currentLineRenderer.positionCount = ++vertexCount;
                currentLineRenderer.SetPosition(vertexCount - 1,
                    Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 10)));
            }

            if (Input.GetKeyDown(saveKey) && points.Count > 0)
            {
                SaveGesture();
                ClearDrawing();
            }
        }

        private void BeginNewStroke()
        {
            ++strokeId;
            GameObject gestureObj = Instantiate(gestureLinePrefab, Vector3.zero, Quaternion.identity).gameObject;
            currentLineRenderer = gestureObj.GetComponent<LineRenderer>();
            gestureLinesRenderer.Add(currentLineRenderer);
            vertexCount = 0;
        }

        private void SaveGesture()
        {
            string fileName = $"{gestureName}.xml";
            string folderPath = "";

#if UNITY_EDITOR
            folderPath = Path.Combine(Application.streamingAssetsPath, EDITOR_PRE_TRAINING_GESTURE_PATH);
#elif UNITY_ANDROID || UNITY_IOS
            folderPath = Application.persistentDataPath;
#else
            folderPath = Application.dataPath;
#endif

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = Path.Combine(folderPath, fileName);
            GestureIO.WriteGesture(points.ToArray(), gestureName, fullPath);
            Debug.Log($"Gesture saved to: {fullPath}");

            RebuildIndexFile(folderPath);
        }

        private void RebuildIndexFile(string folderPath)
        {
            string indexPath = Path.Combine(folderPath, INDEX_FILENAME);
            var xmlFiles = Directory.GetFiles(folderPath, "*.xml")
                                     .Select(Path.GetFileName)
                                     .Where(name => !string.Equals(name, INDEX_FILENAME, StringComparison.OrdinalIgnoreCase))
                                     .ToList();

            File.WriteAllLines(indexPath, xmlFiles);
            Debug.Log($"Index rebuilt with {xmlFiles.Count} entries: {indexPath}");
        }

        private void ClearDrawing()
        {
            points.Clear();
            foreach (var line in gestureLinesRenderer)
            {
                if (line != null) Destroy(line.gameObject);
            }
            gestureLinesRenderer.Clear();
            strokeId = -1;
            vertexCount = 0;
        }

        void OnGUI()
        {
            GUI.Box(drawArea, "Draw Area");
            GUI.Label(new Rect(10, Screen.height - 30, 300, 30), $"Press [{saveKey}] to save gesture: {gestureName}");
        }
    }
}
