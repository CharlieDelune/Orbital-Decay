using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectablePopup : MonoBehaviour
{

	private SelectableActionType? selectedAction;

	private Selectable parentSelectable;

	[SerializeField] private Button buttonPrefab;
	[SerializeField] private BuildButton buildButtonPrefab;
	[SerializeField] private Transform buttonsParent;
	private List<GameObject> buttons = new List<GameObject>();

	[SerializeField] private IntGameEvent onActionSelectedEvent;

	/// Loads the popup based on the selectable
	public void LoadPopup(Selectable selectable)
	{
		this.selectedAction = null;
		this.parentSelectable = selectable;
		List<SelectableActionType> validActionTypes = selectable.GetValidActionTypes();
		for(int i = 0; i < this.buttons.Count; i++)
		{
			Destroy(this.buttons[i]);
		}
		this.buttons.Clear();
		for(int i = 0; i < validActionTypes.Count; i++)
		{
			int g = i;
			Button button = Instantiate(this.buttonPrefab);
			this.buttons.Add(button.gameObject);
			button.transform.SetParent(this.buttonsParent, false);
			button.GetComponentsInChildren<Text>()[0].text = validActionTypes[i].ToString();
			button.onClick.AddListener(()=>this.onButtonPress(validActionTypes[g]));
		}
	}

	/// Loads the build option buttons
	private void loadBuildOptions()
	{
		Builder builder = GameStateManager.Instance.SelectedCell.Selectable as Builder;
		if(builder != null)
		{
			for(int i = 0; i < this.buttons.Count; i++)
			{
				Destroy(this.buttons[i].gameObject);
			}
			this.buttons.Clear();

			string[] buildOptions = builder.BuildOptionLabels;

			for(int i = 0; i < buildOptions.Length; i++)
			{
				int g = i;
				BuildButton buildButton = Instantiate(this.buildButtonPrefab) as BuildButton;
				UnitRecipe recipe = builder.BuildOptions[g];

				GameObject buildOptionWrapper = buildButton.SetButton(builder.Faction, () => this.onSelectBuildOption(recipe, builder), builder.BuildOptions[g]);
				this.buttons.Add(buildOptionWrapper);
				buildOptionWrapper.transform.SetParent(this.buttonsParent, false);
			}
		}
	}

	private void onSelectBuildOption(UnitRecipe recipe, Builder builder)
	{
		Debug.Log("Selected Build Option " + recipe.OutputName);
		builder.SetBuildOption(recipe);
	}

	private void onButtonPress(SelectableActionType actionType)
	{
		Debug.Log("Selected action: " + actionType.ToString());
		onActionSelectedEvent.Raise((int)actionType);
		if(actionType == SelectableActionType.Build)
		{
			this.loadBuildOptions();
		}
	}
}