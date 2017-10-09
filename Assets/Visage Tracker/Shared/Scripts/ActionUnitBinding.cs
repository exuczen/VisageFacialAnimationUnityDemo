using UnityEngine;
using System.Collections;
using System;

public class ActionUnitBinding : MonoBehaviour
{
    public bool Initialized
    {
        get { return data != null; }
    }

    public VisageTracker Tracker;
    public string Name;
    public string ActionUnitName;
    public bool Inverted;
    public Vector2 Limits;
    public float Weight = 1f;

    public float Value;

    [HideInInspector]
    public float NormalizedValue;

    public int FilterWindowSize = 4;
    private float[] normalizedValueHistory;
    public float FilterConstant = 0.3f;

    public float FilteredValue;

    public ActionUnitBindingTarget[] Targets;

    private ActionUnitData data;

    void Start()
    {
        normalizedValueHistory = new float[FilterWindowSize];
    }

    void Update()
    {
        if (Tracker == null)
            return;

        if (!Initialized)
        {
            data = Tracker.GetActionUnit(ActionUnitName);
        }
        else
        {
            Value = data.Value;
            NormalizedValue = (data.Value - Limits.x) / (Limits.y - Limits.x);
            NormalizedValue = Mathf.Clamp01(NormalizedValue);
            if (Inverted)
                NormalizedValue = 1f - NormalizedValue;

            // push back normalized history
            for (int i = 1; i < FilterWindowSize; i++)
                normalizedValueHistory[i - 1] = normalizedValueHistory[i];

            // add normalized value to history
            normalizedValueHistory[FilterWindowSize - 1] = NormalizedValue;

            // filter value
            FilteredValue = Filter(NormalizedValue);

            // apply to all targets
            foreach (ActionUnitBindingTarget target in Targets)
            {
                if (target.BlendshapeIndex >= 0 && target.Weight >= 0f)
                    target.Renderer.SetBlendShapeWeight(target.BlendshapeIndex, FilteredValue * Weight * target.Weight * 100f);
            }
        }
    }

    private float Filter(float value)
    {
        // get maximum variation
        float maxVariation = 0f;
        for (int i = 0; i < FilterWindowSize - 1; i++)
            maxVariation = Mathf.Max(maxVariation, Mathf.Abs(value - normalizedValueHistory[i]));

        // get weights
        float[] weights = new float[FilterWindowSize];
        for (int i = 0; i < FilterWindowSize; i++)
        {
            weights[i] = Mathf.Exp(-i * FilterConstant * maxVariation);
        }

        // get sum of weights
        float weightSum = 0f;
        for (int i = 0; i < FilterWindowSize; i++)
            weightSum += weights[i];

        // filter value
        float filteredValue = 0f;
        for (int i = 0; i < FilterWindowSize; i++)
        {
            filteredValue += weights[i] * normalizedValueHistory[i] / weightSum;
        }

        return filteredValue;
    }

    public static void SetupBinding(VisageTracker tracker, TextAsset configuration)
    {
        string text = configuration.text;
        string[] lines = text.Split(new [] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            // skip comments
            if (line.StartsWith("#"))
                continue;

            string[] values = line.Split(new [] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string[] trimmedValues = new string[8];
            for (int i = 0; i < Mathf.Min(values.Length, 8); i++)
            {
                // trim values
                trimmedValues[i] = values[i].Trim();
            }

            // parse au name
            string au = trimmedValues[0];

            // parse blendshape identifier
            string blendshape = trimmedValues[1];
            string[] blendShapeParts = blendshape.Split(':');
            if (blendShapeParts.Length < 2)
            {
                Debug.LogError("Invalid blendshape_indentifier value in configuration '" + configuration.name + "'.", tracker);
                return;
            }

            string blendshapeObjectName = blendShapeParts[0];
            int blendshapeIndex = 0;
            if (!int.TryParse(blendShapeParts[1], out blendshapeIndex))
            {
                Debug.LogError("Invalid blendshape_indentifier value in configuration '" + configuration.name + "'.", tracker);
                return;
            }

            GameObject target = GameObject.Find(blendshapeObjectName);
            if (target == null || target.GetComponent<SkinnedMeshRenderer>() == null)
            {
				Debug.LogError(target==null);
                Debug.LogError("No valid blendshape target named '" + blendshapeObjectName + "' defined in configuration: '" + configuration.name + "'.", tracker);
                return;
            }
            
            // parse min limit
            float min = -1f;
            if (!float.TryParse(trimmedValues[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out min))
            {
                Debug.LogError("Invalid min_limit value in binding configuration '" + configuration.name + "'.", tracker);
                return;
            }

            // parse max limit
            float max = 1f;
            if (!float.TryParse(trimmedValues[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out max))
            {
                Debug.LogError("Invalid max_limit value in binding configuration '" + configuration.name + "'.", tracker);
                return;
            }

            // parse inverted
            bool inverted = false;
            if (!string.IsNullOrEmpty(trimmedValues[4]))
                inverted = trimmedValues[4] == "1";

            // parse weight
            float weight = 1f;
            if (!string.IsNullOrEmpty(trimmedValues[5]))
            {
                if (!float.TryParse(trimmedValues[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out weight))
                {
                    Debug.LogError("Invalid weight value in binding configuration '" + configuration.name + "'.", tracker);
                    return;
                }
            }
            
            // parse filter window
            int filterWindow = 6;
            if (!string.IsNullOrEmpty(trimmedValues[6]))
            {
                if (!int.TryParse(trimmedValues[6], out filterWindow) || filterWindow < 0 || filterWindow > 16)
                {
                    Debug.LogError("Invalid filter_window value in binding configuration '" + configuration.name + "'.", tracker);
                    return;
                }
            }

            // parse filter amount
            float filterAmount = 0.3f;
            if (!string.IsNullOrEmpty(trimmedValues[7]))
            {
                if (!float.TryParse(trimmedValues[7], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out filterAmount))
                {
                    Debug.LogError("Invalid filter_amount value in binding configuration '" + configuration.name + "'.", tracker);
                    return;
                }
            }

            // add new binding
            ActionUnitBinding binding = tracker.gameObject.AddComponent<ActionUnitBinding>();
            binding.Name = au + " -> " + blendshape;
            binding.Tracker = tracker;
            binding.ActionUnitName = au;
            binding.Limits = new Vector2(min, max);
            binding.Inverted = inverted;
            binding.Weight = weight;
            binding.FilterWindowSize = filterWindow;
            binding.FilterConstant = filterAmount;
            binding.Targets = new ActionUnitBindingTarget[1];
            binding.Targets[0] = new ActionUnitBindingTarget();
            binding.Targets[0].Renderer = GameObject.Find(blendshapeObjectName).GetComponent<SkinnedMeshRenderer>();
            binding.Targets[0].BlendshapeIndex = blendshapeIndex;
            binding.Targets[0].Weight = 1f;
        }
    }
}
