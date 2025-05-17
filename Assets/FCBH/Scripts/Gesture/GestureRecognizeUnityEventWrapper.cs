using PDollarGestureRecognizer;
using UnityEngine;
using UnityEngine.Events;

namespace FCBH
{
    public class GestureRecognizeUnityEventWrapper : MonoBehaviour
    {
        [SerializeField] private UnityEvent onDrawStart;
        [SerializeField] private UnityEvent<Result> onGestureRecognized;
        [SerializeField] private UnityEvent onDrawEnd;

        #region Unity methods

        private void OnEnable()
        {
            RuntimeGestureRecognizer.OnDrawStart += onDrawStart.Invoke;
            RuntimeGestureRecognizer.OnGestureRecognized += onGestureRecognized.Invoke;
            RuntimeGestureRecognizer.OnDrawEnd += onDrawEnd.Invoke;
        }

        private void OnDisable()
        {
            RuntimeGestureRecognizer.OnDrawStart -= onDrawStart.Invoke;
            RuntimeGestureRecognizer.OnGestureRecognized -= onGestureRecognized.Invoke;
            RuntimeGestureRecognizer.OnDrawEnd -= onDrawEnd.Invoke;
        }

        #endregion
    }
}
