
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineUI : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private BalanceManager balanceManager;
    [SerializeField] private SlotMachineController slotMachineController;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Bet Buttons")]
    [SerializeField] private Button bet10Button;
    [SerializeField] private Button bet50Button;
    [SerializeField] private Button bet100Button;

    private int previousBalance = -1;
    private int previousBet = -1;

    private void OnEnable()
    {
        if (slotMachineController != null)
            slotMachineController.OnSpinCompleted += ShowResult;
    }

    private void OnDisable()
    {
        if (slotMachineController != null)
            slotMachineController.OnSpinCompleted -= ShowResult;
    }

    private void Start()
    {
        RefreshUI();
        resultText.text = "GOOD LUCK!";
    }

    private void Update()
    {
        if (previousBalance != balanceManager.CurrentBalance ||
            previousBet != balanceManager.CurrentBet)
        {
            RefreshUI();
        }
    }

    public void SelectBet10()
    {
        SelectBet(10);
    }

    public void SelectBet50()
    {
        SelectBet(50);
    }

    public void SelectBet100()
    {
        SelectBet(100);
    }

    private void SelectBet(int amount)
    {
        if (balanceManager.SetBet(amount))
            RefreshUI();
    }

    private void RefreshUI()
    {
        int balance = balanceManager.CurrentBalance;
        int bet = balanceManager.CurrentBet;

        balanceText.text = "BALANCE: " + balance;

        bet10Button.interactable = balance >= 10;
        bet50Button.interactable = balance >= 50;
        bet100Button.interactable = balance >= 100;

        previousBalance = balance;
        previousBet = bet;
    }

    private void ShowResult(bool isWin, int payout)
    {
        if (isWin)
        {
            resultText.text = "YOU WON! +" + payout + " COINS";
        }
        else
        {
            resultText.text = "TRY AGAIN!";
            resultText.color = Color.tomato;
        }
    }
}
