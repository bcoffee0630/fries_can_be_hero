using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using PDollarGestureRecognizer;

namespace FCBH
{
    public class RuntimeGestureRecognizer : MonoBehaviour
    {
        [Header("Settings")]
        public Transform linePrefab;
        [Range(0f, 1f)] public float drawAreaX = 0.1f;
        [Range(0f, 1f)] public float drawAreaY = 0.1f;
        [Range(0f, 1f)] public float drawAreaWidth = 0.8f;
        [Range(0f, 1f)] public float drawAreaHeight = 0.8f;
        public float autoRecognizeDelay = 1f;

        [Header("Events")]
        public UnityEvent onDrawStart;
        public UnityEvent<string> onGestureRecognized;
        public UnityEvent onDrawEnd;

        private const string EDITOR_PRE_TRAINING_GESTURE_PATH = "GestureSets";
        private List<Point> points = new List<Point>();
        private List<LineRenderer> gestureLines = new List<LineRenderer>();
        private LineRenderer currentLine;
        private int strokeId = -1;
        private int vertexCount = 0;
        private Vector3 inputPosition;
        private bool isDrawing = false;

        private List<Gesture> trainingSet = new List<Gesture>();
        private Coroutine autoRecognizeCoroutine;
        private Rect drawArea;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (Camera.main == null) return;

            UpdateDrawArea();

            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(drawArea.xMin, drawArea.yMin, 10));
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(drawArea.xMax, drawArea.yMax, 10));

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
            drawArea = new Rect(
                Screen.width * drawAreaX,
                Screen.height * drawAreaY,
                Screen.width * drawAreaWidth,
                Screen.height * drawAreaHeight
            );
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            inputPosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0) && drawArea.Contains(inputPosition)) BeginDraw();
            if (Input.GetMouseButton(0) && isDrawing) ContinueDraw();
            if (Input.GetMouseButtonUp(0) && isDrawing) EndDraw();
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                inputPosition = touch.position;
                if (touch.phase == TouchPhase.Began && drawArea.Contains(inputPosition)) BeginDraw();
                if (touch.phase == TouchPhase.Moved && isDrawing) ContinueDraw();
                if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isDrawing) EndDraw();
            }
#endif
        }

        private void BeginDraw()
        {
            isDrawing = true;
            strokeId++;
            vertexCount = 0;
            if (autoRecognizeCoroutine != null) StopCoroutine(autoRecognizeCoroutine);

            GameObject lineObj = Instantiate(linePrefab.gameObject);
            currentLine = lineObj.GetComponent<LineRenderer>();
            gestureLines.Add(currentLine);
            onDrawStart?.Invoke();
        }

        private void ContinueDraw()
        {
            points.Add(new Point(inputPosition.x, -inputPosition.y, strokeId));
            currentLine.positionCount = ++vertexCount;
            currentLine.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 10)));
        }

        private void EndDraw()
        {
            isDrawing = false;
            onDrawEnd?.Invoke();
            autoRecognizeCoroutine = StartCoroutine(AutoRecognizeAfterDelay());
        }

        private IEnumerator AutoRecognizeAfterDelay()
        {
            yield return new WaitForSeconds(autoRecognizeDelay);
            RecognizeGesture();
            ClearDrawing();
        }

        private void RecognizeGesture()
        {
            if (points.Count == 0 || trainingSet.Count == 0) return;
            Gesture candidate = new Gesture(points.ToArray());
            Result result = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
            Debug.Log($"Recognized gesture: {result.GestureClass} ({result.Score})");
            onGestureRecognized?.Invoke(result.GestureClass);
        }

        public void ClearDrawing()
        {
            points.Clear();
            foreach (var line in gestureLines)
            {
                if (line != null) Destroy(line.gameObject);
            }
            gestureLines.Clear();
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
                trainingSet.Add(gesture);
#else
                UnityWebRequest fileReq = UnityWebRequest.Get(gesturePath);
                yield return fileReq.SendWebRequest();

                if (fileReq.result == UnityWebRequest.Result.Success)
                {
                    string xml = fileReq.downloadHandler.text;
                    Gesture gesture = GestureIO.ReadGestureFromXML(xml);
                    trainingSet.Add(gesture);
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
