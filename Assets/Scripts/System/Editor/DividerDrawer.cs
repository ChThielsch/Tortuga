using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DividerAttribute))]
public class DividerDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        if (attribute is DividerAttribute dividerAttribute)
        {
            EditorGUI.LabelField(position, dividerAttribute.header, EditorStyles.boldLabel);
            position.y += EditorGUIUtility.singleLineHeight;

            Rect lineRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 0.5f, position.width, 1f);
            EditorGUI.DrawRect(lineRect, Color.gray);
        }
    }

    public override float GetHeight()
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (attribute is DividerAttribute dividerAttribute && !string.IsNullOrEmpty(dividerAttribute.header))
        {
            height += EditorGUIUtility.singleLineHeight;
        }

        return height;
    }
}