using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Binder_ImageAndText : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    Text text;

    public Image Image => image;
    public Text Text => text;
    public void Set(Sprite sprite, string str)
    {
        image.sprite = sprite;
        text.text = str;
    }
    public void Set(Color color, string str)
    {
        image.color = color;
        text.text = str;
    }
    public void Set(Sprite sprite, Color color, string str)
    {
        image.sprite = sprite;
        image.color = color;
        text.text = str;
    }
}
