
public class PayoutCalculator
{
    // Returns the payout multiplier for a winning symbol.
    private int GetPayoutMultiplier(SymbolType symbol)
    {
        switch (symbol)
        {
            case SymbolType.RedSeven:
                return 10;

            case SymbolType.Cherries:
                return 5;

            case SymbolType.Bell:
                return 3;

            case SymbolType.Bar:
                return 2;

            default:
                return 0;
        }
    }

    // Calculates the total payout for the middle row.
    public int CalculatePayout(SymbolType[] middleSymbols, int betAmount)
    {
        if (middleSymbols == null ||
            middleSymbols.Length != 3 ||
            betAmount <= 0)
        {
            return 0;
        }

        SymbolType firstSymbol = middleSymbols[0];

        // Check whether all three symbols match.
        for (int i = 1; i < middleSymbols.Length; i++)
        {
            if (middleSymbols[i] != firstSymbol)
            {
                return 0;
            }
        }

        int multiplier = GetPayoutMultiplier(firstSymbol);

        return betAmount * multiplier;
    }
}
