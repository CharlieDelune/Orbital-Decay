public class InGameResource {
	private string name;
	private bool isBaseResource;

	public InGameResource(string name, bool isBaseResource)
	{
		this.name = name;
		this.isBaseResource = isBaseResource;
	}

	public string Name { get { return name; } }
	public bool IsBaseResource { get { return isBaseResource; } }
}