
using UnityEngine;

public class BalanceManager : MonoBehaviour
{
    [Header("Balance Settings")]
    [SerializeField] private int startingBalance = 1000;

    [Header("Bet Settings")]
    [SerializeField] private int[] availableBets = { 10, 50, 100 };

    private int currentBalance;
    private int currentBet;

    public int CurrentBalance => currentBalance;
    public int CurrentBet => currentBet;

    public bool CanAffordBet => currentBalance >= currentBet;

    private void Awake()
    {
        currentBalance = startingBalance;
        currentBet = availableBets[0];
    }

    // Select a bet amount.
    public bool SetBet(int betAmount)
    {
        bool isValidBet = false;

        foreach (int bet in availableBets)
        {
            if (bet == betAmount)
            {
                isValidBet = true;
                break;
            }
        }

        if (!isValidBet)
        {
            Debug.LogWarning("Invalid bet amount!");
            return false;
        }

        if (betAmount > currentBalance)
        {
            Debug.LogWarning("Not enough coins for this bet!");
            return false;
        }

        currentBet = betAmount;

        Debug.Log($"Bet selected: {currentBet}");

        return true;
    }

    // Deduct the selected bet before spinning.
    public bool TryPlaceBet()
    {
        if (!CanAffordBet)
        {
            Debug.LogWarning("Insufficient balance!");
            return false;
        }

        currentBalance -= currentBet;

        Debug.Log($"Bet placed: {currentBet}. Remaining balance: {currentBalance}");

        return true;
    }

    // Add winnings to the player's balance.
    public void AddWinnings(int amount)
    {
        if (amount <= 0)
            return;

        currentBalance += amount;

        Debug.Log($"Won {amount} coins! New balance: {currentBalance}");
    }
}