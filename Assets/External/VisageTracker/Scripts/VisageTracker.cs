using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Visage.FaceTracking
{
	/** Enum used for tracking status.
	 */
	public enum TrackStatus
	{
		Off = 0,
		Ok = 1,
		Recovering = 2,
		Init = 3
	}

	/** Class that implements the behaviour of tracking application.
	 * 
	 * This is the core class that shows how to use visage|SDK capabilities in Unity. It connects with visage|SDK through calls to native methods that are implemented in VisageTrackerUnityPlugin.
	 * It uses tracking data to transform objects that are attached to it in ControllableObjects list.
	 */
	public class VisageTracker : MonoBehaviour
	{
		#region Properties

		/** Tracker config file in editor.
		 */
#if !UNITY_EDITOR
    [HideInInspector]
#endif
		public string ConfigFileEditor;

		/** Tracker config file name on standalone platforms.
		 */
#if !UNITY_STANDALONE_WIN
		[HideInInspector]
#endif
		public string ConfigFileStandalone;

		/** Tracker config file on iOS.
		 */
#if !UNITY_IPHONE
		[HideInInspector]
#endif
		public string ConfigFileIOS;

		/** Tracker config file on Android.
		 */
#if !UNITY_ANDROID
    [HideInInspector]
#endif
		public string ConfigFileAndroid;

		/** Tracker config file on Mac.
		 */
#if !UNITY_STANDALONE_OSX
		[HideInInspector]
#endif
		public string ConfigFileOSX;

#if UNITY_ANDROID && !UNITY_EDITOR
	private AndroidJavaObject androidCameraActivity;
#endif

#if !UNITY_STANDALONE_WIN || !UNITY_EDITOR
		[HideInInspector]
#endif
		public string licenseFileFolder;

#if !UNITY_STANDALONE_OSX && !UNITY_ANDROID && !UNITY_IPHONE
    [HideInInspector]
#endif
		public string licenseFileName;


		/** Binding configurations imported as text assets.
		 */
		//public TextAsset[] BindingConfigurations;

		/** Tracker status.
		*/
		public TrackStatus TrackerStatus = TrackStatus.Off;

		/** Flag to check if tracker is active or not.
		 */
		public bool TrackerActive { get { return TrackerStatus != TrackStatus.Off; } }

		/** Head traslation data from the tracker.
		 */
		public Vector3 Translation;

		/** Head rotation data from the tracker.
		 */
		public Vector3 Rotation;

		/** Gaze direction data from the tracker.
		 */
		public Vector2 GazeDirection;

		/** Focal length of the camera, used to set scene parameters.
		*/
		public float Focus;

		/** Camera frame width.*/
		[HideInInspector]
		public int ImageWidth;

		/** Camera frame heigth.*/
		[HideInInspector]
		public int ImageHeight;

		/** Width of the texture for displaying camera frame
		 */
		[HideInInspector]
		public int TexWidth;

		/** Height of the texture for displaying camera frame
		 */
		[HideInInspector]
		public int TexHeight;

		/** Texture for video display and 3D face model.
		*/
		public Texture Frame = null;
		private Color32[] texturePixels;
		private GCHandle texturePixelsHandle;
		public WebCamTexture CameraTexture = null;

		/** Sets whether the action unit data is retrieved.
		 */
		public bool ActionUnitsEnabled = true;

		/** Number of defined action units.
		 */
		public int ActionUnitCount;

		/** Array of all action units.
		 */
		public ActionUnitData[] ActionUnits;

		private string currentDir;
		private string visageDir;

		private Color32[] cameraImageData;

		private int DisplayStatus = 0;
		private bool isTracking = false;

		[Header("Buttons")]
		[SerializeField]
		private Canvas canvas;
		[SerializeField]
		private Button trackButton;
		[SerializeField]
		private Button switchCameraButton;
		[SerializeField]
		private Toggle showMaskToggle;
		[SerializeField]
		private Image trackStartImage;
		[SerializeField]
		private Image trackStopImage;
		//public Texture2D imageSwitchCam;
		//public Texture2D imageStartTracking;
		//public Texture2D imageStopTracking;
		//private bool trackButton;
		//private bool stopTrackButton;
		//private bool normalButton;
		//private bool switchCamButton;
		//private bool maskButton;
		//private bool mask = false;
		//private GUIStyle startTrackingStyle = null;
		//private GUIStyle stopTrackingStyle = null;
		//private GUIStyle customButtonStyle = null;
		//private GUIStyle switchCamButtonStyle = null;
		//GUIContent contentSwitchCam = new GUIContent();
		//GUIContent contentStartTracking = new GUIContent();
		//GUIContent contentStopTracking = new GUIContent();

		private bool AppStarted = false;

		[Header("Camera settings")]
		public int Orientation = 0;
		private int currentOrientation = 0;
		public int isMirrored = 1;
		public int device = 0;
		private int currentDevice = 0;
		public int defaultCameraWidth = -1;
		public int defaultCameraHeight = -1;
		public bool setCameraFieldOfView;

		[Header("GUI")]
		[SerializeField]
		private bool showGUI;
		[SerializeField]
		private VisageVideoPreview videoPreview;

		private AndroidJavaClass unity;

		#endregion

		private static VisageTracker instance;

		public static VisageTracker Instance {
			get {
				if (instance == null)
					instance = FindObjectOfType<VisageTracker>();
				return instance;
			}
		}

		//private List<FaceActionUnitModel> modelList;
		//public void RemoveModel(FaceActionUnitModel model)
		//{
		//	if (!modelList.Contains(model))
		//		modelList.Remove(model);
		//}
		//public void AddModel(FaceActionUnitModel model)
		//{
		//	if (!modelList.Contains(model))
		//		modelList.Add(model);
		//}

		private string configFilePath;

		private string licenseFilePath;

		/** This method is called before any of the Update methods are called the first time.
		 * 
		 * It initializes helper variables and the tracker.
		 */
		private void Awake()
		{
			Debug.Log("<color=blue>VisageTracker.Awake</color>");
			//contentSwitchCam.image = (Texture2D)imageSwitchCam;
			//contentStartTracking.image = (Texture2D)imageStartTracking;
			//contentStopTracking.image = (Texture2D)imageStopTracking;
			//startTrackingStyle = new GUIStyle();
			//startTrackingStyle.normal.background = (Texture2D)contentStartTracking.image;
			//startTrackingStyle.active.background = (Texture2D)contentStartTracking.image;
			//startTrackingStyle.hover.background = (Texture2D)contentStartTracking.image;
			//stopTrackingStyle = new GUIStyle();
			//stopTrackingStyle.normal.background = (Texture2D)contentStopTracking.image;
			//stopTrackingStyle.active.background = (Texture2D)contentStopTracking.image;
			//stopTrackingStyle.hover.background = (Texture2D)contentStopTracking.image;
			//switchCamButtonStyle = new GUIStyle();
			//switchCamButtonStyle.normal.background = (Texture2D)contentSwitchCam.image;
			//switchCamButtonStyle.active.background = (Texture2D)contentSwitchCam.image;
			//switchCamButtonStyle.hover.background = (Texture2D)contentSwitchCam.image;

			Translation = new Vector3(0, 0, 0);
			Rotation = new Vector3(0, 0, 0);

			// choose config file
			this.configFilePath = Application.streamingAssetsPath + "/" + ConfigFileStandalone;
			this.licenseFilePath = Application.streamingAssetsPath + "/" + licenseFileFolder;

			switch (Application.platform)
			{

				case RuntimePlatform.IPhonePlayer:
					configFilePath = "Data/Raw/VisageTracker/" + ConfigFileIOS;
					licenseFilePath = "Data/Raw/VisageTracker/" + licenseFileName;
					break;
				case RuntimePlatform.Android:
					configFilePath = Application.persistentDataPath + "/" + ConfigFileAndroid;
					licenseFilePath = Application.persistentDataPath + "/" + licenseFileName;
					break;
				case RuntimePlatform.OSXPlayer:
					configFilePath = Application.dataPath + "/Resources/Data/StreamingAssets/VisageTracker/" + ConfigFileOSX;
					licenseFilePath = Application.dataPath + "/Resources/Data/StreamingAssets/VisageTracker/" + licenseFileName;
					break;
				case RuntimePlatform.OSXEditor:
					configFilePath = Application.dataPath + "/StreamingAssets/VisageTracker/" + ConfigFileOSX;
					licenseFilePath = Application.dataPath + "/StreamingAssets/VisageTracker/" + licenseFileName;
					break;
				case RuntimePlatform.WindowsEditor:
					configFilePath = Application.streamingAssetsPath + "/" + ConfigFileEditor;
					licenseFilePath = Application.streamingAssetsPath + "/" + licenseFileFolder;
					break;
			}

			// initialize tracker
			InitializeTracker(configFilePath, licenseFilePath);
		}

		private void Start()
		{
			trackButton.onClick.AddListener(OnTrackButtonClick);
			switchCameraButton.onClick.AddListener(OnSwitchCameraButtonClick);
			showMaskToggle.onValueChanged.AddListener(OnMaskToggleChange);
			canvas.gameObject.SetActive(showGUI);
		}

		public void OnTrackButtonClick()
		{
			//if (!isTracking)
			//{
			//	if (ImageWidth < ImageHeight)
			//		trackButton = GUI.Button(new Rect(0, Screen.height - Screen.width / 2, Screen.width / 8, Screen.width / 8), " ", startTrackingStyle);
			//	else
			//		trackButton = GUI.Button(new Rect(0, Screen.height - Screen.height / 2, Screen.height / 8, Screen.height / 8), " ", startTrackingStyle);
			//	if (trackButton)
			//		isTracking = true;
			//}
			//if (isTracking)
			//{
			//	if (ImageWidth < ImageHeight)
			//		stopTrackButton = GUI.Button(new Rect(0, Screen.height - Screen.width / 2, Screen.width / 8, Screen.width / 8), " ", stopTrackingStyle);
			//	else
			//		stopTrackButton = GUI.Button(new Rect(0, Screen.height - Screen.height / 2, Screen.height / 8, Screen.height / 8), " ", stopTrackingStyle);
			//	if (stopTrackButton)
			//	{
			//		isTracking = false;
			//		TrackerStatus = TrackStatus.Off;
			//	}
			//}
			isTracking = !isTracking;
			trackStartImage.gameObject.SetActive(!isTracking);
			trackStopImage.gameObject.SetActive(isTracking);
			if (!isTracking)
				TrackerStatus = TrackStatus.Off;
		}

		public void OnSwitchCameraButtonClick()
		{
			//if (ImageWidth < ImageHeight)
			//	switchCamButton = GUI.Button(new Rect(Screen.width - Screen.width / 6, Screen.height - Screen.width / 2, Screen.width / 6, Screen.width / 6), " ", switchCamButtonStyle);
			//else
			//	switchCamButton = GUI.Button(new Rect(Screen.width - Screen.height / 6, Screen.height - Screen.height / 2, Screen.height / 6, Screen.height / 6), " ", switchCamButtonStyle);
			//if (switchCamButton)
			//	currentDevice = (currentDevice == 1) ? 0 : 1;
			currentDevice = (currentDevice == 1) ? 0 : 1;
		}

		public void OnMaskToggleChange(bool enabled)
		{
			//if (mask)
			//{
			//	if (ImageWidth < ImageHeight)
			//		normalButton = GUI.Button(new Rect(Screen.width - Screen.width / 30 - Screen.width / 3, Screen.height / 6, Screen.width / 3, Screen.height / 12), "Normal");
			//	else
			//		normalButton = GUI.Button(new Rect(Screen.width - Screen.height / 30 - Screen.height / 3, Screen.width / 6, Screen.height / 3, Screen.width / 12), "Normal");
			//	if (normalButton)
			//	{

			//		videoPreview.DesiredScreenWidth = 1f;
			//		videoPreview.PreviewResults = true;
			//		mask = false;
			//	}
			//}
			//else
			//{
			//	if (ImageWidth < ImageHeight)
			//		maskButton = GUI.Button(new Rect(Screen.width - Screen.width / 30 - Screen.width / 3, Screen.height / 6, Screen.width / 3, Screen.height / 12), "Mask");
			//	else
			//		maskButton = GUI.Button(new Rect(Screen.width - Screen.height / 30 - Screen.height / 3, Screen.width / 6, Screen.height / 3, Screen.width / 12), "Mask");
			//	if (maskButton)
			//	{
			//		videoPreview.DesiredScreenWidth = 1f;
			//		videoPreview.PreviewResults = false;
			//		mask = true;
			//	}
			//}
			videoPreview.PreviewResults = enabled;
		}

		private void OnEnable()
		{
			Debug.Log("<color=blue>VisageTracker.OnEnable</color>");

			// clear all existing binding components
			//ActionUnitBinding[] existingBindings = GetComponents<ActionUnitBinding> ();
			//foreach (ActionUnitBinding binding in existingBindings)
			//	Destroy (binding);

			// setup new bindings
			//foreach (TextAsset configuration in BindingConfigurations)
			//	ActionUnitBinding.SetupBinding(this, configuration);

			//check orientation and start camera
			Orientation = getDeviceOrientation();
			OpenCamera(Orientation, device, defaultCameraWidth, defaultCameraHeight, isMirrored);

			isTracking = true;
			trackStartImage.gameObject.SetActive(false);
			trackStopImage.gameObject.SetActive(true);

			if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)
				Debug.Log("VisageTracker.Awake: Notice: if graphics API is set to OpenGLCore, the texture might not get properly updated.");
		}

		private void OnDisable()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.androidCameraActivity.Call("closeCamera");
#else
			VisageTrackerNative._closeCamera();
#endif
			isTracking = false;
		}


		/** This method is called every frame.
		 * 
		 * It fetches the tracking data from the tracker and transforms controlled objects accordingly. 
		 * It also fetches vertex, triangle and texture coordinate data to generate 3D face model from the tracker.
		 * And lastly it refreshes the video frame texture with the new frame data.
		 * 
		 */
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
#if (UNITY_IPHONE) && UNITY_EDITOR
		// no tracking on ios while in editor
		return;
#endif
			// update tracker status, translation and rotation
			int trackStatus;

			if (isTracking)
			{
				currentOrientation = getDeviceOrientation();

				// check if orientation or camera device changed
				if (currentOrientation != Orientation || currentDevice != device)
				{
					OpenCamera(currentOrientation, currentDevice, defaultCameraWidth, defaultCameraHeight, isMirrored);
					Orientation = currentOrientation;
					device = currentDevice;
					Frame = null;
				}

				// grab new frame and start face tracking on it
				VisageTrackerNative._grabFrame();
				trackStatus = VisageTrackerNative._track();
				VisageTrackerNative._get3DData(out Translation.x, out Translation.y, out Translation.z, out Rotation.x, out Rotation.y, out Rotation.z);

				TrackerStatus = (TrackStatus)trackStatus;
				//	isTracking = TrackerStatus != 0;
			}

			// exit if no tracking
			if (TrackerStatus == TrackStatus.Off)
				return;

			// set correct camera field of view
			GetCameraInfo();

#if UNITY_ANDROID && !UNITY_EDITOR
		//waiting to get information about frame width and height
        if (ImageWidth == 0 || ImageHeight == 0)
            return;
#endif

			// update gaze direction
			float[] gazeDirection = new float[2];
			VisageTrackerNative._getGazeDirection(gazeDirection);
			GazeDirection = new Vector2(gazeDirection[0] * Mathf.Rad2Deg, gazeDirection[1] * Mathf.Rad2Deg);

			// get image
			RefreshImage();

			// get action units
			if (ActionUnitsEnabled)
				RefreshActionUnits();
		}


		void OnDestroy()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			this.androidCameraActivity.Call("closeCamera");
#else
			VisageTrackerNative._closeCamera();
#endif
		}


		/** This method initializes the tracker.
		 */
		bool InitializeTracker(string config, string license)
		{
			Debug.Log("VisageTracker: Initializing tracker with config: '" + config + "'");

#if (UNITY_IPHONE) && UNITY_EDITOR
		return false;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
		// initialize visage vision
		VisageTrackerNative._loadVisageVision();
		Unzip();
		
		unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		this.androidCameraActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
#endif
			// initialize tracker
			VisageTrackerNative._initTracker(config, license);
			return true;
		}


		void GetCameraInfo()
		{
#if (UNITY_IPHONE) && UNITY_EDITOR
			return;
#endif
			VisageTrackerNative._getCameraInfo(out Focus, out ImageWidth, out ImageHeight);
#if UNITY_ANDROID && !UNITY_EDITOR
			if (ImageWidth == 0 || ImageHeight == 0)
				return;
#endif
			if (setCameraFieldOfView)
			{
				// set camera field of view
				float aspect = ImageWidth / (float)ImageHeight;
				float yRange = (ImageWidth > ImageHeight) ? 1.0f : 1.0f / aspect;
				Camera.main.fieldOfView = Mathf.Rad2Deg * 2.0f * Mathf.Atan(yRange / Focus);
			}
		}

		/** Returns the action unit with the specified name.
		*/
		public ActionUnitData GetActionUnit(string name)
		{
			foreach (ActionUnitData data in ActionUnits)
			{


				if (data.Name == name)
					return data;
			}

			return null;
		}

		private void RefreshActionUnits()
		{
			// initialize action units
			if (ActionUnitCount == 0)
			{
				ActionUnitCount = VisageTrackerNative._getActionUnitCount();
				ActionUnits = new ActionUnitData[ActionUnitCount];
				for (int actionUnitIndex = 0; actionUnitIndex < ActionUnitCount; actionUnitIndex++)
				{
					string name = Marshal.PtrToStringAnsi(VisageTrackerNative._getActionUnitName(actionUnitIndex));
					bool used = VisageTrackerNative._getActionUnitUsed(actionUnitIndex);
					ActionUnitData actionUnitData = new ActionUnitData(actionUnitIndex, name, used);
					ActionUnits[actionUnitIndex] = actionUnitData;

				}
			}

			// get action unit values
			if (ActionUnitCount > 0)
			{
				float[] values = new float[ActionUnitCount];
				VisageTrackerNative._getActionUnitValues(values);
				for (int actionUnitIndex = 0; actionUnitIndex < ActionUnitCount; actionUnitIndex++)
				{
					ActionUnits[actionUnitIndex].Value = values[actionUnitIndex];
				}
			}
		}

		void RefreshImage()
		{
#if (UNITY_IPHONE) && UNITY_EDITOR
		return;
#endif

			// create texture
			if (Frame == null && isTracking)
			{
				TexWidth = Convert.ToInt32(Math.Pow(2.0, Math.Ceiling(Math.Log(ImageWidth) / Math.Log(2.0))));
				TexHeight = Convert.ToInt32(Math.Pow(2.0, Math.Ceiling(Math.Log(ImageHeight) / Math.Log(2.0))));
#if UNITY_ANDROID && !UNITY_EDITOR
			Frame = new Texture2D (TexWidth, TexHeight, TextureFormat.RGB24, false);
#else
				Frame = new Texture2D(TexWidth, TexHeight, TextureFormat.RGBA32, false);
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
				// "pin" the pixel array in memory, so we can pass direct pointer to it's data to the plugin,
				// without costly marshaling of array of structures.
				texturePixels = ((Texture2D)Frame).GetPixels32(0);
				texturePixelsHandle = GCHandle.Alloc(texturePixels, GCHandleType.Pinned);
#endif
			}
			if (Frame != null && isTracking)
			{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
				// send memory address of textures' pixel data to VisageTrackerUnityPlugin
				VisageTrackerNative._setFrameData(texturePixelsHandle.AddrOfPinnedObject());
				((Texture2D)Frame).SetPixels32(texturePixels, 0);
				((Texture2D)Frame).Apply();

#elif UNITY_IPHONE || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_ANDROID
            if (SystemInfo.graphicsDeviceVersion.StartsWith ("Metal"))
            	VisageTrackerNative._bindTextureMetal (Frame.GetNativeTexturePtr ());
            else
            VisageTrackerNative._bindTexture ((int)Frame.GetNativeTexturePtr ());
#endif
			}
		}

		// if width and height are -1, values will be set internally
		void OpenCamera(int orientation, int currDevice, int width, int height, int mirrored)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (device == currDevice && AppStarted)
			return;
		//camera needs to be opened on main thread
        this.androidCameraActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            this.androidCameraActivity.Call("closeCamera");
            this.androidCameraActivity.Call("GrabFromCamera", width, height, currDevice);
        }));
        AppStarted = true;
#else
			VisageTrackerNative._openCamera(orientation, currDevice, width, height, mirrored);
#endif

		}


		/** Returns current device orientation.
		 */
		int getDeviceOrientation()
		{
			int devOrientation;

			//Device orientation is obtained in AndroidCameraPlugin so we only need information about whether orientation is changed
#if UNITY_ANDROID && !UNITY_EDITOR
		int oldWidth = ImageWidth;
		int oldHeight = ImageHeight;
		VisageTrackerNative._getCameraInfo(out Focus, out ImageWidth, out ImageHeight);
		
		if ((oldWidth!=ImageWidth || oldHeight!=ImageHeight) && ImageWidth != 0 && ImageHeight !=0 && oldWidth != 0 && oldHeight !=0 )
			devOrientation = (Orientation ==1) ? 0:1;
		else
			devOrientation = Orientation;
#else

			if (Input.deviceOrientation == DeviceOrientation.Portrait)
				devOrientation = 0;
			else if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
				devOrientation = 2;
			else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
				devOrientation = 3;
			else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
				devOrientation = 1;
			else if (Input.deviceOrientation == DeviceOrientation.FaceUp)
				devOrientation = Orientation;
			else
				devOrientation = 0;
#endif

			return devOrientation;

		}

		void Unzip()
		{
			string[] pathsNeeded = {
			"candide3.fdp",
			"candide3.wfm",
			"FacialFeaturesTracker-High.cfg",
			"FacialFeaturesTracker-Low.cfg",
			"jk_300.fdp",
			"jk_300.wfm",
			"jk_300.fdp",
			"jk_300.wfm",
			"visage_powered.png",
			"warning.png",
			"bdtsdata/FF/ff.dat",
			"bdtsdata/LBF/lv",
			"bdtsdata/LBF/pr.lbf",
			"bdtsdata/NN/fa.lbf",
			"bdtsdata/NN/fc.lbf",
			"bdtsdata/LBF/pe/landmarks.txt",
			"bdtsdata/LBF/pe/W",
			"bdtsdata/LBF/pe/lp11.bdf",
			"bdtsdata/LBF/ye/lp11.bdf",
			"bdtsdata/LBF/ye/W",
			"bdtsdata/LBF/ye/landmarks.txt"
			, "900-638-182-995-087-495-807-784-735-158-947.vlc"
		};
			string outputDir;
			string localDataFolder = "VisageTracker";

			StreamWriter sw;

			outputDir = Application.persistentDataPath;

			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}
			foreach (string filename in pathsNeeded)
			{
				//if(!File.Exists(outputDir + "/" + filename))
				//{

				WWW unpacker = new WWW("jar:file://" + Application.dataPath + "!/assets/" + localDataFolder + "/" + filename);

				while (!unpacker.isDone) { }

				if (!string.IsNullOrEmpty(unpacker.error))
				{
					Debug.LogWarning("VisageTracker.Unzip: " + unpacker.error);
					continue;
				}

				//Debug.Log("VisageTracker.Unzip: filename=" + filename);

				if (filename.Contains("/"))
				{
					string[] split = filename.Split('/');
					string name = "";
					string folder = "";
					string curDir = outputDir;

					for (int i = 0; i < split.Length; i++)
					{
						if (i == split.Length - 1)
						{
							name = split[i];
						}
						else
						{
							folder = split[i];
							curDir = curDir + "/" + folder;
						}
					}
					if (!Directory.Exists(curDir))
					{
						Directory.CreateDirectory(curDir);
					}

					File.WriteAllBytes("/" + curDir + "/" + name, unpacker.bytes);
				}
				else
				{
					File.WriteAllBytes("/" + outputDir + "/" + filename, unpacker.bytes);
				}
				//Debug.Log("VisageTracker.Unzip: File written " + filename + "\n");
			}
		}
	} 
}