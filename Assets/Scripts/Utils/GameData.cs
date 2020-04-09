using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class GameData : MonoBehaviour
{
	private static GameData _instance;
	public static GameData Instance { get { return _instance; } }

	public TextAsset resourceFile;
	public TextAsset unitsFile;
	public TextAsset recipesFile;

	private Dictionary<string, InGameResource> nameToResource;
	private Dictionary<string, UnitInfo> nameToUnitInfo;
	private Dictionary<string, List<Recipe>> unitNameToRecipes;

	private UnitInfo mainBaseUnitInfo;
	private List<InGameResource> baseResources;

	public List<InGameResource> BaseResources { get { return baseResources; }}
	public UnitInfo MainBaseUnitInfo { get { return mainBaseUnitInfo; }}

	public List<Recipe> GetRecipes(string unitName) {
		return unitNameToRecipes[unitName];
	}

	public UnitInfo GetUnitInfo(string unitName) {
		return nameToUnitInfo[unitName];
	}

	public InGameResource GetResource(string resourceName) {
		return nameToResource[resourceName];
	}

	void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple GameData instances are present. There should only be one instance of a Singleton.");
		}
		else
		{
			_instance = this;
		}

		// Load Resources
		nameToResource = new Dictionary<string, InGameResource>();
		baseResources = new List<InGameResource>();
		using (StringReader reader = new StringReader(resourceFile.text))
		{
			string[] headers = reader.ReadLine().Split(',');
			string[] expectedHeaders = {"Name", "Is Base"};
			CheckFileHeaders("resources", expectedHeaders, headers);
			
			while (reader.Peek() != -1) 
			{
				string[] fields = reader.ReadLine().Split(',');
				InGameResource resource = new InGameResource(name: fields[0], isBaseResource: bool.Parse(fields[1]));

				nameToResource[resource.Name] = resource;
				if(resource.IsBaseResource)
				{
					baseResources.Add(resource);
				}
			}
		}

		// Load Units
		mainBaseUnitInfo = null;
		nameToUnitInfo = new Dictionary<string, UnitInfo>();
		using (StringReader reader = new StringReader(unitsFile.text))
		{
			string[] headers = reader.ReadLine().Split(',');
			string[] expectedHeaders = {"Name", "Range", "Health", "Armor", "Attack"};
			CheckFileHeaders("units", expectedHeaders, headers);
			
			while (reader.Peek() != -1) 
			{
				string[] fields = reader.ReadLine().Split(',');

				Unit unitPrefab = Resources.Load<Unit>("UnitPrefabs/"+fields[0]);

				if(unitPrefab == null)
				{
					throw new System.Exception("Failed to load unit in Resources/UnitPrefabs: "+fields[0]);
				}

				UnitInfo unitInfo = new UnitInfo(name: fields[0],
												 maxRange: int.Parse(fields[1]),
												 health: int.Parse(fields[2]),
												 armor: int.Parse(fields[3]),
												 attack: int.Parse(fields[4]),
												 unitPrefab: unitPrefab);

				nameToUnitInfo[fields[0]] = unitInfo;
				if(mainBaseUnitInfo != null)
				{
					mainBaseUnitInfo = unitInfo;
				}
			}
		}

		// Load Recipes
		unitNameToRecipes = new Dictionary<string, List<Recipe>>();
		using (StringReader reader = new StringReader(recipesFile.text))
		{
			string[] headers = reader.ReadLine().Split(',');
			string[] expectedHeaders = {"Output", "Output Quantity", "Max Stack", "Belonging Unit", "Input Item", "Input Quantity"};
			CheckFileHeaders("recipes", expectedHeaders, headers);
			

			Recipe recipe = null;
			while (reader.Peek() != -1) 
			{
				string[] fields = reader.ReadLine().Split(',');

				// Starting a new recipe
				if(fields[0] != "") {
					// recipe will be null in the first iteration 
					if(recipe != null)
					{
						RegisterRecipe(recipe);
					}

					// Check whether the recipe creates a resource or a unit
					if(nameToResource.ContainsKey(fields[0]))
					{
						recipe = new ResourceRecipe(outputName: fields[0],
													belongingUnit: fields[3],
													outputQuantity: int.Parse(fields[1]),
													maxStack: int.Parse(fields[2]));
					}
					else
					{
						recipe = new UnitRecipe(outputName: fields[0],
												belongingUnit: fields[3]);
					}
				}

				recipe.AddInput(nameToResource[fields[4]], int.Parse(fields[5]));
			}
			
			RegisterRecipe(recipe);
		}
	}

	private void RegisterRecipe(Recipe recipe)
	{
		recipe.FinalizeRecipe();

		List<Recipe> recipes = null;
		unitNameToRecipes.TryGetValue(recipe.BelongingUnit, out recipes);
		if(recipes == null)
		{
			recipes = new List<Recipe>();
		}
		recipes.Add(recipe);
		unitNameToRecipes[recipe.BelongingUnit] = recipes;
	}

	private void CheckFileHeaders(string file, string[] expectedHeaders, string[] actualHeaders)
	{
		if(expectedHeaders.Length != actualHeaders.Length)
		{
			throw new System.Exception("Expected file \"" + file + "\" to have " + expectedHeaders.Length + " headers, but instead it has: " + actualHeaders.Length);
		}
		for(int i=0; i<expectedHeaders.Length; ++i)
		{
			if(expectedHeaders[i] != actualHeaders[i])
			{
				throw new System.Exception("Expect column " + i + " of \"" + file + "\" to have value \"" + expectedHeaders[i] +"\", but instead got: "+actualHeaders[i]);
			}
		}
	}
}