using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

public static partial class VisageTrackerNative
{
#if UNITY_IOS
    /** This function initialises the tracker.
 	* 
 	* Implemented in VisageTrackerUnityPlugin library.
 	*/
    [DllImport("__Internal")]
	public static extern void _initTracker(string config, string license);
	
    /** This functions binds a texture with the given native hardware texture id through OpenGL.
     * 
     * Implemented in VisageTrackerUnityPlugin library.
     */
    [DllImport("__Internal")]
    public static extern void _bindTexture(int texID);

	/** This functions binds a texture with the given native hardware texture id through metal.
     * 
     * Implemented in VisageTrackerUnityPlugin library.
     */
	[DllImport ("__Internal")]
	public static extern void _bindTextureMetal (IntPtr texID);
	
    /** This functions returns the current head translation, rotation and tracking status.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _get3DData(out float tx, out float ty, out float tz, out float rx, out float ry, out float rz);

    /** This functions returns camera info.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern bool _getCameraInfo(out float focus, out int width, out int height);

    /** This functions returns the action unit count.
	* 
	* Implemented in VisageTrackerUnityPlugin library.
	*/
    [DllImport("__Internal")]
    public static extern int _getActionUnitCount();

    /** This functions returns the name of the action unit with the specified index.
	* 
	* Implemented in VisageTrackerUnityPlugin library.
	*/
    [DllImport("__Internal")]
    public static extern IntPtr _getActionUnitName(int index);

    /** This functions true if the action unit with the spectified index is used.
	* 
	* Implemented in VisageTrackerUnityPlugin library.
	*/
    [DllImport("__Internal")]
    public static extern bool _getActionUnitUsed(int index);

    /** This functions returns all action unit values.
	* 
	* Implemented in VisageTrackerUnityPlugin library.
	*/
    [DllImport("__Internal")]
    public static extern void _getActionUnitValues(float[] values);

    /** This functions returns the gaze direction.
	 * 
	 * Implemented in VisageTrackerUnityPlugin library.
	 */
    [DllImport("__Internal")]
    public static extern bool _getGazeDirection(float[] direction);

    /** This function returns the feature point positions in normalized 2D screen coordinates.
     * 
     * Implemented in VisageTrackerUnity library.
     */
    [DllImport("__Internal")]
    public static extern bool _getFeaturePoints2D(int number, int[] groups, int[] indices, float[] positions);
	
	[DllImport("__Internal")]
    public static extern bool _getFeaturePoints3D(int number, int[] groups, int[] indices, float[] positions);
	
	[DllImport("__Internal")]
    public static extern bool _getFeaturePoints3DRel(int number, int[] groups, int[] indices, float[] positions);

	/** This function starts face tracking on current frame and returns tracker status.
     * 
     * Implemented in VisageTrackerUnity library.
     */
	[DllImport ("__Internal")]
	public static extern int _track();

	/** This function grabs current frame.
     * 
     * Implemented in VisageTrackerUnity library.
     */
	[DllImport ("__Internal")]
	public static extern void _grabFrame();

	/** This function initializes new camera with the given orientation and camera information.
     * 
     * Implemented in VisageTrackerUnity library.
     */
	[DllImport ("__Internal")]
	public static extern void _openCamera(int orientation, int device, int width, int height, int mirrored);

	/** This function closes camera.
	* 
	* Implemented in VisageTrackerUnity library.
	*/
	[DllImport ("__Internal")]
	public static extern void _closeCamera();

#endif

}