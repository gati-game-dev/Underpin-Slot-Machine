
public class WinEvaluator
{
    public bool IsWinningCombination(SymbolType[] middleSymbols)
    {
        if (middleSymbols == null || middleSymbols.Length != 3)
        {
            return false;
        }

        SymbolType firstSymbol = middleSymbols[0];

        for (int i = 1; i < middleSymbols.Length; i++)
        {
            if (middleSymbols[i] != firstSymbol)
            {
                return false;
            }
        }

        return true;
    }
}
