using VideojogosLusofona.LusoLander;

public interface IInputSender
{
    void GetInput(out bool thrust, out RotationInput rotation);
}
