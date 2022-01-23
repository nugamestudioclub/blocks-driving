using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class ToggleableController : MonoBehaviour
{

	[SerializeField]
	private bool on;
	



	private void OnMouseDown()
	{

		Debug.Log($"Clicking {gameObject.name}");
		on = !on;
		Toggle();
	}

	public abstract void Toggle();

}
