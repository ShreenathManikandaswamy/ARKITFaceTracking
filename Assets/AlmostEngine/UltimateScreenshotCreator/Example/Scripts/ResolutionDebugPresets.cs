#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using AlmostEngine.Screenshot;

namespace AlmostEngine.Examples
{
		[InitializeOnLoad]
		public class ResolutionDebugPresets
		{

				static ResolutionDebugPresets ()
				{
						Init ();
						ScreenshotResolutionPresets.Add (m_Presets);
				}



				public static List<ScreenshotResolution> m_Presets = new List<ScreenshotResolution> ();

				public static void Init ()
				{

			
						m_Presets.Add (new ScreenshotResolution ("Debug", 800, 600, "")); 
						m_Presets.Add (new ScreenshotResolution ("Debug", 300, 400, "")); 
						m_Presets.Add (new ScreenshotResolution ("Debug", 3000, 8000, "")); 
						m_Presets.Add (new ScreenshotResolution ("Debug", 800, 300, "")); 
						m_Presets.Add (new ScreenshotResolution ("Debug", 3000, 2000, "")); 
						m_Presets.Add (new ScreenshotResolution ("Debug", 200, 300, ""));  
						m_Presets.Add (new ScreenshotResolution ("Debug", 4000, 1000, "")); 



				}
		}
}


#endif