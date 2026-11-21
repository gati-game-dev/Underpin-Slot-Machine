
using UnityEngine;

public class LeverController : MonoBehaviour
{
    [SerializeField] private SlotMachineController slotMachineController;

    private void OnMouseDown()
    {
        slotMachineController.SpinMachine();
    }
}
