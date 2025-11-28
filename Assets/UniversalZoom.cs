using UnityEngine;

public class UniversalZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 1f;        // Zoom sensitivity
    public float pinchSpeed = 0.01f;    // Touch sensitivity
    public float minScale = 0.5f;
    public float maxScale = 3f;

    private float _currentScale;

    void Start()
    {
        _currentScale = transform.localScale.x;
    }

    void Update()
    {
        HandleMouseZoom();
        HandleTouchZoom();

        _currentScale = Mathf.Clamp(_currentScale, minScale, maxScale);
        transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
    }

    // ------------------------------
    // PC: Scroll Wheel Zoom
    // ------------------------------
    void HandleMouseZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0f)
        {
            _currentScale += scroll * zoomSpeed * Time.deltaTime * 20f;
        }
    }

    // ------------------------------
    // Mobile: Pinch Zoom
    // ------------------------------
    void HandleTouchZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Previous positions
            Vector2 t0Prev = t0.position - t0.deltaPosition;
            Vector2 t1Prev = t1.position - t1.deltaPosition;

            float prevDistance = Vector2.Distance(t0Prev, t1Prev);
            float currDistance = Vector2.Distance(t0.position, t1.position);

            float diff = currDistance - prevDistance;

            _currentScale += diff * pinchSpeed;
        }
    }
}
