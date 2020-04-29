using UnityEngine;

/// Handles local camera movement and rotation
public class CameraMovement : MonoBehaviour
{

	[SerializeField] private Transform cameraSubWrapper;
	[SerializeField] private Camera camera;
	[SerializeField] private float rotateSpeed;
	[SerializeField] private float backAndForthFactor;

	[SerializeField] private float minZ;
	[SerializeField] private float maxZ;

	[SerializeField] private float minY;
	[SerializeField] private float maxY;

	[SerializeField] private float minRotation;
	[SerializeField] private float maxRotation;

	[SerializeField] private float maxZoom;
	[SerializeField] private float zoomSensitivityScroll;
	[SerializeField] private float zoomSensitivityKeypress;
	[SerializeField] private float innerZoomThreshold;
	[SerializeField] private float maxZoomPosition;

	private Rigidbody rb;

	private float distanceFactor = 0.0f;
	private float zoomPosition = 0.0f;

	/// Rotation and distance

	private void Awake()
	{
		this.rb = this.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		float v = Input.GetAxis("Vertical");

		this.distanceFactor = Mathf.Clamp(this.distanceFactor - (v * this.backAndForthFactor * Time.deltaTime), 0.0f, 1.0f);
		Vector3 basePosition = new Vector3(
			this.cameraSubWrapper.localPosition.x,
			Mathf.Lerp(this.minY, this.maxY, this.distanceFactor),
			Mathf.Lerp(this.minZ, this.maxZ, this.distanceFactor)
		);

		Vector3 baseNonLocalPosition = basePosition + this.transform.position;

		float declineAngle = Mathf.Lerp(this.minRotation, this.maxRotation, this.distanceFactor);

		this.cameraSubWrapper.transform.localEulerAngles = new Vector3(
			declineAngle,
			this.cameraSubWrapper.localEulerAngles.y,
			this.cameraSubWrapper.localEulerAngles.z
		);

		float z = 0.0f;
		float scrollInput = -Input.GetAxis("Mouse ScrollWheel");
		float keyInput = Input.GetAxis("Zoom");
		bool usingScroll;
		if(Mathf.Abs(scrollInput) > Mathf.Abs(keyInput))
		{
			usingScroll = true;
			z = scrollInput;
		}
		else
		{
			usingScroll = false;
			z = keyInput;
		}
		this.zoomPosition = Mathf.Clamp(this.zoomPosition - z * Time.deltaTime * ((usingScroll) ? this.zoomSensitivityScroll : this.zoomSensitivityKeypress), 0.0f, this.maxZoomPosition);

		if(this.zoomPosition > this.innerZoomThreshold)
		{
			GameStateManager.Instance.InnerZoomed = true;
		}
		else if(GameStateManager.Instance.InnerZoomed)
		{
			GameStateManager.Instance.InnerZoomed = false;
		}

		this.cameraSubWrapper.localPosition = basePosition - ((baseNonLocalPosition - GameStateManager.Instance.GridInView.transform.position) * this.zoomPosition);
	}

	private void FixedUpdate()
	{
		float h = Input.GetAxis("Horizontal");
		this.rb.AddTorque(0.0f, -h * this.rotateSpeed, 0.0f);
	}
}
