using UnityEngine;

public class MinesweeperTouchHandler : MonoBehaviour
{
    private const float ZoomPower = 2f;
    private const float MovePower = 2.5f;

    [SerializeField] private MinesweeperLayout mvLayout;

    [SerializeField] private RectTransform mvField;

    [HideInInspector] public bool isMultipleFingerMovement;
    
    private readonly Vector2[] _currentFingerPositions = new Vector2[2];
    private float _zoomFactor = 1;
    private float _lastDistance;
    private Vector2 _lastCenter;

    void Update()
    {
        if (Input.touchCount == 0) isMultipleFingerMovement = false;
        if (Input.touchCount <= 1) return;
        isMultipleFingerMovement = true;
        bool moved = false;
        for (int i = 0; i < 2; i++)
        {
            Touch touch = Input.GetTouch(i);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _currentFingerPositions[i] = touch.position;
                    if (i != 1) break;
                    _lastDistance = Vector2.Distance(_currentFingerPositions[0], _currentFingerPositions[1]);
                    _lastCenter = (_currentFingerPositions[0] + _currentFingerPositions[1]) / 2f;
                    moved = false;
                    break;
                case TouchPhase.Moved:
                    moved = true;
                    _currentFingerPositions[i] = touch.position;
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                default:
                    return;
            }
        }

        if (!moved) return;
        Zoom();
        Move();
    }

    void Zoom()
    {
        float currentDistance = Vector2.Distance(_currentFingerPositions[0], _currentFingerPositions[1]);
        if (!(Mathf.Abs(_lastDistance - currentDistance) > 0.001f)) return;
        float distanceDiff = (currentDistance - _lastDistance) /
                             Mathf.Min(mvLayout.screenSafeAreaWidth, mvLayout.screenSafeAreaHeight);
        _zoomFactor += distanceDiff * ZoomPower;
        _zoomFactor = Mathf.Clamp(_zoomFactor, 1, 2);
        mvField.localScale = Vector3.one * _zoomFactor;
        _lastDistance = currentDistance;
    }

    void Move()
    {
        Vector2 currentCenter = (_currentFingerPositions[0] + _currentFingerPositions[1]) / 2f;
        if (_zoomFactor <= 1f) return;
        Vector2 direction = (currentCenter - _lastCenter) * (MovePower / _zoomFactor);
        if (direction.Equals(Vector2.zero)) return;
        Vector2 anchoredPosition = mvField.anchoredPosition;
        anchoredPosition += direction;
        float width = mvLayout.screenSafeAreaHeight > mvLayout.screenSafeAreaWidth ? 1675f : 2300f;
        float height = mvLayout.screenSafeAreaHeight > mvLayout.screenSafeAreaWidth ? 2300f : 1675f;
        float borderX = width * (_zoomFactor - 1f) / 2f;
        float borderY = height * (_zoomFactor - 1f) / 2f;
        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, -borderX, borderX);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, -borderY, borderY);
        mvField.anchoredPosition = anchoredPosition;
        _lastCenter = currentCenter;
    }
}