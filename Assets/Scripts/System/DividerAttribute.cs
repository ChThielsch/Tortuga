using UnityEngine;

public class DividerAttribute : PropertyAttribute
{
    public string header;

    public DividerAttribute(string header = "")
    {
        this.header = header;
    }
}