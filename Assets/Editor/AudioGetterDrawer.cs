using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Custom property drawer for the AudioGetter class
[CustomPropertyDrawer(typeof(AudioGetter))]
public class AudioGetterDrawer : PropertyDrawer
{
    // Override the OnGUI method to customize how the property is displayed in the Inspector
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Remove the default property label and replace it with a custom label
        position = EditorGUI.PrefixLabel(position, label);

        // Display a popup field to select an audio clip from the AudioLibrary
        // The selected audio clip is determined by the "id" property of the AudioGetter
        property.FindPropertyRelative("id").intValue = EditorGUI.Popup(position, property.FindPropertyRelative("id").intValue, AudioLibrary.audioNamesList.ToArray());

        // Optionally can update the "audioName" property based on the selected "id" here
        // property.FindPropertyRelative("audioName").stringValue = AudioLibrary.audioNamesList[property.FindPropertyRelative("id").intValue];
    }
}
