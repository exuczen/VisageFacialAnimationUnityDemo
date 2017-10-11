using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ActionUnitBinding
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

    public ActionUnitBinding()
    {
        normalizedValueHistory = new float[FilterWindowSize];
    }

	public void UpdateAction()
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


}
