public class ResourceRecipe : Recipe
{
	private int outputQuantity;
	private int maxStack;

	public ResourceRecipe(string outputName, string belongingUnit, int outputQuantity, int maxStack) : base(outputName, belongingUnit)
	{
		this.outputQuantity = outputQuantity;
		this.maxStack = maxStack;
	}

	public int OutputQuantity { get { return outputQuantity; } }
	public int MaxStack { get { return maxStack; } }
}
