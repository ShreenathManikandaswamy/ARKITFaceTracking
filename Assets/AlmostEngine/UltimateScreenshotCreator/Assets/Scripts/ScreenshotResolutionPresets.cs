#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace AlmostEngine.Screenshot
{
		[InitializeOnLoad]
		public class ScreenshotResolutionPresets
		{


				public static List<ScreenshotResolution> m_ResolutionPresets = new List<ScreenshotResolution> ();


				public static Dictionary<string, List<ScreenshotResolution>> m_Categories = new Dictionary<string, List<ScreenshotResolution>> ();


				static ScreenshotResolutionPresets ()
				{
						Init ();
				}

				public static void Init ()
				{

						m_ResolutionPresets.Add (new ScreenshotResolution ("Default", 1920, 1080, "FHD(1080p)"));
						m_ResolutionPresets.Add (new ScreenshotResolution ("Default", 1680, 1050, "WSXGA+"));
						m_ResolutionPresets.Add (new ScreenshotResolution ("Default", 1600, 900, "HD+"));   
						m_ResolutionPresets.Add (new ScreenshotResolution ("Default", 1440, 900, "WXGA+")); 
						m_ResolutionPresets.Add (new ScreenshotResolution ("Default", 1366, 768, "HD"));  
						m_ResolutionPresets.Add (new ScreenshotResolution ("Default", 1280, 1024, "SXGA"));
						m_ResolutionPresets.Add (new ScreenshotResolution ("Default", 1280, 720, "WXGA(720p)")); 
						m_ResolutionPresets.Add (new ScreenshotResolution ("Default", 1024, 768, "XGA")); 

						UpdateCategories ();

				}

				public static void Add (List<ScreenshotResolution> resolutions)
				{
						m_ResolutionPresets.AddRange (resolutions);
						UpdateCategories ();
				}

				public static void Add (ScreenshotResolution resolution)
				{
						m_ResolutionPresets.Add (resolution);
						UpdateCategories ();
				}

				public static void UpdateCategories ()
				{
						m_Categories.Clear ();
						foreach (ScreenshotResolution res in m_ResolutionPresets) {

								if (!m_Categories.ContainsKey (res.m_Category)) {
										m_Categories [res.m_Category] = new List<ScreenshotResolution> ();
								}

								m_Categories [res.m_Category].Add (res);
						}
				}
		}
}


#endif