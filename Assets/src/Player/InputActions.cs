using UnityEngine;

public class InputActions : MonoBehaviour {
	public Vector2 movement;
	public bool    jump = false;
	public bool    dash;

	InputSystem_Actions _inputActions;

	void Update() {
		movement = _inputActions.Player.Move.ReadValue<Vector2>();
		jump     = _inputActions.Player.Jump.WasPressedThisFrame();
		dash     = _inputActions.Player.Dash.WasPressedThisFrame();
	}

	void Awake() {
		_inputActions = new InputSystem_Actions();
	}

	void OnEnable() {
		_inputActions.Enable();
	}

	void OnDisable() {
		_inputActions.Disable();
	}
}
