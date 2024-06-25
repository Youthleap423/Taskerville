using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIControllersAndData.Store.Categories.Buildings;

public class BuildingCreator : MonoBehaviour
{
	protected bool inConstruction = true;

	
	public float progress = 0f;     //for progress timer, one minute

	public int                              //for offline production then load
		buildingTime = 1,
		remainingTime = 1;

	public Sprite[] field_construction;
	public Sprite[] village_construction;
	[SerializeField]
	private SpriteRenderer _contructionRender;          //Toolhouse, Cannon, ArcherTower, etc 

	[SerializeField]
	private ConstructorController _contructorController;

	[SerializeField]
	protected Text remainTimeTF;

	public string[] field_anims;
	public string[] village_anims;

	public GBuilding gBuilding = null;
	public float timeToBuild = 0f;

	//protected Component soundFX;//, resourceGenerator;
	private string groundMaterial = "Grass";

	void Start()
	{
		//soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();
		remainingTime = buildingTime - 1;
	}

	private void OnApplicationFocus(bool focus)
	{
#if !UNITY_EDITOR
		if (focus)
		{
			if (gBuilding != null)
			{
				var passedSeconds = (float)((double.Parse(Utilities.SystemTimeInMillisecondsString) - double.Parse(gBuilding.Lbuilding.created_at)) / 1000);
				gBuilding.UpdateProgress(Mathf.Clamp01((float)(passedSeconds) / 60.0f / (float)buildingTime));
				progress = Mathf.Clamp01((float)(passedSeconds) / 60.0f / (float)(buildingTime));
			}
		}
#endif
	}

	void FixedUpdate()
	{
		if (inConstruction)
		{
			UpdateTime();
		}
	}

	private void UpdateTime()
	{
#if UNITY_EDITOR
		progress += /*Time.deltaTime / buildingTime / 60;*/ 0.0002f;
#else
		progress += Time.deltaTime / buildingTime / 60;
#endif
		progress = Mathf.Clamp(progress, 0, 1);

		if (gBuilding != null)
        {
			gBuilding.UpdateProgress(progress);
		}
		
		remainingTime = (int)(buildingTime * (1 - progress));

		ShowTime();

		if (progress == 1)             //building finished - the progress bar has reached 1												
		{
			OnComplete();
		}
		else
		{
			UpdateContructionRender();
		}
		
	}
	
	private void ShowTime()
    {
		var cTime = (float)(buildingTime) * (1 - progress);
		var totalSec = (int)(cTime * 60);
		var second = totalSec % 60;
		var min = (totalSec / 60) % 60;
		var hour = totalSec / 3600;
		var str = "";
		if (hour == 0)
        {
			str = string.Format("{0}m{1}s", min, second);
        }
        else
        {
			str = string.Format("{0}h{1}m{2}s", hour, min, second);
		}
		remainTimeTF.text = str;
    }

	virtual public void OnComplete()
	{
		if (!inConstruction)
		{
			return;
		}

		//((SoundFX)soundFX).BuildingFinished();

		gBuilding.UpdateProgress(1.0f);
		BuildManager.Instance.CompleteBuild(transform.parent.gameObject);

		var cgPath = gameObject.GetComponent<ConstructionPath>();
		if (cgPath != null)
		{
			cgPath.ResetPath();
		}

		Destroy(gameObject);
		inConstruction = false;
	}

	
	private void UpdateContructionRender()
	{
		int index = Mathf.FloorToInt(progress * 100 / 25);
		if (index < field_construction.Length)
		{
			var curSprite = groundMaterial.Equals("Grass") ? field_construction[index] : village_construction[index];
			_contructorController.actionAnim = groundMaterial.Equals("Grass") ? field_anims[index] : village_anims[index];
			_contructionRender.sprite = curSprite;
		}
	}

	public void SetGroundMaterial(string material)
	{
		groundMaterial = material;
		UpdateContructionRender();
	}

	virtual public void SetGBuilding(GBuilding building)
    {
		gBuilding = building;
		SetGroundMaterial(gBuilding.GroundMaterial);
    }
}
