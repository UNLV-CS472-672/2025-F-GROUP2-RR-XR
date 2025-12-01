using UnityEngine;
using UnityEngine.EventSystems;

public class HideOnClickOutside : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        // Detect left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerInsideUI())
            {
                gameObject.SetActive(false);
            }
        }
    }

    bool IsPointerInsideUI()
    {
        Vector2 mousePos = Input.mousePosition;

        // Convert the screen point to UI space for this rect
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos, canvas.worldCamera);
    }
}
