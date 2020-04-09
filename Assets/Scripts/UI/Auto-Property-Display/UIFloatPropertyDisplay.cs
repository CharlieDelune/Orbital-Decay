using UnityEngine;
using UnityEngine.UI;

/// Auto displays a FloatProperty on a Text element
public class UIFloatPropertyDisplay : BaseListener<float>
{
    [SerializeField] private Text text;
    [SerializeField] private string preText;
    [SerializeField] private string postText;

    private FloatProperty floatProperty;

    void Start()
    {
        this.floatProperty = (FloatProperty) this.Target;
        this.updateText();
    }

    public override void OnRaise(float value)
    {
        this.updateText();
    }

    private void updateText()
    {
        this.text.text = preText + this.floatProperty.Value + postText;
    }
}
