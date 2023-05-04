using System;
using UnityEngine;
using UnityEngine.UI;

public class PyrahnaMenu : MonoBehaviour
{
	[SerializeField]
	private Slider _bendSlider;

	[SerializeField]
	private Slider _stretchSlider;

	[SerializeField]
	private Slider _ballPositionSlider;

	public event Action<float> OnBendChanged;
	public event Action<float> OnStretchChanged;
	public event Action<float> OnBallPositionChanged;

	private void Awake()
	{
		_bendSlider.onValueChanged.AddListener(Handle_OnBendChanged);
		_stretchSlider.onValueChanged.AddListener(Handle_OnStretchChanged);
		_ballPositionSlider.onValueChanged.AddListener(Handle_OnBallPositionChanged);
	}

	private void Handle_OnBendChanged(float newValue)
	{
		OnBendChanged?.Invoke(newValue);
	}

	private void Handle_OnStretchChanged(float newValue)
	{
		OnStretchChanged?.Invoke(newValue);
	}

	private void Handle_OnBallPositionChanged(float newValue)
	{
		OnBallPositionChanged?.Invoke(newValue);
	}
}