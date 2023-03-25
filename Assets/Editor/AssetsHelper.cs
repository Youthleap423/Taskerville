
using Assets.Scripts.UIControllersAndData.Images;
using UIControllersAndData.Store.Categories.Buildings;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AssetsHelper : MonoBehaviour 
{

	/// <summary>
	/// Creates an asset file for MaxCap
	/// </summary>
	[MenuItem("Tools/Create init data table")]
	static void CreateInitData()
	{
		InitData asset = ScriptableObject.CreateInstance<InitData>();
		AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_INIT_DATA_ASSET_CREATE);
		AssetDatabase.SaveAssets();
	}
		
	/// <summary>
	/// Creates an asset file for Images
	/// </summary>
	[MenuItem("Tools/Create images table")]
	static void CreateImagesTable()
	{
		ImageItemList asset = ScriptableObject.CreateInstance<ImageItemList>();
		AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_IMAGE_ASSET);
		AssetDatabase.SaveAssets();
	}
		
	/// <summary>
	/// Creates an asset file for store category
	/// </summary>
	[MenuItem("Tools/Create road category table")]
	static void CreateStoreCategoryTable()
	{
		BuildingsCategoryData asset = ScriptableObject.CreateInstance<BuildingsCategoryData>();
		AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_ROAD_CATEGORY_ASSET);
		AssetDatabase.SaveAssets();
	}
		
	/// <summary>
	/// Creates an asset file for buildings category
	/// </summary>
	[MenuItem("Tools/Create buildings category table")]
	static void CreateBuildingsCategoryTable()
	{
		BuildingsCategoryData asset = ScriptableObject.CreateInstance<BuildingsCategoryData>();
		AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_BUILDINGS_CATEGORY_ASSET);
		AssetDatabase.SaveAssets();
	}


	/// <summary>
	/// Creates an asset file for walls category
	/// </summary>
	[MenuItem("Tools/Create resource category table")]
	static void CreateResourceCategoryTable()
	{
		ResourceData asset = ScriptableObject.CreateInstance<ResourceData>();
		AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_RESOURCE_CATEGORY_ASSET);
		AssetDatabase.SaveAssets();
	}
	/// <summary>
	/// Creates an asset file for unit category
	/// </summary>
	[MenuItem("Tools/Create villager category table")]
	static void CreateVillagerCategoryTable()
	{
		VillageData asset = ScriptableObject.CreateInstance<VillageData>();
		AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_VILLAGER_CATEGORY_ASSET);
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Tools/Create schedule table")]
	static void CreateScheduleTable()
	{
		ScheduleData asset = ScriptableObject.CreateInstance<ScheduleData>();
		AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_SCHEDULE_ASSET);
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Tools/Create Artifact table")]
	static void CreateArtifactTable()
	{
		ArtifactData asset = ScriptableObject.CreateInstance<ArtifactData>();
		AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_ARTIFACT_ASSET);
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Tools/Load")]
	static void LoadArtwork()
    {
		var artifactData = AssetDatabase.LoadAssetAtPath<ArtifactData>(Constants.PATH_FOR_ARTIFACT_ASSET);
		List<Dictionary<string, object>> data = CSVReader.Read("paintingvalues(final)");
		string str = "";
		for (var i = 0; i < data.Count; i++)
		{
			
			var name = data[i]["PAINTING_NAME"].ToString();
			var artist_name = data[i]["ARTIST_NAME"].ToString();
			var path = data[i]["PAINTING_IMAGE_PATH"].ToString();
			path = path.Remove(0, 14);
			path = path.Substring(0, path.Length - 4);

			var newArt = new CArtwork();
			newArt.id = i.ToString();
			newArt.name = name;
			newArt.artist_name = artist_name;
			newArt.contactInfo = "www.enelsonduran.com";
			newArt.image_path = path.Trim();

			artifactData.artworks.Add(newArt);
			str = str + string.Format("- name: {0}\n  id: {1}\n  artist_name: {2}\n  contact info: {3}\n image_path: {4}\n", newArt.id, newArt.name, newArt.artist_name, newArt.contactInfo, newArt.image_path);
			//print("PAINTING_NAME " + name + " " +
			//       "ARTIST_NAME " + artist_name + " " +
			//       "PAINTING_IMAGE_PATH " + path);
		}
		Debug.LogError(str);
	}
}
