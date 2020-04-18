using UnityEngine;

/// Handles local camera movement and rotation
public class CameraMovement : MonoBehaviour
{

	[SerializeField] private Transform camera;
	[SerializeField] private float rotateSpeed;
	[SerializeField] private float backAndForthFactor;

	[SerializeField] private float minZ;
	[SerializeField] private float maxZ;

	[SerializeField] private float minY;
	[SerializeField] private float maxY;

	[SerializeField] private float minRotation;
	[SerializeField] private float maxRotation;


	private Rigidbody rb;

	private float distanceFactor = 0.0f;

	/// Rotation and distance

	private void Awake()
	{
		this.rb = this.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		float h = Input.GetAxis("Horizontal");
		this.rb.AddTorque(0.0f, -h * this.rotateSpeed, 0.0f);

		float v = Input.GetAxis("Vertical");

		this.distanceFactor = Mathf.Clamp(this.distanceFactor - v * this.backAndForthFactor, 0.0f, 1.0f);
		this.camera.localPosition = new Vector3(
			this.camera.localPosition.x,
			Mathf.Lerp(this.minY, this.maxY, this.distanceFactor),
			Mathf.Lerp(this.minZ, this.maxZ, this.distanceFactor)
		);
		this.camera.transform.localEulerAngles = new Vector3(
			Mathf.Lerp(this.minRotation, this.maxRotation, this.distanceFactor),
			this.camera.localEulerAngles.y,
			this.camera.localEulerAngles.z
		);
	}
}