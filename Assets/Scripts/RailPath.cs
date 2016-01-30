using UnityEngine;
using System.Collections.Generic;

public class RailPath : MonoBehaviour {

	public List<Transform> nodes;

	private List<float>	fragmentLengths = new List<float>();

	public float totalLength { get; private set; }

	#if UNITY_EDITOR
	[InspectorButton("UpdateNodes")]
	public bool updateNodes;
	#endif

	void Awake () {
		totalLength = 0f;

		fragmentLengths.Clear ();

		for (int i = 0; i < nodes.Count - 1; ++i) {
			fragmentLengths.Add(Vector3.Distance (nodes [i].position, nodes [i + 1].position));
			totalLength += fragmentLengths [i];
		}

		fragmentLengths.Add(Vector3.Distance (nodes [nodes.Count - 1].position, nodes [0].position));
		totalLength += fragmentLengths [nodes.Count - 1];
	}

	public Vector3 GetPositionOnPath (float distance) {
		distance %= totalLength;

		int fragmentIndex = 0;
		while (true) {
			float fragmentLength = fragmentLengths [fragmentIndex];

			if (distance < fragmentLength) {
				Vector3 lastNode = nodes[fragmentIndex].position;
				Vector3 nextNode;

				if (fragmentIndex < nodes.Count - 1)
					nextNode = nodes [fragmentIndex + 1].position;
				else
					nextNode = nodes [0].position;

				return Vector3.MoveTowards (lastNode, nextNode, distance);
			}

			distance -= fragmentLength;
			++fragmentIndex;
		}
	}
		
	public Vector3 GetPositionBetweenIndices (int from, int to, float interpolation) {
		if (from > to) {
			Debug.LogWarning ("Get Position Between Indices where: from > to");
			return Vector3.zero;
		}
		if (from > nodes.Count) {
			Debug.LogWarning ("Get Position Between Indices where: from > nodes.Count");
			return Vector3.zero;
		}
		if (to > nodes.Count) {
			Debug.LogWarning ("Get Position Between Indices where: to > nodes.Count");
			return Vector3.zero;
		}

		interpolation = Mathf.Clamp01 (interpolation);

		float distance = 0f;
		for (int i = from; i < to; ++i)
			distance += fragmentLengths [i];

		distance *= interpolation;

		int currentindex = from;
		while (true) {
			float fragmentLength = fragmentLengths [currentindex < nodes.Count ? currentindex : 0];

			if (distance > fragmentLength) {
				distance -= fragmentLength;
				++currentindex;
			} else {
				Vector3 lastNode = nodes[currentindex].position;
				Vector3 nextNode;

				if (currentindex < nodes.Count - 1)
					nextNode = nodes [currentindex + 1].position;
				else
					nextNode = nodes [0].position;

				return Vector3.MoveTowards (lastNode, nextNode, distance);
			}
		}
	}

	public Vector3 GetPositionAfterIndice (int from, float interpolation) {
		return GetPositionBetweenIndices(from, nodes.Count, interpolation);
	}

	public Vector3 GetStart () {
		return nodes [0].position;
	}

	#if UNITY_EDITOR
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		for (int i = 0; i < nodes.Count - 1; ++i)
			Gizmos.DrawLine (nodes [i].position, nodes [i + 1].position);
		Gizmos.DrawLine (nodes [nodes.Count - 1].position, nodes [0].position);
	}
	#endif

	void UpdateNodes () {
		nodes.Clear ();

		int count = 0;
		foreach (Transform child in transform) {			
			nodes.Add (child);
			child.gameObject.name = "Node " + count;
			++count;
		}
	}

}
