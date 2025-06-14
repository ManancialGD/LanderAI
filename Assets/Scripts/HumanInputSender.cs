using UnityEngine;
using UnityEngine.InputSystem;
using VideojogosLusofona.LusoLander;

public class HumanInputSender : MonoBehaviour, IInputSender
{
    [Header("Inputs")]
    [SerializeField]
    private InputActionReference thrustAction;

    [SerializeField]
    private InputActionReference rotateAction;

    private void Awake()
    {
        if (thrustAction == null || rotateAction == null)
        {
            Debug.LogError("InputActionReferences for thrust or rotation are not assigned.");
        }
    }

    public void GetInput(out bool thrust, out RotationInput rotation)
    {
        thrust = thrustAction.action.IsPressed();

        rotateAction.action.TryReadValue(out float value);

        rotation = value > 1e-5f ? RotationInput.Right :
                   value < -1e-5f ? RotationInput.Left :
                   RotationInput.None;
    }
}