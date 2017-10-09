using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

public static partial class VisageTrackerNative
{
#if UNITY_STANDALONE_WIN
    /** This function initialises the tracker.
 	 * 
 	 * Implemented in VisageTrackerUnityPlugin library.
 	 */
    #if (UNITY_64 || UNITY_EDITOR_64)
	    [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _initTracker(string config, string license);

    /** Fills the imageData with the current frame image data.
     * 
     * Implemented in VisageTrackerUnityPlugin library.
     */
    #if (UNITY_64 || UNITY_EDITOR_64)
	    [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _setFrameData(IntPtr imageData);

    /** This functions returns the current head translation, rotation and tracking status.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    #if (UNITY_64 || UNITY_EDITOR_64)
	    [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _get3DData(out float tx, out float ty, out float tz, out float rx, out float ry, out float rz);

    /** This functions returns camera info.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern bool _getCameraInfo(out float focus, out int width, out int height);

    /** This functions returns the action unit count.
	* 
	* Implemented in VisageTrackerUnityPlugin library.
	*/
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern int _getActionUnitCount();

    /** This functions returns the name of the action unit with the specified index.
	* 
	* Implemented in VisageTrackerUnityPlugin library.
	*/
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern IntPtr _getActionUnitName(int index);

    /** This functions true if the action unit with the spectified index is used.
	* 
	* Implemented in VisageTrackerUnityPlugin library.
	*/
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern bool _getActionUnitUsed(int index);

    /** This functions returns all action unit values.
	* 
	* Implemented in VisageTrackerUnityPlugin library.
	*/
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _getActionUnitValues(float[] values);

    /** This functions returns the gaze direction.
	 * 
	 * Implemented in VisageTrackerUnityPlugin library.
	 */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern bool _getGazeDirection(float[] direction);

    /** This function returns the feature point positions in normalized 2D screen coordinates.
     * 
     * Implemented in VisageTrackerUnity library.
     */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern bool _getFeaturePoints2D(int number, int[] groups, int[] indices, float[] positions);
	
	 #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern bool _getFeaturePoints3D(int number, int[] groups, int[] indices, float[] positions);
	
	 #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern bool _getFeaturePoints3DRel(int number, int[] groups, int[] indices, float[] positions);

    /** This function starts face tracking on current frame and returns tracker status.
     * 
     * Implemented in VisageTrackerUnity library.
     */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern int _track();

    /**This function grabs current frame.
      * 
      * Implemented in VisageTrackerUnity library.
      */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _grabFrame();

    /** This function initializes new camera with the given orientation and camera information.
     * 
     * Implemented in VisageTrackerUnityPlugin library.
     */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _openCamera(int orientation, int device, int width, int height, int mirrored);

	 /** This function closes camera.
     * 
     * Implemented in VisageTrackerUnityPlugin library.
     */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _closeCamera();
	
#endif
}
