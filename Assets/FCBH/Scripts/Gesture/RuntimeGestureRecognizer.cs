using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PDollarGestureRecognizer;

#if !UNITY_EDITOR
using UnityEngine.Networking;
#endif

namespace FCBH
{
    public class RuntimeGestureRecognizer : MonoBehaviour
    {
        [SerializeField] private bool isActive;
        [SerializeField] private GameConfig config;

        public static event Action OnDrawStart;
        public static event Action<Result> OnGestureRecognized;
        public static event Action OnDrawEnd;

        private bool _isDrawAreaInitialized = false;
        private List<Point> _points = new();
        private List<LineRenderer> _gestureLines = new();
        private LineRenderer _currentLine;
        private int _strokeId = -1;
        private int _vertexCount = 0;
        private Vector3 _inputPosition;
        private bool _isDrawing = false;
        private List<Gesture> _trainingSet = new();
        private Coroutine _autoRecognizeCoroutine;
        private Rect _drawArea;

        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }

        private const string EDITOR_PRE_TRAINING_GESTURE_PATH = "GestureSets";

        #region Unity methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (Camera.main == null) return;

            UpdateDrawArea();

            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(_drawArea.xMin, _drawArea.yMin, 10));
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(_drawArea.xMax, _drawArea.yMax, 10));

            Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);
            Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
#endif

        private void Start()
        {
            UpdateDrawArea();
            StartCoroutine(LoadGesturesFromStreamingAssets());
        }

        private void UpdateDrawArea()
        {
            if (!IsActive)
                return;
            
            if (_isDrawAreaInitialized)
                return;
            _isDrawAreaInitialized = true;
            
            if (config)
            {
                _drawArea = new Rect(
                    Screen.width * config.DrawAreaX,
                    Screen.height * config.DrawAreaY,
                    Screen.width * config.DrawAreaWidth,
                    Screen.height * config.DrawAreaHeight
                );
            }
        }

        private void Update()
        {
            if (!isActive)
                return;
            HandleInput();
        }

        #endregion

        private void HandleInput()
        {
            UpdateDrawArea();
#if UNITY_EDITOR || UNITY_STANDALONE
            _inputPosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0) && _drawArea.Contains(_inputPosition)) BeginDraw();
            if (Input.GetMouseButton(0) && _isDrawing) ContinueDraw();
            if (Input.GetMouseButtonUp(0) && _isDrawing) EndDraw();
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                _inputPosition = touch.position;
                if (touch.phase == TouchPhase.Began && _drawArea.Contains(_inputPosition)) BeginDraw();
                if (touch.phase == TouchPhase.Moved && _isDrawing) ContinueDraw();
                if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && _isDrawing) EndDraw();
            }
#endif
        }

        private void BeginDraw()
        {
            _isDrawing = true;
            _strokeId++;
            _vertexCount = 0;
            if (_autoRecognizeCoroutine != null) StopCoroutine(_autoRecognizeCoroutine);

            GameObject lineObj = Instantiate(config.GesturePrefab.gameObject);
            _currentLine = lineObj.GetComponent<LineRenderer>();
            _gestureLines.Add(_currentLine);
            OnDrawStart?.Invoke();
        }

        private void ContinueDraw()
        {
            _points.Add(new Point(_inputPosition.x, -_inputPosition.y, _strokeId));
            _currentLine.positionCount = ++_vertexCount;
            _currentLine.SetPosition(_vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(_inputPosition.x, _inputPosition.y, 10)));
        }

        private void EndDraw()
        {
            _isDrawing = false;
            OnDrawEnd?.Invoke();
            _autoRecognizeCoroutine = StartCoroutine(AutoRecognizeAfterDelay());
        }

        private IEnumerator AutoRecognizeAfterDelay()
        {
            yield return new WaitForSeconds(config.RecognizeDelay);
            RecognizeGesture();
            ClearDrawing();
        }

        private void RecognizeGesture()
        {
            if (_points.Count == 0 || _trainingSet.Count == 0) return;
            Gesture candidate = new Gesture(_points.ToArray());
            Result result = PointCloudRecognizer.Classify(candidate, _trainingSet.ToArray());
            Debug.Log($"Recognized gesture: {result.GestureClass} ({result.Score})");
            OnGestureRecognized?.Invoke(result);
        }

        public void ClearDrawing()
        {
            _points.Clear();
            foreach (var line in _gestureLines)
            {
                if (line != null) Destroy(line.gameObject);
            }
            _gestureLines.Clear();
        }

        private IEnumerator LoadGesturesFromStreamingAssets()
        {
            string indexPath = Path.Combine(Application.streamingAssetsPath, EDITOR_PRE_TRAINING_GESTURE_PATH, "index.txt");

#if UNITY_EDITOR
            if (!File.Exists(indexPath))
            {
                Debug.LogError("index.txt not found in StreamingAssets.");
                yield break;
            }

            string[] gestureFiles = File.ReadAllLines(indexPath);
#else
            UnityWebRequest indexReq = UnityWebRequest.Get(indexPath);
            yield return indexReq.SendWebRequest();

            if (indexReq.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load index.txt: " + indexReq.error);
                yield break;
            }

            string[] gestureFiles = indexReq.downloadHandler.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
#endif

            foreach (var file in gestureFiles)
            {
                string gesturePath = Path.Combine(Application.streamingAssetsPath, EDITOR_PRE_TRAINING_GESTURE_PATH, file);

#if UNITY_EDITOR
                if (!File.Exists(gesturePath)) continue;
                string xml = File.ReadAllText(gesturePath);
                Gesture gesture = GestureIO.ReadGestureFromXML(xml);
                _trainingSet.Add(gesture);
#else
                UnityWebRequest fileReq = UnityWebRequest.Get(gesturePath);
                yield return fileReq.SendWebRequest();

                if (fileReq.result == UnityWebRequest.Result.Success)
                {
                    string xml = fileReq.downloadHandler.text;
                    Gesture gesture = GestureIO.ReadGestureFromXML(xml);
                    _trainingSet.Add(gesture);
                }
                else
                {
                    Debug.LogWarning($"Failed to load gesture: {file}, Error: {fileReq.error}");
                }
#endif
            }
        }
    }
}
