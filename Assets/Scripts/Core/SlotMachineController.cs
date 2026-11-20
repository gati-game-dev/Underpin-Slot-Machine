using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private ReelController[] reels;

    [Header("Systems")]
    [SerializeField] private SlotResultGenerator resultGenerator;

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

    [ContextMenu("Generate Random Result")]
    public void GenerateRandomResult()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            SymbolType[] reelSymbols = resultGenerator.GenerateResult(3);
            reels[i].SetSymbols(reelSymbols);
        }
    }
}