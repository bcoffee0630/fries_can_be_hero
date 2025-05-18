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
        #region Structure

        public enum TargetShape
        {
            Circle,
            Down,
            Left,
            Right,
            Square,
            Thunder,
            Tick,
            Triangle,
            Up,
            X,
        }

        #endregion
        
        [SerializeField] private TargetShape gestureShape = TargetShape.Circle;
        [SerializeField] private KeyCode saveKey = KeyCode.S;
        [SerializeField] private Transform gestureLinePrefab;

        [Range(0f, 1f), SerializeField] private float drawAreaX = 0.1f;
        [Range(0f, 1f), SerializeField] private float drawAreaY = 0.1f;
        [Range(0f, 1f), SerializeField] private float drawAreaWidth = 0.8f;
        [Range(0f, 1f), SerializeField] private float drawAreaHeight = 0.8f;

        private List<Point> _points = new();
        private List<LineRenderer> _gestureLinesRenderer = new();
        private LineRenderer _currentLineRenderer;
        private int _strokeId = -1;
        private int _vertexCount = 0;
        private Rect _drawArea;

        private const string EDITOR_PRE_TRAINING_GESTURE_PATH = "GestureSets";
        private const string INDEX_FILENAME = "index.txt";

        #region Unity methods

        private void Start()
        {
            _drawArea = GestureUtility.GetDrawArea(drawAreaX, drawAreaY, drawAreaWidth, drawAreaHeight);
        }

        private void Update()
        {
            _drawArea = GestureUtility.GetDrawArea(drawAreaX, drawAreaY, drawAreaWidth, drawAreaHeight);

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

            if (_drawArea.Contains(inputPosition) && isPressed)
            {
                _points.Add(new Point(inputPosition.x, -inputPosition.y, _strokeId));
                _currentLineRenderer.positionCount = ++_vertexCount;
                _currentLineRenderer.SetPosition(_vertexCount - 1,
                    Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 10)));
            }

            if (Input.GetKeyDown(saveKey) && _points.Count > 0)
            {
                SaveGesture();
                ClearDrawing();
            }
        }

        private void OnGUI()
        {
            GUI.Box(_drawArea, "Draw Area");
            GUI.Label(new Rect(10, Screen.height - 30, 300, 30), $"Press [{saveKey}] to save gesture: {gestureShape}");
        }

        #endregion

        private void BeginNewStroke()
        {
            ++_strokeId;
            GameObject gestureObj = Instantiate(gestureLinePrefab, Vector3.zero, Quaternion.identity).gameObject;
            _currentLineRenderer = gestureObj.GetComponent<LineRenderer>();
            _gestureLinesRenderer.Add(_currentLineRenderer);
            _vertexCount = 0;
        }

        private void SaveGesture()
        {
            string fileName = $"{gestureShape}.xml";
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
            GestureIO.WriteGesture(_points.ToArray(), gestureShape.ToString(), fullPath);
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
            _points.Clear();
            foreach (var line in _gestureLinesRenderer)
            {
                if (line != null) Destroy(line.gameObject);
            }
            _gestureLinesRenderer.Clear();
            _strokeId = -1;
            _vertexCount = 0;
        }
    }
}
