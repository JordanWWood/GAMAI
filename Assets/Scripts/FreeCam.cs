using UnityEngine;

// Courtesy of ashleydavis on github https://gist.github.com/ashleydavis/f025c03a9221bc840a2b 

/// <summary>
/// A simple free camera to be added to a Unity game object.
/// 
/// Keys:
///	wasd / arrows	- movement
///	q/e 			- up/down (local space)
///	r/f 			- up/down (world space)
///	pageup/pagedown	- up/down (world space)
///	hold shift		- enable fast movement mode
///	right mouse  	- enable free look
///	mouse			- free look / rotation
///     
/// </summary>
public class FreeCam : MonoBehaviour {
    /// <summary>
    /// Normal speed of camera movement.
    /// </summary>
    public float movementSpeed = 10f;

    /// <summary>
    /// Speed of camera movement when shift is held down,
    /// </summary>
    public float fastMovementSpeed = 100f;

    /// <summary>
    /// Sensitivity for free look.
    /// </summary>
    public float freeLookSensitivity = 3f;

    /// <summary>
    /// Amount to zoom the camera when using the mouse wheel.
    /// </summary>
    public float zoomSensitivity = 10f;

    /// <summary>
    /// Amount to zoom the camera when using the mouse wheel (fast mode).
    /// </summary>
    public float fastZoomSensitivity = 50f;

    /// <summary>
    /// Set to true when free looking (on right mouse button).
    /// </summary>
    private bool _looking;

    void Update() {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var newMovementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

        var position = transform.position;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            position += (Time.deltaTime * newMovementSpeed * -transform.right);

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            position += (Time.deltaTime * newMovementSpeed * transform.right);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            position += (Time.deltaTime * newMovementSpeed * transform.forward);

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            position += (Time.deltaTime * newMovementSpeed * -transform.forward);

        if (Input.GetKey(KeyCode.Q))
            position += (Time.deltaTime * newMovementSpeed * transform.up);

        if (Input.GetKey(KeyCode.E))
            position += (Time.deltaTime * newMovementSpeed * -transform.up);

        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
            position += (Time.deltaTime * newMovementSpeed * Vector3.up);

        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
            position += (Time.deltaTime * newMovementSpeed * -Vector3.up);

        if (_looking) {
            var localEulerAngles = transform.localEulerAngles;
            var newRotationX = localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            var newRotationY = localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
            transform.localEulerAngles = localEulerAngles;
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0) {
            var newZoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
            position += newZoomSensitivity * axis * transform.forward;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1)) {
            StopLooking();
        }

        transform.position = position;
    }

    void OnDisable() {
        StopLooking();
    }

    /// <summary>
    /// Enable free looking.
    /// </summary>
    public void StartLooking() {
        _looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Disable free looking.
    /// </summary>
    public void StopLooking() {
        _looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}