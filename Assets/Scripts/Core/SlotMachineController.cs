
using System.Collections;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private ReelController[] reels;

    [Header("Systems")]
    [SerializeField] private SlotResultGenerator resultGenerator;

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
        // Start all reels.
        StartAllReels();

        // Let the reels spin for a while.
        yield return new WaitForSeconds(2f);

        // Stop reels one after another.
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].StopAtResult(results[i]);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
