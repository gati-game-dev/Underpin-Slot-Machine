
using System.Collections;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private ReelController[] reels;

    [Header("Systems")]
    [SerializeField] private SlotResultGenerator resultGenerator;
    [SerializeField] private BalanceManager balanceManager;

    private WinEvaluator winEvaluator = new WinEvaluator();
    private PayoutCalculator payoutCalculator = new PayoutCalculator();

    private bool isSpinInProgress;

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
        // Prevent multiple simultaneous spins.
        if (isSpinInProgress)
            return;

        // Deduct the bet before starting.
        if (!balanceManager.TryPlaceBet())
            return;

        isSpinInProgress = true;

        // Generate the complete outcome before spinning.
        SymbolType[][] results = new SymbolType[reels.Length][];

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

            // Wait until the current reel has actually stopped.
            while (reels[i].IsSpinning)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }

        EvaluateResult();

        isSpinInProgress = false;
    }

    private void EvaluateResult()
    {
        SymbolType[] middleSymbols = new SymbolType[reels.Length];

        for (int i = 0; i < reels.Length; i++)
        {
            middleSymbols[i] = reels[i].MiddleSymbol;
        }

        Debug.Log(
            $"MIDDLE ROW: " +
            $"{middleSymbols[0]} | " +
            $"{middleSymbols[1]} | " +
            $"{middleSymbols[2]}"
        );

        bool isWin =
            winEvaluator.IsWinningCombination(middleSymbols);

        if (isWin)
        {
            int payout = payoutCalculator.CalculatePayout(
                middleSymbols,
                balanceManager.CurrentBet
            );

            balanceManager.AddWinnings(payout);

            Debug.Log($"JACKPOT! You won {payout} coins!");
        }
        else
        {
            Debug.Log("No win. Better luck next time!");
        }
    }
}
