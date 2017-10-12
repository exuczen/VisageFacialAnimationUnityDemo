﻿using UnityEngine;
using System.Collections;
using System;

namespace Visage.FaceTracking
{
	[Serializable]
	public class ActionUnitBindingTarget
	{
		[HideInInspector]
		public string Name;

		public SkinnedMeshRenderer Renderer;
		public int BlendshapeIndex;
		public float Weight = 1f;
	} 
}
