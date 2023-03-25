using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodcutterController : MonoBehaviour
{
	[SerializeField] private AnimController animController;
	[SerializeField] private string direction = "SW";

	private string[] availableActions = new string[] { "chopping"};
	private string action = "chopping";
	private System.Random random = new System.Random();
	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine("Action", 0.3f);
	}

	IEnumerator Action(float second)
	{
		yield return new WaitForSeconds(second);

		animController.ChangeAnim(action);
		animController.Turn(direction);
		animController.UpdateCharacterAnimation();

		//float nextTime = (float)(new System.Random().NextDouble()) * 4f + 2f;
		//StartCoroutine("Action", nextTime);
	}
}
