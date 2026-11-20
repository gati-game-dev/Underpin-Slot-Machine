using UnityEngine;

public class SlotResultGenerator : MonoBehaviour
{
    public SymbolType GenerateRandomSymbol()
    {
        int randomIndex = Random.Range(0, 4);

        return (SymbolType)randomIndex;
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