using UnityEngine;
using System.Collections;

using AlmostEngine.Screenshot;

namespace AlmostEngine.Examples
{
		/// <summary>
		/// Add this script to a scene object to have a iOS gallery permission request popup at startup.
		/// </summary>
		public class RequestAuthAtStartup : MonoBehaviour
		{
				void Start ()
				{
						#if UNITY_IOS
						if(!iOsUtils.HasGalleryAuthorization()){
							iOsUtils.RequestGalleryAuthorization();
						}
						#endif
				}
		}
}