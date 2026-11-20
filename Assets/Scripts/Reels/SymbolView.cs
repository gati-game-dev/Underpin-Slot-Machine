using UnityEngine;

public class SymbolView : MonoBehaviour
{
    [Header("Symbol Sprites")]
    [SerializeField] private Sprite redSevenSprite;
    [SerializeField] private Sprite cherriesSprite;
    [SerializeField] private Sprite bellSprite;
    [SerializeField] private Sprite barSprite;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    public SymbolType CurrentSymbol { get; private set; }

    public void SetSymbol(SymbolType symbol)
    {
        CurrentSymbol = symbol;

        switch (symbol)
        {
            case SymbolType.RedSeven:
                spriteRenderer.sprite = redSevenSprite;
                break;

            case SymbolType.Cherries:
                spriteRenderer.sprite = cherriesSprite;
                break;

            case SymbolType.Bell:
                spriteRenderer.sprite = bellSprite;
                break;

            case SymbolType.Bar:
                spriteRenderer.sprite = barSprite;
                break;
        }
    }
}