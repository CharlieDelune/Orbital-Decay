using UnityEngine;
using UnityEngine.UI;

/// Auto displays a StringProperty on a Text element
public class UIStringPropertyDisplay : BaseListener<string>
{
    [SerializeField] private Text text;
    [SerializeField] private string preText;
    [SerializeField] private string postText;

    private StringProperty stringProperty;

    void Start()
    {
        this.stringProperty = (StringProperty) this.Target;
        this.updateText();
    }

    public override void OnRaise(string value)
    {
        this.updateText();
    }

    private void updateText()
    {
        this.text.text = preText + this.stringProperty.Value + postText;
    }
}
