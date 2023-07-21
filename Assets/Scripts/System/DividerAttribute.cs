using UnityEngine;

public class DividerAttribute : PropertyAttribute
{
    public string header;
    public Color color = Color.gray; // Default color is gray.

    public DividerAttribute(string header = "")
    {
        this.header = header;
    }
}