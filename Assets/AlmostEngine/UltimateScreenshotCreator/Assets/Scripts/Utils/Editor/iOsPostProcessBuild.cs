﻿
#if UNITY_IOS	&&  !IGNORE_IOS_SCREENSHOT

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;

namespace AlmostEngine.Screenshot
{
		public class iOsPostProcessBuild
		{
				
				/// <summary>
				/// Changes the xcode plist to request the usage of the photo gallery.
				/// </summary>
				/// <param name="buildTarget">Build target.</param>
				/// <param name="pathToBuiltProject">Path to built project.</param>
				[PostProcessBuild]
				public static void ChangeXcodePlist (BuildTarget buildTarget, string pathToBuiltProject)
				{
						Debug.Log ("Adding plist key for PhotoLibrary usage");

						// Get plist
						string plistPath = pathToBuiltProject + "/Info.plist";
						string plist = File.ReadAllText (plistPath);

						AddKey (ref plist, "NSPhotoLibraryUsageDescription", "Ultimate Screenshot Creator requires access to the photo library to add the screenshots to the Camera Roll.");
						AddKey (ref plist, "NSPhotoLibraryAddUsageDescription", "Ultimate Screenshot Creator requires access to the photo library to add the screenshots to the Camera Roll.");

						// Write to file
						File.WriteAllText (plistPath, plist);
				}


				static void AddKey(ref string plist, string key, string description)
				{
						// Look if key exists
						if (plist.Contains (key)) {
								return;
						}

						// Get end of plist keys
						int end = plist.LastIndexOf ("</dict>");

						// Insert usage key and description
						plist = plist.Insert (end, "    <key>"+key+"</key>\n    <string>"+description+"</string>\n");


						Debug.Log ("Key " + key + " added to project plist.");
				}
		}
}
#endif
