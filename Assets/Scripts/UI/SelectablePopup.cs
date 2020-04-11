using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectablePopup : MonoBehaviour
{

	private SelectableActionType? selectedAction;
	private string selectedUnitToBuild;

	private Selectable parentSelectable;

	[SerializeField] private Button buttonPrefab;
	[SerializeField] private Transform buttonsParent;
	private List<Button> buttons = new List<Button>();

	[SerializeField] private IntGameEvent onActionSelectedEvent;

	/// Loads the popup based on the selectable
	public void LoadPopup(Selectable selectable)
	{
		this.selectedAction = null;
		this.selectedUnitToBuild = "";
		this.parentSelectable = selectable;
		List<SelectableActionType> validActionTypes = selectable.GetValidActionTypes();
		for(int i = 0; i < this.buttons.Count; i++)
		{
			Destroy(this.buttons[i].gameObject);
		}
		this.buttons.Clear();
		for(int i = 0; i < validActionTypes.Count; i++)
		{
			int g = i;
			this.buttons.Add(Instantiate(this.buttonPrefab));
			this.buttons[i].transform.SetParent(this.buttonsParent, false);
			this.buttons[i].GetComponentsInChildren<Text>()[0].text = validActionTypes[i].ToString();
			this.buttons[i].onClick.AddListener(()=>this.onButtonPress(validActionTypes[g]));
		}
	}

	private void loadBuildOptions()
	{
		///	not implemented
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