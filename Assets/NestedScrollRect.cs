using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] // This makes it show in Inspector
    [Tooltip("Drag the parent ScrollRect here (Scroll Area Main Menu)")]
    public ScrollRect parentScrollRect;

    [SerializeField] // This makes it show in Inspector
    private ScrollRect childScrollRect;
    private bool routeToParent = false;

    void Awake()
    {
        childScrollRect = GetComponent<ScrollRect>();

        // Try to find parent automatically if not set
        if (parentScrollRect == null)
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                parentScrollRect = parent.GetComponent<ScrollRect>();
                if (parentScrollRect != null)
                    break;
                parent = parent.parent;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Determine if drag is more vertical or horizontal
        float horizontal = Mathf.Abs(eventData.delta.x);
        float vertical = Mathf.Abs(eventData.delta.y);

        if (vertical > horizontal)
        {
            // Route to parent (vertical scroll)
            routeToParent = true;
            if (parentScrollRect != null)
                parentScrollRect.OnBeginDrag(eventData);
        }
        else
        {
            // Keep in child (horizontal scroll)
            routeToParent = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (routeToParent && parentScrollRect != null)
        {
            parentScrollRect.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent && parentScrollRect != null)
        {
            parentScrollRect.OnEndDrag(eventData);
        }
        routeToParent = false;
    }
}