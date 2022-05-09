using UnityEngine;

namespace BeyondEarthGames 
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMotor : MonoBehaviour
	{
		#region Inspector Exposed Variables

		[SerializeField] private PlayerMotorSettings _settings;
		[SerializeField] private Transform _camera;

		#endregion

		#region Private Variables

		private CharacterController _motor;

		private float cameraRotX = 0;
		private float angularVelocity = 0;

		#endregion

		#region Unity Callbacks

		private void Awake()
		{
			_motor = GetComponent<CharacterController>();

			if (_settings.LockCursorOnAwake)
				Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			HandleGravityAndJump();
			HandlePosition();
			HandleCameraAndBodyRotation();
		}

		private void OnApplicationFocus(bool focus)
		{
			if (_settings.LockCursorOnFocus)
				Cursor.lockState = CursorLockMode.Locked;
		}

		#endregion

		#region Private Methods

		private void HandleGravityAndJump()
		{
			if (!_settings.CanJump) return;
			
			if (_motor.isGrounded)
			{
				angularVelocity = -0.01f;

				if (Input.GetKeyDown(_settings.JumpKey))
				{
					angularVelocity = _settings.JumpForce;
				}
			}
			else
			{
				angularVelocity -= _settings.Gravity * Time.deltaTime;
			}
		}

		private void HandlePosition()
		{
			Vector3 move = new Vector3(Input.GetAxis("Horizontal"), angularVelocity, Input.GetAxis("Vertical"));

			move.x *= _settings.HorizontalMovementWeight * _settings.MovementSpeed * Time.deltaTime;
			move.z *= _settings.VerticalMovementWeight * _settings.MovementSpeed * Time.deltaTime;

			// NOTE: a better design needs to be implemented here.
			if (move.x != 0 && move.z != 0)
			{
				move.x *= 0.5f;
				move.z *= 0.5f;
			}

			_motor.Move(transform.TransformDirection(move));
		}

		private void HandleCameraAndBodyRotation()
		{
			Vector2 input = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

			cameraRotX = Mathf.Clamp(cameraRotX - input.x, _settings.MinCameraTilt, _settings.MaxCameraTilt);
			transform.Rotate(0, input.y, 0);

			_camera.localRotation = Quaternion.Euler(cameraRotX, _camera.localEulerAngles.y, _camera.localEulerAngles.z);
		}

		#endregion
	}
}
