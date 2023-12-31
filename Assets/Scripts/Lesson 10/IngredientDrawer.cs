using UnityEditor;
using UnityEngine;

namespace Lesson_10
{
    [CustomPropertyDrawer(typeof(Ingredient))]
    
    public class IngredientDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            var amountRect = new Rect(position.x, position.y, 30, position.height);
            var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
            var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);
            
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("Amount"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("Unit"), GUIContent.none);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"), GUIContent.none);
            
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}