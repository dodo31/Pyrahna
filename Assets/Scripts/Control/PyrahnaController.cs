using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PyrahnaController : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer _headRenderer;

	[SerializeField]
	private Transform _headArmature;

	[SerializeField]
	private SkinnedMeshRenderer _bodyRenderer;

	[SerializeField]
	private Transform _bodyArmature;

	[SerializeField]
	private BallController _ballController;

	private Vector3[] _bodyVerticesOriginal;

	private float _currentBend;
	private float _currentStretch;
	private float _currentBallPosition;

	private void Awake()
	{
		_currentBend = 0;
		_currentStretch = 0;
		_currentBallPosition = 0;

		_bodyRenderer.sharedMesh = Instantiate(_bodyRenderer.sharedMesh);

		_bodyVerticesOriginal = new Vector3[_bodyRenderer.sharedMesh.vertexCount];
		Array.Copy(_bodyRenderer.sharedMesh.vertices, _bodyVerticesOriginal, _bodyRenderer.sharedMesh.vertexCount);
	}

	public void SetBend(float newBend)
	{
		SetStemBonesOrientation(newBend);
		RefreshHeadTransform();
		_currentBend = newBend;
	}

	public void SetStretch(float newStretch)
	{
		SetStemBonesScale(newStretch);
		RefreshHeadTransform();
		_currentStretch = newStretch;
	}

	public void SetBallPosition(float newBallPosition)
	{
		DeformStem(newBallPosition);
		SetMouthOpening(newBallPosition);
		SetHeadScale(newBallPosition);
		_ballController.SetPosition(newBallPosition);
		_ballController.Setscale(newBallPosition);
		_currentBallPosition = newBallPosition;
	}

	public void SetStemBonesOrientation(float orientation)
	{
		for (int i = 0; i < _bodyRenderer.bones.Length; i++)
		{
			Transform boneTransform = _bodyRenderer.bones[i];

			if (IsStemBone(boneTransform))
			{
				if (i == 0)
				{
					boneTransform.localRotation = Quaternion.Euler(90, 0, orientation);
				}
				else
				{
					boneTransform.localRotation = Quaternion.Euler(0, 0, orientation);
				}
			}
		}
	}

	public void SetStemBonesScale(float length)
	{
		int stemBoneIndex = 0;

		for (int i = 0; i < _bodyRenderer.bones.Length; i++)
		{
			Transform boneTransform = _bodyRenderer.bones[i];

			if (IsStemBone(boneTransform))
			{
				if (stemBoneIndex > 1)
				{
					boneTransform.localScale = new Vector3(1, length, 1);
				}

				stemBoneIndex++;
			}
		}
	}

	public void DeformStem(float position)
	{
		float positionFactor = (position - 0.85f) / (1.15f - 0.85f) + 0.15f;

		float deformationRadius = 0.1f;

		Dictionary<float, List<int>> positionsToMatchingIndexClusters = new Dictionary<float, List<int>>();

		Vector3[] bodyVerticesCurrent = _bodyRenderer.sharedMesh.vertices;
		Color[] bodyColorsCurrent = _bodyRenderer.sharedMesh.colors;

		for (int i = 0; i < bodyColorsCurrent.Length; i++)
		{
			Color bodyColor = bodyColorsCurrent[i];

			if (!bodyColor.Equals(Color.red))
			{
				List<int> indexCluster = new List<int>();

				if (!positionsToMatchingIndexClusters.ContainsKey(bodyColor.g))
				{
					positionsToMatchingIndexClusters[bodyColor.g] = indexCluster;
				}
				else
				{
					indexCluster = positionsToMatchingIndexClusters[bodyColor.g];
				}

				indexCluster.Add(i);
			}
		}

		Vector3[] bodyVerticesNew = new Vector3[_bodyVerticesOriginal.Length];
		Array.Copy(_bodyVerticesOriginal, bodyVerticesNew, _bodyVerticesOriginal.Length);

		foreach (KeyValuePair<float, List<int>> positionToMatchingIndexCluster in positionsToMatchingIndexClusters)
		{
			float clusterPosition = positionToMatchingIndexCluster.Key;
			List<int> clusterIndices = positionToMatchingIndexCluster.Value;
			List<Vector3> clusterVertices = clusterIndices.Select(index => _bodyVerticesOriginal[index]).ToList();

			Vector3 totalVertex = Vector3.zero;
			foreach (Vector3 vertex in clusterVertices)
			{
				totalVertex += vertex;
			}
			Vector3 clusterCenter = totalVertex / (clusterIndices.Count * 1f);

			float positionDelta = Mathf.Abs(positionFactor - clusterPosition);

			if (positionDelta < deformationRadius)
			{
				float deformation = (1 - positionDelta / deformationRadius) * 2f;

				foreach (int vertexIndex in clusterIndices)
				{
					Vector3 clusterVertex = _bodyVerticesOriginal[vertexIndex];
					Vector3 vertexDelta = clusterVertex - clusterCenter;
					bodyVerticesNew[vertexIndex] = clusterCenter + vertexDelta * (1 + deformation);
				}
			}
		}

		_bodyRenderer.sharedMesh.vertices = bodyVerticesNew;
	}

	public void SetMouthOpening(float newBallPosition)
	{
		Transform[] rightLipsBones = _headRenderer.bones
			.Where(bone => bone.name.ToLower().Contains("lips") && bone.name.ToLower().Contains("right"))
			.ToArray();

		Transform[] leftLipsBones = _headRenderer.bones
			.Where(bone => bone.name.ToLower().Contains("lips") && bone.name.ToLower().Contains("left"))
			.ToArray();

		Transform rightLipsRootBone = rightLipsBones.First();
		Transform leftLipsRootBone = leftLipsBones.First();

		float angleFactor = 1 - (newBallPosition - 0.85f) / (1.15f - 0.85f);

		float rightLipsAngle = Mathf.Lerp(18, -15, angleFactor);
		float leftLipsAngle = Mathf.Lerp(-85, -42, angleFactor);

		Vector3 currentRightLipsEuler = rightLipsRootBone.localRotation.eulerAngles;
		Vector3 currentLeftLipsEuler = leftLipsRootBone.localRotation.eulerAngles;

		rightLipsRootBone.localRotation = Quaternion.Euler(currentRightLipsEuler.x, currentRightLipsEuler.y, rightLipsAngle + 180);
		leftLipsRootBone.localRotation = Quaternion.Euler(currentLeftLipsEuler.x, currentLeftLipsEuler.y, leftLipsAngle + 180);
	}

	private void SetHeadScale(float newBallPosition)
	{
		float positionFactor = (newBallPosition - 0.85f) / (1.15f - 0.85f) - 0.8f;
		float newScaleFactor = Mathf.Lerp(1f, 1.15f, positionFactor * (1 / 0.2f));
		_headArmature.parent.localScale = Vector3.one * newScaleFactor;
	}

	private void RefreshHeadTransform()
	{
		Transform lastBoneTransform = _bodyRenderer.bones.Last(boneTransform => IsStemBone(boneTransform));
		Transform lastBoneEndTransform = lastBoneTransform.GetChild(0);

		_headArmature.parent.position = lastBoneEndTransform.position;
		_headArmature.parent.rotation = lastBoneEndTransform.rotation * Quaternion.Euler(-90, 0, 0);
	}

	private bool IsStemBone(Transform boneTransform)
	{
		return boneTransform.name.ToLower().Contains("stem");
	}

	public float CurrentBend { get => _currentBend; }
	public float CurrentStretch { get => _currentStretch; }
	public float CurrentBallPosition { get => _currentBallPosition; }
}
