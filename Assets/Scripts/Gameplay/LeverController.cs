
using System.Collections;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SlotMachineController slotMachineController;

    [Header("Animation Settings")]
    [SerializeField] private float pullDuration = 0.12f;
    [SerializeField] private float holdDuration = 0.15f;
    [SerializeField] private float pullAngle = 30f;

    private bool isPulling;
    private Quaternion normalRotation;

    private void Awake()
    {
        normalRotation = transform.localRotation;
    }

    private void OnMouseDown()
    {
        if (isPulling || !slotMachineController.CanStartSpin)
            return;

        StartCoroutine(PullLever());
    }

    private IEnumerator PullLever()
    {
        isPulling = true;

        // Rotate the lever to the pulled position.
        transform.localRotation = normalRotation *
            Quaternion.Euler(0f, 0f, pullAngle);

        yield return new WaitForSeconds(pullDuration);

        // Start the slot machine.
        slotMachineController.SpinMachine();

        // Hold the lever in the pulled position.
        yield return new WaitForSeconds(holdDuration);

        // Return to the original rotation.
        transform.localRotation = normalRotation;

        isPulling = false;
    }
}
