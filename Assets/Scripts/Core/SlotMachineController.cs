
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

    // Events allow the UI to respond when a spin starts or finishes
    // without making the controller directly responsible for UI updates.
    public event Action OnSpinStarted;
    public event Action<bool, int> OnSpinCompleted;

    private WinEvaluator winEvaluator = new WinEvaluator();
    private PayoutCalculator payoutCalculator = new PayoutCalculator();

    private bool isSpinInProgress;

    public bool IsSpinInProgress => isSpinInProgress;

    // A spin can only begin if another spin is not running
    // and the player has enough balance for the selected bet.
    public bool CanStartSpin =>
    !isSpinInProgress && balanceManager != null && balanceManager.CanAffordBet;

    [ContextMenu("Generate Random Result")]
    public void GenerateRandomResult()
    {
        // Generate and assign a random set of symbols to each reel.
        for (int i = 0; i < reels.Length; i++)
        {
            SymbolType[] reelSymbols = resultGenerator.GenerateResult(3);
            reels[i].SetSymbols(reelSymbols);
        }
    }

    [ContextMenu("Start All Reels")]
    public void StartAllReels()
    {
        // Start every reel together.
        foreach (ReelController reel in reels)
            reel.StartSpinning();
    }

    [ContextMenu("Stop All Reels")]
    public void StopAllReels()
    {
        // Immediately stop every reel. This method is also useful for testing.
        foreach (ReelController reel in reels)
            reel.StopSpinning();
    }

    [ContextMenu("Spin Machine")]
    public void SpinMachine()
    {
        // Prevent multiple spins from running simultaneously.
        if (isSpinInProgress)
            return;

        // Deduct the selected bet before starting the spin.
        // If the player cannot afford it, the spin does not proceed.
        if (!balanceManager.TryPlaceBet())
            return;

        isSpinInProgress = true;

        // Notify subscribed systems, such as the UI, that spinning has started.
        OnSpinStarted?.Invoke();

        // Generate the final result for every reel before the animation begins.
        // This ensures the outcome is determined independently of reel movement.
        SymbolType[][] results = new SymbolType[reels.Length][];

        for (int i = 0; i < reels.Length; i++)
            results[i] = resultGenerator.GenerateResult(3);

        StartCoroutine(SpinRoutine(results));
    }

    private IEnumerator SpinRoutine(SymbolType[][] results)
    {
        // Start all reels together and allow them to spin for two seconds.
        StartAllReels();
        yield return new WaitForSeconds(2f);

        // Stop reels sequentially to create a staggered stopping effect.
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].StopAtResult(results[i]);

            // Wait until the current reel has completely stopped
            // before moving on to the next reel.
            while (reels[i].IsSpinning)
                yield return null;

            // Add a short delay between consecutive reel stops.
            yield return new WaitForSeconds(0.2f);
        }

        // Evaluate the final symbols only after every reel has stopped.
        EvaluateResult();

        isSpinInProgress = false;
    }

    private void EvaluateResult()
    {
        // Collect the middle-row symbol from each reel.
        // These three symbols form the active horizontal payline.
        SymbolType[] middleSymbols = new SymbolType[reels.Length];

        for (int i = 0; i < reels.Length; i++)
            middleSymbols[i] = reels[i].MiddleSymbol;

        // Check whether all three middle-row symbols match.
        bool isWin = winEvaluator.IsWinningCombination(middleSymbols);
        int payout = 0;

        if (isWin)
        {
            // Calculate winnings using the matched symbol's multiplier
            // and the bet amount used for this spin.
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

        // Notify subscribed systems that the spin has finished,
        // passing the win status and payout amount.
        OnSpinCompleted?.Invoke(isWin, payout);
    }
}
