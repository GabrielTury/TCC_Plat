using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[System.AttributeUsage(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ButtonMonoAttribute : PropertyAttribute
{
    public string ButtonLabel { get; }

    public ButtonMonoAttribute(string buttonLabel = null)
    {
        ButtonLabel = buttonLabel;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonMethodDrawerMono : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector first
        DrawDefaultInspector();

        // Get the target object (the MonoBehaviour)
        MonoBehaviour monoBehaviour = (MonoBehaviour)target;

        // Get all methods from the target object's class
        MethodInfo[] methods = monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        // Loop through each method and check for the ButtonAttribute
        foreach (var method in methods)
        {
            // Get the ButtonAttribute on the method, if any
            var buttonAttribute = (ButtonMonoAttribute)System.Attribute.GetCustomAttribute(method, typeof(ButtonMonoAttribute));

            if (buttonAttribute != null)
            {
                // Use the custom label if provided, otherwise default to the method name
                string buttonLabel = string.IsNullOrEmpty(buttonAttribute.ButtonLabel) ? method.Name : buttonAttribute.ButtonLabel;

                // Draw a button with the specified label
                if (GUILayout.Button(buttonLabel))
                {
                    // Invoke the method when the button is clicked
                    method.Invoke(monoBehaviour, null);
                }
            }
        }
    }
}
#endif
