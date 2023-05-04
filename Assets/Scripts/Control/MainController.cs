using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
	[SerializeField]
	private PyrahnaController _pyrahnaController;

	[SerializeField]
	private PyrahnaMenu _pyrahnaMenu;

	private void Awake()
	{
		_pyrahnaMenu.OnBendChanged += Handle_OnBendChanged;
		_pyrahnaMenu.OnStretchChanged += Handle_OnStretchChanged;
		_pyrahnaMenu.OnBallPositionChanged += Handle_OnBallPositionChanged;
	}

	private void Handle_OnBendChanged(float newValue)
	{
		_pyrahnaController.SetBend(newValue);
	}

	private void Handle_OnStretchChanged(float newValue)
	{
		_pyrahnaController.SetStretch(newValue);
	}
    
    private void Handle_OnBallPositionChanged(float newValue)
	{
		_pyrahnaController.SetBallPosition(newValue);
	}
}
