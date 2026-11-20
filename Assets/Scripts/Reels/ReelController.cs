using UnityEngine;

public class ReelController : MonoBehaviour
{
    [Header("Reel Configuration")]
    [SerializeField] private SymbolView[] symbols;

    public SymbolView[] Symbols => symbols;

    public SymbolType MiddleSymbol => symbols[1].CurrentSymbol;


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
}