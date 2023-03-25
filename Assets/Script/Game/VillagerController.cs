using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerController : MonoBehaviour
{  //constrolls the dobbit during construction

	private int curPathIndex;
	public string walkAnim = "Walk", actionAnim = "Idle";
	public float speed = 55.0f, mass = 5.0f;
	private float curSpeed, pathLength;
	private Vector3 targetPoint, velocity;

	//public bool isLooping = true;

	private enum DobbitState { Walk, Action, None }
	private DobbitState currentState = DobbitState.None;
	private AnimController dobbitAnimController;

	public ConstructionPath path;

	private Vector3 prevPosition = Vector3.zero;
	public float actionTime = 2.0f;

    private void OnEnable()
    {
        path.onResetPath += Path_onResetPath;   
    }

    private void OnDisable()
    {
		path.onResetPath -= Path_onResetPath;
	}

    void Start()
	{

		dobbitAnimController = GetComponent<AnimController>();
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

	private void Path_onResetPath()
	{
		pathLength = path.Length;
		curPathIndex = 0;
		Vector3 pos = path.GetPoint(curPathIndex);
		float zDepth = Utilities.GetZDepth(pos);
		transform.position = new Vector3(pos.x, pos.y, zDepth); //zDepth correction at instantiation
		currentState = DobbitState.Walk;
	}

	private void Walk()
	{
		curSpeed = speed * Time.deltaTime;
		targetPoint = path.GetPoint(curPathIndex);

		//if path point reached, move to next point
		if (Vector2.Distance(transform.position, targetPoint) <
			path.Radius)
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

			currentState = DobbitState.Action;
			StartCoroutine("Action", actionTime);

            //Debug.LogError(curPathIndex + ":" + path.GetPoint(curPathIndex));
        }
        else
        {
			
			velocity += Steer(targetPoint);
			transform.position += velocity;
		}
	}

	IEnumerator Action(float second)
	{
		ChangeAnimation(actionAnim);
		yield return new WaitForSeconds(second);
		currentState = DobbitState.Walk;
		ChangeAnimation(walkAnim);
	}

	private void ChangeAnimation(string action)
	{
		string direction = " ";
		direction = Utilities.GetDirectionString(targetPoint, path.GetPoint(curPathIndex));

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

