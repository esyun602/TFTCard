
public class InputManager
{
	public static InputManager Instance { get; } = new();
	public InputSystem_Actions InputActions { get; } = new();
}