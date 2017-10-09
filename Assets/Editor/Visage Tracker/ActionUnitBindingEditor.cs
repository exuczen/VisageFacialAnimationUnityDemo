using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ActionUnitBinding))]
public class ActionUnitBindingEditor : Editor 
{
    ActionUnitBinding binding;

    public override void OnInspectorGUI()
    {
        binding = (ActionUnitBinding)target;

        // name and action unit
        EditorGUILayout.Separator();
        binding.Tracker = EditorGUILayout.ObjectField("Tracker", binding.Tracker, typeof(VisageTracker), true) as VisageTracker;
        EditorGUILayout.LabelField("Binding", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.whiteLargeLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(150f));
        EditorGUILayout.LabelField("Action unit", GUILayout.MaxWidth(150f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        binding.Name = EditorGUILayout.TextField(binding.Name, GUILayout.MaxWidth(150f));
        binding.ActionUnitName = EditorGUILayout.TextField(binding.ActionUnitName, GUILayout.MaxWidth(150f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Weight: " + binding.Weight.ToString("0.00"), GUILayout.MaxWidth(150f));
        binding.Weight = EditorGUILayout.Slider(binding.Weight, 0f, 1f, GUILayout.MaxWidth(150f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        // limits
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Limits", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.whiteLargeLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Raw value: " + binding.Value.ToString("0.00"), GUILayout.MaxWidth(150f));
        EditorGUILayout.LabelField("Limits: " + binding.Limits.x.ToString("0.00") + ", " + binding.Limits.y.ToString("0.00"), GUILayout.MaxWidth(150f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        binding.Inverted = EditorGUILayout.ToggleLeft("Inverted", binding.Inverted, GUILayout.MaxWidth(150f));
        EditorGUILayout.MinMaxSlider(ref binding.Limits.x, ref binding.Limits.y, -1f, 1f, GUILayout.MaxWidth(150f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Separator();

        // filter
        EditorGUILayout.LabelField("Filter", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.whiteLargeLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filtered value: " + binding.FilteredValue.ToString("0.00"), GUILayout.MaxWidth(150f));
        binding.FilterWindowSize = EditorGUILayout.IntField("Filter window", binding.FilterWindowSize, GUILayout.MaxWidth(150f));
        EditorGUILayout.EndHorizontal();
        binding.FilterConstant = EditorGUILayout.Slider("Filter amount", binding.FilterConstant, 0f, 1f, GUILayout.MaxWidth(305f));
        EditorGUILayout.EndVertical();
        EditorGUILayout.Separator();
        
        // targets
        EditorGUILayout.BeginVertical(EditorStyles.whiteLargeLabel);
        serializedObject.Update();
        SerializedProperty prop = serializedObject.FindProperty("Targets");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(prop, true, GUILayout.MaxWidth(305f));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            if (binding.Targets != null)
            {
                foreach (ActionUnitBindingTarget t in binding.Targets)
                {
                    t.Name = "None";
                    if (t.Renderer != null)
                        t.Name = t.Renderer.name;
                }
            }
        }
        EditorGUILayout.Separator();
        EditorGUILayout.EndVertical();
    }
}
