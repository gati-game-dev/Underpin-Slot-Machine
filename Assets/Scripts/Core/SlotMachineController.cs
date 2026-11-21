
using System.Collections;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private ReelController[] reels;

    [Header("Systems")]
    [SerializeField] private SlotResultGenerator resultGenerator;

    private WinEvaluator winEvaluator = new WinEvaluator();

    [ContextMenu("Generate Random Result")]
    public void GenerateRandomResult()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            SymbolType[] reelSymbols =
                resultGenerator.GenerateResult(3);

            reels[i].SetSymbols(reelSymbols);
        }
    }

    [ContextMenu("Start All Reels")]
    public void StartAllReels()
    {
        foreach (ReelController reel in reels)
        {
            reel.StartSpinning();
        }
    }

    [ContextMenu("Stop All Reels")]
    public void StopAllReels()
    {
        foreach (ReelController reel in reels)
        {
            reel.StopSpinning();
        }
    }

    [ContextMenu("Spin Machine")]
    public void SpinMachine()
    {
        SymbolType[][] results = new SymbolType[reels.Length][];

        // Generate the complete outcome before spinning.
        for (int i = 0; i < reels.Length; i++)
        {
            results[i] = resultGenerator.GenerateResult(3);
        }

        StartCoroutine(SpinRoutine(results));
    }

    private IEnumerator SpinRoutine(SymbolType[][] results)
    {
        StartAllReels();

        yield return new WaitForSeconds(2f);

        // Stop each reel sequentially.
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].StopAtResult(results[i]);

            // Wait until this reel has actually stopped.
            while (reels[i].IsSpinning)
            {
                yield return null;
            }

            // Small pause before stopping the next reel.
            yield return new WaitForSeconds(0.2f);
        }

        // Evaluate only after every reel has stopped.
        EvaluateResult();
    }

    private void EvaluateResult()
    {
        SymbolType[] middleSymbols = new SymbolType[reels.Length];

        for (int i = 0; i < reels.Length; i++)
        {
            middleSymbols[i] = reels[i].MiddleSymbol;
        }

        Debug.Log(
            $"EVALUATED MIDDLE ROW: " +
            $"{middleSymbols[0]} | " +
            $"{middleSymbols[1]} | " +
            $"{middleSymbols[2]}"
        );

        bool isWin =
            winEvaluator.IsWinningCombination(middleSymbols);

        if (isWin)
        {
            Debug.Log("JACKPOT! All three middle symbols match!");
        }
        else
        {
            Debug.Log("No win. Better luck next time!");
        }
    }
}
