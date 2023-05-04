using UnityEngine;

public class BallController : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer _headRenderer;

	private Vector3 _initialLocPosition;

	private void Awake()
	{
		_initialLocPosition = transform.localPosition;
	}
	
	public void SetPosition(float position)
	{
		float positionFactor = (position - 0.85f) / (1.15f - 0.85f) - 0.85f;
		float newPosZ = _initialLocPosition.z + Mathf.Lerp(-3f, 0.75f, positionFactor * (1 / 0.15f));
		transform.localPosition = new Vector3(_initialLocPosition.x, _initialLocPosition.y, newPosZ);
	}
	
	public void Setscale(float position)
	{
		float positionFactor = (position - 0.85f) / (1.15f - 0.85f) - 0.85f;
		float newScaleFactor = Mathf.Lerp(0.05f, 0.9f, positionFactor * (1 / 0.15f));
		transform.localScale = Vector3.one * newScaleFactor;
	}
}
