
using UnityEngine;

public class SlotResultGenerator : MonoBehaviour
{
    [Header("Symbol Probability Weights")]
    [SerializeField] private int redSevenWeight = 10;
    [SerializeField] private int cherriesWeight = 20;
    [SerializeField] private int bellWeight = 30;
    [SerializeField] private int barWeight = 40;

    public SymbolType GenerateRandomSymbol()
    {
        // Calculate the combined weight of all available symbols.
        // The individual weights determine each symbol's relative probability.
        int totalWeight =
            redSevenWeight +
            cherriesWeight +
            bellWeight +
            barWeight;

        if (totalWeight <= 0)
        {
            Debug.LogError("Total symbol probability weight must be greater than zero!");
            return SymbolType.Bar;
        }

        // Generate an integer within the total weight range.
        // Random.Range uses an exclusive upper bound for integer values.
        int randomNumber = Random.Range(0, totalWeight);

        // Check which symbol's probability interval contains the random number.
        // Each interval has a size equal to that symbol's assigned weight.
        if (randomNumber < redSevenWeight)
            return SymbolType.RedSeven;

        // Shift the random number past the Red Seven interval
        // to check the next symbol's probability interval.
        randomNumber -= redSevenWeight;

        if (randomNumber < cherriesWeight)
            return SymbolType.Cherries;

        // Shift past the Cherries interval before checking the Bell interval.
        randomNumber -= cherriesWeight;

        if (randomNumber < bellWeight)
            return SymbolType.Bell;

        // Any remaining random number falls within the BAR interval.
        return SymbolType.Bar;
    }

    public SymbolType[] GenerateResult(int reelCount)
    {
        SymbolType[] result = new SymbolType[reelCount];

        // Generate each reel's symbols independently using the weighted RNG.
        for (int i = 0; i < reelCount; i++)
        {
            result[i] = GenerateRandomSymbol();
        }

        return result;
    }
}
