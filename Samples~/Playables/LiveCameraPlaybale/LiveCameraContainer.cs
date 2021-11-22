using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEditor;

namespace Kinogoblin.Playables
{
	[Serializable]
	public class LiveCameraContainer : MonoBehaviour
	{
		public Transform cinemachineCameraActor;
		public CinemachineObjects cinemachineGroup;


		public void CreateDollyCameraWithPath()
		{
			CinemachineVirtualCamera vcam = null;
			GameObject go = null;
			foreach (Transform child in transform)
			{
				if (child.name.Contains("CM vcam"))
				{
					vcam = child.gameObject.GetComponent<CinemachineVirtualCamera>();
				}

				if (child.name.Contains("DollyTrack"))
				{
					go = child.gameObject;
				}
			}
			if (vcam == null)
			{
				vcam = InternalCreateVirtualCamera(
						"CM vcam", true, typeof(CinemachineComposer), typeof(CinemachineTrackedDolly));
				vcam.transform.parent = transform;
			}
			if (go == null)
			{
				go = InspectorUtility.CreateGameObject("DollyTrack",
						typeof(CinemachineSmoothPath));
				go.transform.parent = transform;
			}
			if (SceneView.lastActiveSceneView != null)
				go.transform.position = SceneView.lastActiveSceneView.pivot;
			Undo.RegisterCreatedObjectUndo(go, "create track");
			CinemachineSmoothPath path = go.GetComponent<CinemachineSmoothPath>();
			var dolly = vcam.GetCinemachineComponent<CinemachineTrackedDolly>();
			Undo.RecordObject(dolly, "create track");
			dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
			dolly.m_Path = path;

			cinemachineGroup.cinemachineSmoothPath = path;
			cinemachineGroup.cinemachineVirtualCamera = vcam;
			cinemachineGroup.dolly = dolly;
		}

		public static CinemachineVirtualCamera InternalCreateVirtualCamera(
			string name, bool selectIt, params Type[] components)
		{
			// Create a new virtual camera
			var brain = CreateCameraBrainIfAbsent();
			GameObject go = InspectorUtility.CreateGameObject(
					"CM vcam",
					typeof(CinemachineVirtualCamera));
			CinemachineVirtualCamera vcam = go.GetComponent<CinemachineVirtualCamera>();
			vcam.transform.position = brain.transform.position;
			vcam.transform.rotation = brain.transform.rotation;
			Undo.RegisterCreatedObjectUndo(go, "create " + name);
			GameObject componentOwner = vcam.GetComponentOwner().gameObject;
			foreach (Type t in components)
				Undo.AddComponent(componentOwner, t);
			vcam.InvalidateComponentPipeline();
			if (brain != null && brain.OutputCamera != null)
				vcam.m_Lens = LensSettings.FromCamera(brain.OutputCamera);
			if (selectIt)
				Selection.activeObject = go;
			return vcam;
		}

		public static CinemachineBrain CreateCameraBrainIfAbsent()
		{
			CinemachineBrain[] brains = UnityEngine.Object.FindObjectsOfType(
					typeof(CinemachineBrain)) as CinemachineBrain[];
			CinemachineBrain brain = (brains != null && brains.Length > 0) ? brains[0] : null;
			if (brain == null)
			{
				Camera cam = Camera.main;
				if (cam == null)
				{
					Camera[] cams = UnityEngine.Object.FindObjectsOfType(
							typeof(Camera)) as Camera[];
					if (cams != null && cams.Length > 0)
						cam = cams[0];
				}
				if (cam != null)
				{
					brain = Undo.AddComponent<CinemachineBrain>(cam.gameObject);
				}
			}
			return brain;
		}

		public void CreateSmoothPathWithCartForLook()
		{
			GameObject dollyTrackGO = null;
			GameObject dollyCartGO = null;
			GameObject targetPoint = null;
			GameObject cube = null;
			foreach (Transform child in cinemachineCameraActor.transform)
			{
				if (child.name.Contains("Target Point Look At"))
				{
					targetPoint = child.gameObject;
				}
			}
			foreach (Transform child in transform)
			{
				if (child.name.Contains("Dolly Track Look At"))
				{
					dollyTrackGO = child.gameObject;
					cinemachineGroup.cinemachineSmoothPathForLook = dollyTrackGO.GetComponent<CinemachineSmoothPath>();
				}
				if (child.name.Contains("Dolly Cart Look At"))
				{
					dollyCartGO = child.gameObject;
					cinemachineGroup.cinemachineDollyCartForLook = dollyCartGO.GetComponent<CinemachineDollyCart>();
				}
			}
			if (dollyTrackGO == null)
			{
				dollyTrackGO = new GameObject("Dolly Track Look At");
				dollyTrackGO.transform.parent = transform;
				cinemachineGroup.cinemachineSmoothPathForLook = dollyTrackGO.AddComponent<CinemachineSmoothPath>();
			}
			if (dollyCartGO == null)
			{
				dollyCartGO = new GameObject("Dolly Cart Look At");
				dollyCartGO.transform.parent = transform;
				cinemachineGroup.cinemachineDollyCartForLook = dollyCartGO.AddComponent<CinemachineDollyCart>();
			}
			foreach (Transform child in dollyCartGO.transform)
			{
				if (child.name.Contains("Cube"))
				{
					cube = child.gameObject;
				}
			}
			if (targetPoint == null)
			{
				targetPoint = new GameObject("Target Point Look At");
			}
			targetPoint.transform.parent = cinemachineCameraActor.transform;
			targetPoint.transform.localPosition = new Vector3(0, 0, 5);
			targetPoint.transform.localRotation = new Quaternion(0, 0, 0, 0);
			if (cube == null)
			{
				cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.parent = dollyCartGO.transform;
			}
			cube.transform.localPosition = Vector3.zero;
			cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			cube.transform.localRotation = new Quaternion(0, 0, 0, 0);

			cinemachineGroup.cinemachineDollyCartForLook.m_Path = cinemachineGroup.cinemachineSmoothPathForLook;
			cinemachineGroup.cinemachineVirtualCamera.LookAt = cube.transform;
			cinemachineGroup.targetForLook = targetPoint;

		}
	}
}