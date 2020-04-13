using UnityEngine;
using UnityEngine.UI;

/// Auto displays an IntProperty on a Text element
public class UIIntPropertyDisplay : BaseListener<int>
{
    [SerializeField] private Text text;
    [SerializeField] private string preText;
    [SerializeField] private string postText;

    private IntProperty intProperty;

    void Start()
    {
        this.intProperty = (IntProperty) this.Target;
        this.updateText();
    }

    public override void OnRaise(int value)
    {
        this.updateText();
    }

    private void updateText()
    {
        this.text.text = this.preText + this.intProperty.Value + this.postText;
    }
}
