
public class WinEvaluator
{
    public bool IsWinningCombination(SymbolType[] middleSymbols)
    {
        // A valid result must contain exactly three symbols,
        // representing the middle symbol from each of the three reels.
        if (middleSymbols == null || middleSymbols.Length != 3)
        {
            return false;
        }

        // Use the first reel's middle symbol as the reference for comparison.
        SymbolType firstSymbol = middleSymbols[0];

        // Compare the remaining two symbols against the first symbol.
        // If any symbol differs, the combination is not a win.
        for (int i = 1; i < middleSymbols.Length; i++)
        {
            if (middleSymbols[i] != firstSymbol)
            {
                return false;
            }
        }

        // All three middle-row symbols match, so the combination is a win.
        return true;
    }
}
