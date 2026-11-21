
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BetButtonVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Appearance")]
    [SerializeField] private Image buttonImage;

    [Header("Colours")]
    [SerializeField] private string normalColorHex = "#806A49";
    [SerializeField] private string selectedColorHex = "#F1BB53";
    [SerializeField] private string hoverColorHex = "#FFE09A";

    [Header("Scale")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float hoverScale = 1.13f;

    private bool isSelected;
    private bool isHovered;

    private void Awake()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        ApplyVisuals();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        ApplyVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        ApplyVisuals();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (buttonImage == null)
            return;

        string colorHex;

        if (isHovered)
            colorHex = hoverColorHex;
        else if (isSelected)
            colorHex = selectedColorHex;
        else
            colorHex = normalColorHex;

        if (ColorUtility.TryParseHtmlString(colorHex, out Color buttonColor))
            buttonImage.color = buttonColor;

        float targetScale = normalScale;

        if (isHovered)
            targetScale = hoverScale;
        else if (isSelected)
            targetScale = selectedScale;

        transform.localScale = Vector3.one * targetScale;
    }
}
