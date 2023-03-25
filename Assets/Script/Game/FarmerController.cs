using UnityEngine;
using System.Collections;

public class FarmerController : MonoBehaviour
{  //constrolls the dobbit during construction

	private int curPathIndex;
	public string walkAnim = "Walk", actionAnim = "Plow";
	public float speed = 55.0f, mass = 5.0f;
	private float curSpeed, pathLength;
	private Vector3 targetPoint, velocity;

	//public bool isLooping = true;

	private enum DobbitState { Walk, Action, None }
	private DobbitState currentState = DobbitState.None;
	private AnimController dobbitAnimController;

	public int gatherActionIndex;
	public ConstructionPath path;

	private Vector3 prevPosition = Vector3.zero;

    void Awake()
    {
		path.onResetPath += Path_onResetPath;
	}

    private void Start()
    {
		dobbitAnimController = GetComponent<AnimController>();
	}

    private void OnDestroy()
    {
		path.onResetPath -= Path_onResetPath;
	}

    private void Path_onResetPath()
    {
		pathLength = path.Length;
		curPathIndex = 0;

		Vector3 pos = path.GetPoint(curPathIndex);
		float zDepth = pos.z - 10 / (pos.y + 3300);
		transform.position = new Vector3(pos.x, pos.y, zDepth); //zDepth correction at instantiation
		currentState = DobbitState.Walk;						//velocity = Vector3.zero;
	}


	void Update()
	{
		switch (currentState)
		{
			case DobbitState.Walk:
				Walk();
				break;
		}
	}

	private void Walk()
	{
		curSpeed = speed * Time.deltaTime;
		targetPoint = path.GetPoint(curPathIndex);
		
		//if path point reached, move to next point
		if (Vector3.Distance(transform.position, targetPoint) < path.Radius)
		{
			if (curPathIndex < pathLength - 1)
			{
				curPathIndex++;
            }
            else
            {
				curPathIndex = 0;
			}
			prevPosition = targetPoint;

			if (curPathIndex == gatherActionIndex)
			{
				currentState = DobbitState.Action;
				actionAnim = "Gather1";
				StartCoroutine("Action", 4.0f);
			}else if (curPathIndex == 1)
            {
				currentState = DobbitState.Action;
				actionAnim = "Seed2";
				StartCoroutine("Action", 0.25f);
			}
			else
			{
				ChangeAnimation(walkAnim);
			}

			//Debug.LogError(curPathIndex + ":" + path.GetPoint(curPathIndex));
		}

		velocity += Steer(targetPoint);
		transform.position += velocity;
	}

	IEnumerator Action(float second)
	{
		ChangeAnimation(actionAnim);
		yield return new WaitForSeconds(second);
		ChangeAnimation(walkAnim);
		currentState = DobbitState.Walk;
	}

	private void ChangeAnimation(string action)
	{
		if (action == "Hammer_Sit")
		{
			action = "Hammer1";
		}
		string direction = " ";

		switch (action)
		{

			case "Walk":
				direction = Utilities.GetDirectionString(targetPoint, path.GetPoint(curPathIndex));
				//switch (curPathIndex)
				//{
				//	case 1:
				//		direction = "SW";
				//		break;
				//	case 2:
				//		direction = "SE";
				//		break;
				//	case 3:
				//		direction = "NE";
				//		break;
				//	case 4:
				//		direction = "SW";
				//		break;
				//	case 5:
				//		direction = "NW";
				//		break;
				//	case 0:
				//		direction = "NE";
				//		break;
				//}
				break;

			case "Gather1":
				switch (curPathIndex)
				{
					case 0:
						direction = "N";
						break;
					case 1:
						direction = "E";
						break;
					case 2:
						direction = "S";
						break;
					case 3:
						direction = "W";
						break;
					default:
						direction = "W";
						break;

				}
				break;


			case "Seed2":
				direction = "W";
				break;

			default:
				break;
		}

		//break;
		//}

		dobbitAnimController.ChangeAnim(action);
		dobbitAnimController.Turn(direction);
		dobbitAnimController.UpdateCharacterAnimation();
	}

	public Vector3 Steer(Vector3 target)
	{
		Vector3 desiredVelocity = (target - transform.position);
		float dist = desiredVelocity.magnitude;

		desiredVelocity.Normalize();

		if (dist < 10.0f)
			desiredVelocity *= (curSpeed * (dist / 10.0f));
		else
			desiredVelocity *= curSpeed;

		Vector3 steeringForce = desiredVelocity - velocity;
		Vector3 acceleration = steeringForce / mass;

		transform.position += velocity;

		return acceleration;

	}
}

