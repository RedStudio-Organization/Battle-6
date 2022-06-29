using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RedStudio
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    class SortingLayerAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // One line of  oxygen free code.
            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, SortingLayer.layers.Select(i => i.name).ToArray() );
        }
    }
#endif
}