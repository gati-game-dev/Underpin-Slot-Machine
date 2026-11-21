
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
        // Calculate the total weight of all symbols.
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

        // Generate a random number within the total weight.
        int randomNumber = Random.Range(0, totalWeight);

        // Select a symbol based on its weight.
        if (randomNumber < redSevenWeight)
            return SymbolType.RedSeven;

        randomNumber -= redSevenWeight;

        if (randomNumber < cherriesWeight)
            return SymbolType.Cherries;

        randomNumber -= cherriesWeight;

        if (randomNumber < bellWeight)
            return SymbolType.Bell;

        return SymbolType.Bar;
    }

    public SymbolType[] GenerateResult(int reelCount)
    {
        SymbolType[] result = new SymbolType[reelCount];

        for (int i = 0; i < reelCount; i++)
        {
            result[i] = GenerateRandomSymbol();
        }

        return result;
    }
}
