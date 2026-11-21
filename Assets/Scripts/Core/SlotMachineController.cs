
using System;
using System.Collections;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private ReelController[] reels;

    [Header("Systems")]
    [SerializeField] private SlotResultGenerator resultGenerator;
    [SerializeField] private BalanceManager balanceManager;

    public event Action OnSpinStarted;
    public event Action<bool, int> OnSpinCompleted;

    private WinEvaluator winEvaluator = new WinEvaluator();
    private PayoutCalculator payoutCalculator = new PayoutCalculator();

    private bool isSpinInProgress;
    public bool IsSpinInProgress => isSpinInProgress;
    public bool CanStartSpin =>
    !isSpinInProgress && balanceManager != null && balanceManager.CanAffordBet;

    [ContextMenu("Generate Random Result")]
    public void GenerateRandomResult()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            SymbolType[] reelSymbols = resultGenerator.GenerateResult(3);
            reels[i].SetSymbols(reelSymbols);
        }
    }

    [ContextMenu("Start All Reels")]
    public void StartAllReels()
    {
        foreach (ReelController reel in reels)
            reel.StartSpinning();
    }

    [ContextMenu("Stop All Reels")]
    public void StopAllReels()
    {
        foreach (ReelController reel in reels)
            reel.StopSpinning();
    }

    [ContextMenu("Spin Machine")]
    public void SpinMachine()
    {
        if (isSpinInProgress)
            return;

        if (!balanceManager.TryPlaceBet())
            return;

        isSpinInProgress = true;

        OnSpinStarted?.Invoke();

        SymbolType[][] results = new SymbolType[reels.Length][];

        for (int i = 0; i < reels.Length; i++)
            results[i] = resultGenerator.GenerateResult(3);

        StartCoroutine(SpinRoutine(results));
    }

    private IEnumerator SpinRoutine(SymbolType[][] results)
    {
        StartAllReels();
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].StopAtResult(results[i]);

            while (reels[i].IsSpinning)
                yield return null;

            yield return new WaitForSeconds(0.2f);
        }

        EvaluateResult();
        isSpinInProgress = false;
    }

    private void EvaluateResult()
    {
        SymbolType[] middleSymbols = new SymbolType[reels.Length];

        for (int i = 0; i < reels.Length; i++)
            middleSymbols[i] = reels[i].MiddleSymbol;

        bool isWin = winEvaluator.IsWinningCombination(middleSymbols);
        int payout = 0;

        if (isWin)
        {
            payout = payoutCalculator.CalculatePayout(
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

        OnSpinCompleted?.Invoke(isWin, payout);
    }
}
