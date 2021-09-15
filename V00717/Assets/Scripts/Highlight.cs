using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Highlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Button button;
    ColorBlock cb;
    public void Start()
    {
        button = GetComponent<Button>();
        cb = button.colors;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {        
        cb.normalColor = Color.blue;
        button.colors = cb;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cb.normalColor = Color.black;
        button.colors = cb;
    }
}
