using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool dragOnSurfaces = true;

    /// <summary>
    /// The action index used to identify which action data to give to the Action Belt handler.
    /// </summary>
    public int thisActionIndex = 0;

    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;
    ActionBelt actionBelt;

    public CanvasGroup canvasGroup;

    public delegate void OnDragActionBelt();

    private void Awake()
    {
        if (actionBelt == null)
        {
            actionBelt = FindObjectOfType<ActionBelt>();
        }
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        m_DraggingIcon = new GameObject("icon");

        m_DraggingIcon.transform.SetParent(canvasGroup.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();

        image.sprite = GetComponent<Image>().sprite;
        image.SetNativeSize();

        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;

        SetDraggedPosition(eventData);
        // Notify action belt
        actionBelt.ActionIndex = thisActionIndex;
    }

    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
        {
            SetDraggedPosition(data);
        }
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_DraggingIcon != null)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
            Destroy(m_DraggingIcon);
        }
    }
}
