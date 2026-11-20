using UnityEngine;

public class ReelController : MonoBehaviour
{
    [Header("Reel Configuration")]
    [SerializeField] private SymbolView[] symbols;

    public SymbolView[] Symbols => symbols;

    public SymbolType MiddleSymbol => symbols[1].CurrentSymbol;

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 5f;
    [SerializeField] private float symbolSpacing = 1.2f;
    [SerializeField] private float recycleThreshold = -2f;

    private bool isSpinning;

    [ContextMenu("Test Symbol Assignment")]
    private void TestSymbolAssignment()
    {
        symbols[0].SetSymbol(SymbolType.RedSeven);
        symbols[1].SetSymbol(SymbolType.Cherries);
        symbols[2].SetSymbol(SymbolType.Bell);
    }
    public void SetMiddleSymbol(SymbolType symbol)
    {
        symbols[1].SetSymbol(symbol);
    }

    public void SetSymbols(SymbolType[] newSymbols)
    {
        if (newSymbols.Length != symbols.Length)
        {
            Debug.LogError("Symbol count does not match reel configuration!");
            return;
        }

        for (int i = 0; i < symbols.Length; i++)
        {
            symbols[i].SetSymbol(newSymbols[i]);
        }
    }

    private void Update()
    {
        if (!isSpinning)
            return;

        foreach (SymbolView symbol in symbols)
        {
            symbol.transform.localPosition +=
                Vector3.down * spinSpeed * Time.deltaTime;
        }

        RecycleBottomSymbol();
    }

    private void RecycleBottomSymbol()
    {
        SymbolView lowestSymbol = symbols[0];
        SymbolView highestSymbol = symbols[0];

        foreach (SymbolView symbol in symbols)
        {
            if (symbol.transform.localPosition.y <
                lowestSymbol.transform.localPosition.y)
            {
                lowestSymbol = symbol;
            }

            if (symbol.transform.localPosition.y >
                highestSymbol.transform.localPosition.y)
            {
                highestSymbol = symbol;
            }
        }

        if (lowestSymbol.transform.localPosition.y <= recycleThreshold)
        {
            Vector3 newPosition = lowestSymbol.transform.localPosition;

            newPosition.y =
                highestSymbol.transform.localPosition.y + symbolSpacing;

            lowestSymbol.transform.localPosition = newPosition;
        }
    }

    [ContextMenu("Start Spinning")]
    public void StartSpinning()
    {
        isSpinning = true;
    }

    [ContextMenu("Stop Spinning")]
    public void StopSpinning()
    {
        isSpinning = false;
    }
}