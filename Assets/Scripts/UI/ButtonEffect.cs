using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color textColor;
    [SerializeField] private TextMeshProUGUI text;
    private Color defaultTextColor;
    private Image buttonImage;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        if(text != null)
            defaultTextColor = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = 1f; 
            buttonImage.color = color;
            if(text != null)
                text.color = textColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = 0f; 
            buttonImage.color = color;
            if(text != null)
                text.color = defaultTextColor;
        }
    }

    public void ResetColor()
    {
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = 0f; 
            buttonImage.color = color;
        }
    }
}
