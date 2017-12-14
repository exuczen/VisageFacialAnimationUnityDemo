using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Maths
{
	public static int GetClosestPowerOf2(int i, bool upper)
	{
		//    int x = 1;
		//    while (x<i) {
		//        x<<=1;
		//    }
		//    return greater ? x : (x>>1);
		int x = i;
		if (x < 0)
			return 0;
		x--;
		x |= x >> 1;
		x |= x >> 2;
		x |= x >> 4;
		x |= x >> 8;
		x |= x >> 16;
		x++;
		return upper ? x : (x >> 1);
	}

	public static bool IsPowerOf2(int i)
	{
		return i > 0 && (i & (i - 1)) == 0;
	}
}

public static class AnimatorUtil
{
	public const string BaseLayerName = "Base Layer";
	public const string LayerSeparator = ".";

	//public static string ToStateName(this TriggerName triggerName)
	//{
	//	string triggerNameString = triggerName.ToString();
	//	return string.Concat(triggerNameString.First().ToString().ToUpper(), triggerNameString.Substring(1));
	//}

	public static int EnumStringToHash(System.Enum enumString)
	{
		return Animator.StringToHash(enumString.ToString());
	}

	public static int FullPathStringToHash(System.Enum subStateName, System.Enum stateName)
	{
		return FullPathStringToHash(subStateName.ToString(), stateName.ToString());
	}

	public static int FullPathStringToHash(System.Enum stateName)
	{
		return FullPathStringToHash(stateName.ToString());
	}

	public static int FullPathStringToHash(params string[] subStatePathComponents)
	{
		return Animator.StringToHash(StateFullPath(subStatePathComponents));
	}

	private static string StateFullPath(params string[] subStatePathComponents)
	{
		string[] fullPathComponents = new string[subStatePathComponents.Length + 1];
		fullPathComponents[0] = BaseLayerName;
		for (int i = 0; i < subStatePathComponents.Length; i++)
		{
			fullPathComponents[i + 1] = subStatePathComponents[i];
		}
		return System.String.Join(LayerSeparator, fullPathComponents);
	}
}

public static class EnumUtil
{
	public static Array GetValues<T>()
	{
		return Enum.GetValues(typeof(T));
	}

	public static IEnumerable<T> GetValuesEnumerable<T>()
	{
		return (T[])Enum.GetValues(typeof(T));
	}
}

public static class CoroutineUtil
{
	public static IEnumerator ActionAfterCustomYieldInstruction(UnityAction action, CustomYieldInstruction yieldInstruction)
	{
		yield return yieldInstruction;
		if (action != null)
		{
			action.Invoke();
		}
	}

	public static IEnumerator ActionAfterTime(UnityAction action, float delayInSeconds)
	{
		yield return new WaitForSeconds(delayInSeconds);
		if (action != null)
		{
			action.Invoke();
		}
	}

	public static IEnumerator ActionAfterFrames(UnityAction action, int framesNumber)
	{
		for (int i = 0; i < framesNumber; i++)
		{
			yield return new WaitForEndOfFrame();
		}
		if (action != null)
		{
			action.Invoke();
		}
	}
}

public static class StringExtensionMethods
{
	public static bool Contains(this string str, string substring, StringComparison comp)
	{
		if (substring == null)
			throw new ArgumentNullException("substring", "substring cannot be null.");
		else if (!Enum.IsDefined(typeof(StringComparison), comp))
			throw new ArgumentException("comp is not a member of StringComparison", "comp");

		return str.IndexOf(substring, comp) >= 0;
	}
}

public static class MonoBehaviourExtensionMethods
{
	public static Coroutine StartCoroutineActionAfterTime(this MonoBehaviour mono, UnityAction action, float delayInSeconds)
	{
		return mono.StartCoroutine(CoroutineUtil.ActionAfterTime(action, delayInSeconds));
	}
	public static Coroutine StartCoroutineActionAfterFrames(this MonoBehaviour mono, UnityAction action, int framesNumber)
	{
		return mono.StartCoroutine(CoroutineUtil.ActionAfterFrames(action, framesNumber));
	}
	public static Coroutine StartCoroutineActionAfterCustomYieldInstruction(this MonoBehaviour mono, UnityAction action, CustomYieldInstruction yieldInstruction)
	{
		return mono.StartCoroutine(CoroutineUtil.ActionAfterCustomYieldInstruction(action, yieldInstruction));
	}

}

public static class RectTransformExtensionMethods
{
	public static void FillParent(this RectTransform rectTransform)
	{
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(1, 1);
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchoredPosition = Vector2.zero;
		rectTransform.offsetMax = rectTransform.offsetMin = Vector2.zero;
	}
	public static void ResetAnchoredPosition(this RectTransform rectTransform)
	{
		rectTransform.anchoredPosition = Vector3.zero;
		rectTransform.offsetMax = rectTransform.offsetMin = Vector2.zero;
	}
}

public static class SkinnedMeshRenderernExtensionMethods
{
	public static void SetBlendShapeWeight(this SkinnedMeshRenderer renderer, String name, float value)
	{
		name = string.Concat(string.Concat(renderer.name, "_blendShape."), name);
		int blendShapeIndex = renderer.sharedMesh.GetBlendShapeIndex(name);

		if (blendShapeIndex < 0)
		{
			name = string.Concat(name, " ");
			blendShapeIndex = renderer.sharedMesh.GetBlendShapeIndex(name);

			if (blendShapeIndex < 0)
				return;
		}

		renderer.SetBlendShapeWeight(blendShapeIndex, value);
	}
	public static void SetBlendShapeWeight(this SkinnedMeshRenderer renderer, System.Enum enumName, float value)
	{
		//Debug.Log("SetBlendShapeWeight: renderer.name=" + renderer.name + " blendshapeName=" + enumName);
		//int testIndex = 7;
		//string blendShapeNameAtTestIndex = renderer.sharedMesh.GetBlendShapeName(testIndex);
		//int blendShapeIndexOfBlendShapeNameAtTestIndex = renderer.sharedMesh.GetBlendShapeIndex(blendShapeNameAtTestIndex);
		//Debug.Log("SetBlendShapeWeight: " + blendShapeIndexOfBlendShapeNameAtTestIndex + "*" + blendShapeNameAtTestIndex + "*" + enumName.ToString() + "*" + blendShapeNameAtTestIndex.Equals(enumName.ToString()) + "*");

		SetBlendShapeWeight(renderer, enumName.ToString(), value);
	}

	public static void PrintBonesInFlatView(this SkinnedMeshRenderer renderer, int? counter = null)
	{
		Transform[] bones = renderer.bones;

		if (counter == null)
			counter = bones.Length;

		string s = string.Concat("PrintBonesInFlatView: ", renderer.name, " bones.Length=", bones.Length, " bonesCounter=", counter, "\n");
		for (int i = 0; i < bones.Length; i++)
		{
			Transform bone = bones[i];
			if (bone != null)
			{
				if (bone.parent == null)
					s = string.Concat(s, bone.name, "\n");
				else
					s = string.Concat(s, bone.name, " ", bone.parent.name, "\n");
			}
			else
			{
				s = string.Concat(s, i, " bone is null\n");
			}
		}
		Debug.Log(s);
	}
}


public static class ListExtensionMethods
{
	public static void RemoveAllNullElements<T>(this List<T> list)
	{
		list.RemoveAll(item => item == null);
	}

	//public static void AddArray<T>(this List<T> list, T[] array)
	//{
	//	for (int i = 0; i < array.Length; i++)
	//	{
	//		list.Add(array[i]);
	//	}
	//}
}

public static class TransformExtensionMethods
{
	public static void DestroyAllChildren(this Transform transform)
	{
		foreach (Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}
	}
}

public static class ScrollRectExtensionMethods
{
	public static void SetHorizontalLayoutView(this RectTransform parent, int visibleColumns, RectTransform slotRect, bool middle)
	{
		ScrollRect scrollRect = parent.GetComponent<ScrollRect>() ?? parent.GetComponentInChildren<ScrollRect>();
		HorizontalLayoutGroup grid = scrollRect.GetComponentInChildren<HorizontalLayoutGroup>();
		RectTransform viewport = scrollRect.viewport;
		RectTransform content = scrollRect.content;

		int childCount = grid.transform.childCount;

		float slotRatio = slotRect.rect.width / slotRect.rect.height;

		float screenRatio = (float)Screen.width / (float)Screen.height;

		if (middle)
		{
			content.anchorMin = new Vector2(0.5f, 1);
			content.anchorMax = new Vector2(0.5f, 1);
			content.pivot = new Vector2(0.5f, 1);
		}
		else
		{
			content.anchorMin = new Vector2(0, 1);
			content.anchorMax = new Vector2(0, 1);
			content.pivot = new Vector2(0, 1);
		}

		float slotWidth = (viewport.rect.width - (grid.padding.left + grid.padding.right + (visibleColumns - 1) * grid.spacing)) / visibleColumns;
		float slotHeight = slotWidth / slotRatio;

		Vector2 slotSize = new Vector2(slotWidth, slotHeight);
		slotRect.sizeDelta = slotSize;

		float gridHeight = grid.padding.top + grid.padding.bottom + slotSize.y;

		parent.sizeDelta = new Vector2(parent.sizeDelta.x, gridHeight);

		foreach (Transform child in content)
		{
			child.GetComponent<RectTransform>().sizeDelta = slotSize;
		}
	}

}

