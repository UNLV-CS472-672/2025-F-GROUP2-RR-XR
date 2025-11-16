using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectScalling : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isDragging;
    private float _currentScale;
    public float minScale = 0.5f, maxScale = 3f;

    private float _temp;
    private float _scalingRate = 2f;

    private void Start()
    {
        _currentScale = transform.localScale.x;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }

    private void Update()
    {
        // ================================================
        // PC TESTING — SCROLL WHEEL ZOOM
        // ================================================
        if (_isDragging && Input.mouseScrollDelta.y != 0)
        {
            _currentScale += Input.mouseScrollDelta.y * Time.deltaTime * _scalingRate * 20f;
            _currentScale = Mathf.Clamp(_currentScale, minScale, maxScale);
            transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
        }

        // ================================================
        // MOBILE — PINCH ZOOM
        // ================================================
        if (_isDragging && Input.touchCount == 2)
        {
            transform.localScale = new Vector2(_currentScale, _currentScale);

            float distance = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

            if (_temp > distance) // fingers moving inward (zoom out)
            {
                _currentScale -= Time.deltaTime * _scalingRate;
            }
            else if (_temp < distance) // fingers moving outward (zoom in)
            {
                _currentScale += Time.deltaTime * _scalingRate;
            }

            _currentScale = Mathf.Clamp(_currentScale, minScale, maxScale);
            transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);

            _temp = distance;
        }
    }
}
