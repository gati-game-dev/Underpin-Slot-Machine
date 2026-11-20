
using System.Collections;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    [Header("Reel Configuration")]
    [SerializeField] private SymbolView[] symbols;

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 5f;
    [SerializeField] private float symbolSpacing = 1.2f;
    [SerializeField] private float recycleThreshold = -2f;

    private float currentSpeed;
    private bool isSpinning;

    public SymbolView[] Symbols => symbols;

    public SymbolType MiddleSymbol => symbols[1].CurrentSymbol;

    public void SetMiddleSymbol(SymbolType symbol)
    {
        symbols[1].SetSymbol(symbol);
    }

    public void SetSymbols(SymbolType[] newSymbols)
    {
        if (newSymbols.Length != symbols.Length)
        {
            Debug.LogError("Symbol count does not match reel configuration!");
            return;
        }

        for (int i = 0; i < symbols.Length; i++)
        {
            symbols[i].SetSymbol(newSymbols[i]);
        }
    }

    private void Update()
    {
        if (!isSpinning)
            return;

        foreach (SymbolView symbol in symbols)
        {
            symbol.transform.localPosition +=
                Vector3.down * currentSpeed * Time.deltaTime;
        }

        RecycleBottomSymbol();
    }

    private void RecycleBottomSymbol()
    {
        SymbolView lowestSymbol = symbols[0];
        SymbolView highestSymbol = symbols[0];

        foreach (SymbolView symbol in symbols)
        {
            if (symbol.transform.localPosition.y <
                lowestSymbol.transform.localPosition.y)
            {
                lowestSymbol = symbol;
            }

            if (symbol.transform.localPosition.y >
                highestSymbol.transform.localPosition.y)
            {
                highestSymbol = symbol;
            }
        }

        if (lowestSymbol.transform.localPosition.y <= recycleThreshold)
        {
            Vector3 newPosition = lowestSymbol.transform.localPosition;

            newPosition.y =
                highestSymbol.transform.localPosition.y + symbolSpacing;

            lowestSymbol.transform.localPosition = newPosition;
        }
    }

    [ContextMenu("Start Spinning")]
    public void StartSpinning()
    {
        currentSpeed = spinSpeed;
        isSpinning = true;
    }

    [ContextMenu("Stop Spinning")]
    public void StopSpinning()
    {
        isSpinning = false;
        currentSpeed = 0f;
    }

    public void StopAtResult(SymbolType[] targetSymbols)
    {
        if (targetSymbols.Length != symbols.Length)
        {
            Debug.LogError("Target result does not match reel configuration!");
            return;
        }

        StartCoroutine(StopRoutine(targetSymbols));
    }

    private IEnumerator StopRoutine(SymbolType[] targetSymbols)
    {
        float decelerationDuration = 1.5f;
        float elapsedTime = 0f;

        float startingSpeed = currentSpeed;

        while (elapsedTime < decelerationDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / decelerationDuration
            );

            currentSpeed = Mathf.Lerp(startingSpeed, 0f, progress);

            yield return null;
        }

        // Completely stop the reel.
        currentSpeed = 0f;
        isSpinning = false;

        // Sort symbols from top to bottom.
        System.Array.Sort(symbols, (a, b) =>
            b.transform.localPosition.y.CompareTo(
                a.transform.localPosition.y
            )
        );

        // Align all three symbols.
        for (int i = 0; i < symbols.Length; i++)
        {
            Vector3 position = symbols[i].transform.localPosition;

            position.y = -i * symbolSpacing;

            symbols[i].transform.localPosition = position;
        }

        // Apply the predetermined result.
        SetSymbols(targetSymbols);
    }
}
