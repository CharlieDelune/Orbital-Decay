using System.Collections;
using System.Collections.Generic;

public abstract class Recipe
{
	private string recipeName;
	private string outputName;
	private string belongingUnit;
	private List<(InGameResource, int)> inputs;

	private bool isFinalized;

	public Recipe(string recipeName, string outputName)
	{
		this.recipeName = recipeName;
		this.outputName = outputName;
		this.inputs = new List<(InGameResource, int)>();

		isFinalized = false;
	}

	public void AddInput(InGameResource resource, int amount)
	{
		if(isFinalized)
		{
			throw new System.Exception("Cannot add additional inputs to a finalized recipe");
		}
		if(inputs.Exists(((InGameResource, int) pair) => pair.Item1 == resource))
		{
			throw new System.Exception(outputName+"'s recipe contains the resource \""+resource.Name+"\" multiple times");
		}
		inputs.Add((resource, amount));
	}

	public void FinalizeRecipe()
	{
		if(isFinalized)
		{
			throw new System.Exception("Recipe has already been finalized");
		}
		isFinalized = true;
	}

	public string RecipeName { get { return recipeName; } }
	public string OutputName { get { return outputName; } }
	public string BelongingUnit { get { return belongingUnit; } }
	public List<(InGameResource, int)> Inputs { get { return inputs; } }
}
