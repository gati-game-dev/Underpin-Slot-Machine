
using UnityEngine;

public class ReelController : MonoBehaviour
{
    [Header("Reel Configuration")]
    [SerializeField] private SymbolView[] symbols;

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 5f;
    [SerializeField] private float symbolSpacing = 0.48f;
    [SerializeField] private float recycleThreshold = -0.72f;

    private float currentSpeed;
    private bool isSpinning;
    private bool isStopping;

    private SymbolType[] stoppingResult;
    private int stopRecycleCount;
    private int totalStopRecycles;

    private float stopDeceleration;
    private float remainingStopDistance;

    public SymbolView[] Symbols => symbols;
    public bool IsSpinning => isSpinning;

    public SymbolType MiddleSymbol
    {
        get
        {
            SymbolView[] orderedSymbols = GetSymbolsInVerticalOrder();
            return orderedSymbols[1].CurrentSymbol;
        }
    }

    public void SetMiddleSymbol(SymbolType symbol)
    {
        SymbolView[] orderedSymbols = GetSymbolsInVerticalOrder();
        orderedSymbols[1].SetSymbol(symbol);
    }

    public void SetSymbols(SymbolType[] newSymbols)
    {
        if (newSymbols == null || newSymbols.Length != symbols.Length)
        {
            Debug.LogError("Symbol count does not match reel configuration!");
            return;
        }

        SymbolView[] orderedSymbols = GetSymbolsInVerticalOrder();

        for (int i = 0; i < orderedSymbols.Length; i++)
        {
            orderedSymbols[i].SetSymbol(newSymbols[i]);
        }
    }

    private void Update()
    {
        if (!isSpinning)
            return;

        float movementDistance;

        if (isStopping)
        {
            float deltaTime = Time.deltaTime;

            movementDistance =
                currentSpeed * deltaTime -
                0.5f * stopDeceleration * deltaTime * deltaTime;

            movementDistance = Mathf.Clamp(
                movementDistance,
                0f,
                remainingStopDistance
            );

            currentSpeed = Mathf.Max(
                0f,
                currentSpeed - stopDeceleration * deltaTime
            );

            remainingStopDistance -= movementDistance;
        }
        else
        {
            movementDistance = currentSpeed * Time.deltaTime;
        }

        MoveSymbols(movementDistance);
        RecycleBottomSymbol();
    }

    private void MoveSymbols(float distance)
    {
        foreach (SymbolView symbol in symbols)
        {
            symbol.transform.localPosition +=
                Vector3.down * distance;
        }
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

        if (lowestSymbol.transform.localPosition.y <=
            recycleThreshold + 0.0001f)
        {
            Vector3 newPosition = lowestSymbol.transform.localPosition;

            newPosition.y =
                highestSymbol.transform.localPosition.y + symbolSpacing;

            lowestSymbol.transform.localPosition = newPosition;

            if (isStopping)
            {
                int targetIndex =
                    symbols.Length - 1 -
                    (stopRecycleCount % symbols.Length);

                lowestSymbol.SetSymbol(stoppingResult[targetIndex]);

                stopRecycleCount++;

                if (stopRecycleCount >= totalStopRecycles)
                {
                    FinishStop();
                }
            }
        }
    }

    private SymbolView[] GetSymbolsInVerticalOrder()
    {
        SymbolView[] orderedSymbols = (SymbolView[])symbols.Clone();

        System.Array.Sort(
            orderedSymbols,
            (a, b) =>
                b.transform.localPosition.y.CompareTo(
                    a.transform.localPosition.y
                )
        );

        return orderedSymbols;
    }

    [ContextMenu("Start Spinning")]
    public void StartSpinning()
    {
        currentSpeed = spinSpeed;
        isSpinning = true;
        isStopping = false;
        stopRecycleCount = 0;
    }

    [ContextMenu("Stop Spinning")]
    public void StopSpinning()
    {
        isSpinning = false;
        isStopping = false;
        currentSpeed = 0f;
    }

    public void StopAtResult(SymbolType[] targetSymbols)
    {
        if (targetSymbols == null ||
            targetSymbols.Length != symbols.Length)
        {
            Debug.LogError(
                "Target result does not match reel configuration!"
            );
            return;
        }

        if (!isSpinning || isStopping)
            return;

        stoppingResult = (SymbolType[])targetSymbols.Clone();

        isStopping = true;
        stopRecycleCount = 0;
        totalStopRecycles = symbols.Length * 2;

        SymbolView lowestSymbol = symbols[0];

        foreach (SymbolView symbol in symbols)
        {
            if (symbol.transform.localPosition.y <
                lowestSymbol.transform.localPosition.y)
            {
                lowestSymbol = symbol;
            }
        }

        float distanceToFirstRecycle = Mathf.Max(
            0f,
            lowestSymbol.transform.localPosition.y - recycleThreshold
        );

        remainingStopDistance =
            distanceToFirstRecycle +
            (totalStopRecycles - 1) * symbolSpacing;

        stopDeceleration =
            (currentSpeed * currentSpeed) /
            (2f * remainingStopDistance);
    }

    private void FinishStop()
    {
        isSpinning = false;
        isStopping = false;
        currentSpeed = 0f;

        SymbolView[] orderedSymbols = GetSymbolsInVerticalOrder();

        for (int i = 0; i < orderedSymbols.Length; i++)
        {
            Vector3 position =
                orderedSymbols[i].transform.localPosition;

            position.y = (1 - i) * symbolSpacing;

            orderedSymbols[i].transform.localPosition = position;
        }
    }
}
