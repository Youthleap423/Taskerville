using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LadyController : MonoBehaviour
{
	[SerializeField] private AnimController animController;
	[SerializeField] private string direction = "W";

	private string[] availableActions = new string[] { "acknowledge", "angry", "cocky", "crazy", "happy"};
	private string action = "";
    private System.Random random = new System.Random();
	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine("Action", 0.3f);
	}

	IEnumerator Action(float second)
	{
		yield return new WaitForSeconds(second);

		int index = random.Next(availableActions.Length);
		action = availableActions[index];

		animController.ChangeAnim(action);
		animController.Turn(direction);
		animController.UpdateCharacterAnimation();

		float nextTime = (float)(new System.Random().NextDouble()) * 4f + 2f;
		StartCoroutine("Action", nextTime);
	}
}
