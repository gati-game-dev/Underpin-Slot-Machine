
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

    // Returns the symbol currently positioned in the middle row of the reel.
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

    // Assigns symbols according to their current vertical order (top to bottom).
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

            // Calculate the distance travelled during this frame while decelerating.
            // This uses the standard distance formula for constant deceleration.
            movementDistance =
                currentSpeed * deltaTime -
                0.5f * stopDeceleration * deltaTime * deltaTime;

            // Prevent the reel from moving farther than its remaining stopping distance.
            movementDistance = Mathf.Clamp(
                movementDistance,
                0f,
                remainingStopDistance
            );

            // Gradually reduce the reel's speed until it reaches zero.
            currentSpeed = Mathf.Max(
                0f,
                currentSpeed - stopDeceleration * deltaTime
            );

            remainingStopDistance -= movementDistance;
        }
        else
        {
            // Move at constant speed while the reel is in its normal spinning phase.
            movementDistance = currentSpeed * Time.deltaTime;
        }

        MoveSymbols(movementDistance);
        RecycleBottomSymbol();
    }

    private void MoveSymbols(float distance)
    {
        // Move every symbol down by the same distance to maintain their spacing.
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

        // Find the lowest and highest symbols based on their current local Y positions.
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

            // Move the lowest symbol above the highest one instead of creating a new object.
            // This allows the reel to scroll continuously using the same three symbols.
            newPosition.y =
                highestSymbol.transform.localPosition.y + symbolSpacing;

            lowestSymbol.transform.localPosition = newPosition;

            if (isStopping)
            {
                // Assign the predetermined stopping symbols as the reel completes
                // its recycling cycles. The order ensures the final vertical layout
                // matches the requested top, middle and bottom symbols.
                int targetIndex =
                    symbols.Length - 1 -
                    (stopRecycleCount % symbols.Length);

                lowestSymbol.SetSymbol(stoppingResult[targetIndex]);

                stopRecycleCount++;

                // Finish the animation after the required number of recycling cycles.
                if (stopRecycleCount >= totalStopRecycles)
                {
                    FinishStop();
                }
            }
        }
    }

    private SymbolView[] GetSymbolsInVerticalOrder()
    {
        // Clone the array so sorting does not change the original symbols array.
        SymbolView[] orderedSymbols = (SymbolView[])symbols.Clone();

        // Sort from highest Y position to lowest Y position (top to bottom).
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

        // Store a copy of the target result so the final symbols remain fixed
        // even if the original array is modified elsewhere.
        stoppingResult = (SymbolType[])targetSymbols.Clone();

        isStopping = true;
        stopRecycleCount = 0;

        // Two complete recycling cycles provide enough movement for a gradual stop.
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

        // Calculate how far the lowest symbol must travel before its first recycle.
        float distanceToFirstRecycle = Mathf.Max(
            0f,
            lowestSymbol.transform.localPosition.y - recycleThreshold
        );

        // Include the remaining recycling cycles to determine the total
        // distance the reel must travel before it can finish stopping.
        remainingStopDistance =
            distanceToFirstRecycle +
            (totalStopRecycles - 1) * symbolSpacing;

        // Calculate the constant deceleration needed to bring the reel
        // from its current speed to zero over the remaining stopping distance.
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

        // Snap the symbols to their exact final positions to prevent
        // small positional inaccuracies from accumulating during movement.
        for (int i = 0; i < orderedSymbols.Length; i++)
        {
            Vector3 position =
                orderedSymbols[i].transform.localPosition;

            position.y = (1 - i) * symbolSpacing;

            orderedSymbols[i].transform.localPosition = position;
        }
    }
}
