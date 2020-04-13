public class ResourceRecipe : Recipe
{
	private int outputQuantity;
	private int maxStack;

	public ResourceRecipe(string recipeName, string outputName, int outputQuantity, int maxStack) : base(recipeName, outputName)
	{
		this.outputQuantity = outputQuantity;
		this.maxStack = maxStack;
	}

	public int OutputQuantity { get { return outputQuantity; } }
	public int MaxStack { get { return maxStack; } }
}
