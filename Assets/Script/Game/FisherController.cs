using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisherController : MonoBehaviour
{
	[SerializeField]private AnimController animController;
	private string action = "fishing01";
	private string direction = "W";
	// Start is called before the first frame update
	void Start()
    {
		StartCoroutine("Action", 0.3f);
	}

	void Update()
	{
		if (action == "fishing04")
        {
			action = "fishing01";
			StartCoroutine("Action", 1f);
        }
		
	}

	private void Walk()
	{
		
	}

	IEnumerator Action(float second)
	{
		yield return new WaitForSeconds(second);
		animController.ChangeAnim(action);
		animController.Turn(direction);
		animController.UpdateCharacterAnimation();

		
		action = "fishing02";
		yield return new WaitForSeconds(Random.Range(2f, 3f));

		animController.ChangeAnim(action);
		animController.Turn(direction);
		animController.UpdateCharacterAnimation();

		action = "fishing03";
		yield return new WaitForSeconds(Random.Range(8f, 12f));

		animController.ChangeAnim(action);
		animController.Turn(direction);
		animController.UpdateCharacterAnimation();

		
		yield return new WaitForSeconds(Random.Range(1f, 2f));
		action = "fishing04";
		animController.ChangeAnim(action);
		animController.Turn(direction);
		animController.UpdateCharacterAnimation();
	}
}
