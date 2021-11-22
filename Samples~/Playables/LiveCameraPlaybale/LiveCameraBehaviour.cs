namespace Kinogoblin.Playables
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    using Cinemachine;
    using System.Collections.Generic;
    using Cinemachine.Editor;
    using UnityEditor;

    [Serializable]
    public class LiveCameraBehaviour : PlayableBehaviour
    {
		public LiveCameraContainer _liveCameraContainer;
		private Transform cinemachineCameraActor;
        private CinemachineObjects cinemachineGroup;
        private bool _firstFrameHappened;
        private float _nextPointTime = 0;
        private int _currentPoint;



        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
			_liveCameraContainer = playerData as LiveCameraContainer;

            if (_liveCameraContainer.cinemachineCameraActor == null)
                return;
			cinemachineCameraActor = _liveCameraContainer.cinemachineCameraActor;
			cinemachineGroup = _liveCameraContainer.cinemachineGroup;
            if (cinemachineGroup.cinemachineSmoothPath == null || cinemachineGroup.cinemachineVirtualCamera == null)
            {
				_liveCameraContainer.CreateDollyCameraWithPath();
            }

            if (cinemachineGroup.trackLookAt)
            {
                if (cinemachineGroup.cinemachineSmoothPathForLook == null || cinemachineGroup.cinemachineDollyCartForLook == null)
                {
					_liveCameraContainer.CreateSmoothPathWithCartForLook();
                }
            }

            if ((float)playable.GetTime() <= 0 && cinemachineGroup.cinemachineSmoothPath.transform.position != cinemachineCameraActor.position)
            {
                cinemachineGroup.cinemachineSmoothPath.transform.position = cinemachineCameraActor.position;
                cinemachineGroup.cinemachineSmoothPath.transform.rotation = cinemachineCameraActor.rotation;
                cinemachineGroup.cinemachineSmoothPath.transform.localScale = new Vector3(cinemachineGroup.scale, cinemachineGroup.scale, cinemachineGroup.scale);
                if (cinemachineGroup.trackLookAt)
                {
                    if (cinemachineGroup.cinemachineSmoothPathForLook.transform.position != cinemachineGroup.targetForLook.transform.position)
                    {
                        cinemachineGroup.cinemachineSmoothPathForLook.transform.position = cinemachineGroup.targetForLook.transform.position;
                        cinemachineGroup.cinemachineSmoothPathForLook.transform.rotation = cinemachineGroup.targetForLook.transform.rotation;
                        cinemachineGroup.cinemachineSmoothPathForLook.transform.localScale = new Vector3(cinemachineGroup.scale, cinemachineGroup.scale, cinemachineGroup.scale);
                    }
                }
            }

            if (!_firstFrameHappened)
            {
                _firstFrameHappened = true;

                _currentPoint = 0;


                if (cinemachineGroup.tracking)
                {
                    cinemachineGroup.cinemachineSmoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[1];
                    cinemachineGroup.cinemachineSmoothPath.m_Waypoints[_currentPoint].position = cinemachineGroup.cinemachineSmoothPath.transform.InverseTransformPoint(cinemachineCameraActor.position);
                    cinemachineGroup.cinemachineSmoothPath.m_Waypoints[_currentPoint].roll = 0;
                    if (cinemachineGroup.trackLookAt)
                    {
                        cinemachineGroup.cinemachineSmoothPathForLook.m_Waypoints = new CinemachineSmoothPath.Waypoint[1];
                        cinemachineGroup.cinemachineSmoothPathForLook.m_Waypoints[_currentPoint].position = cinemachineGroup.cinemachineSmoothPathForLook.transform.InverseTransformPoint(cinemachineGroup.targetForLook.transform.position);
                        cinemachineGroup.cinemachineSmoothPathForLook.m_Waypoints[_currentPoint].roll = 0;
                    }
                    _currentPoint++;
                }
            }

            if (_nextPointTime <= (float)playable.GetTime() && cinemachineGroup.tracking)
            {
                List<CinemachineSmoothPath.Waypoint> temp = new List<CinemachineSmoothPath.Waypoint>(cinemachineGroup.cinemachineSmoothPath.m_Waypoints);
                CinemachineSmoothPath.Waypoint newWaypoint = new CinemachineSmoothPath.Waypoint();
                newWaypoint.position = cinemachineGroup.cinemachineSmoothPath.transform.InverseTransformPoint(cinemachineCameraActor.position);
                newWaypoint.roll = 0;
                temp.Add(newWaypoint);
                cinemachineGroup.cinemachineSmoothPath.m_Waypoints = temp.ToArray();
                cinemachineGroup.cinemachineSmoothPath.InvalidateDistanceCache();
                if (cinemachineGroup.trackLookAt)
                {
                    List<CinemachineSmoothPath.Waypoint> tempLookAt = new List<CinemachineSmoothPath.Waypoint>(cinemachineGroup.cinemachineSmoothPathForLook.m_Waypoints);
                    CinemachineSmoothPath.Waypoint newWaypointLookAt = new CinemachineSmoothPath.Waypoint();
                    newWaypointLookAt.position = cinemachineGroup.cinemachineSmoothPathForLook.transform.InverseTransformPoint(cinemachineGroup.targetForLook.transform.position);
                    newWaypointLookAt.roll = 0;
                    tempLookAt.Add(newWaypointLookAt);
                    cinemachineGroup.cinemachineSmoothPathForLook.m_Waypoints = tempLookAt.ToArray();
                    cinemachineGroup.cinemachineSmoothPathForLook.InvalidateDistanceCache();
                }
                _nextPointTime = _nextPointTime + 1 / cinemachineGroup.pointsPerSecond;
                Debug.Log(_nextPointTime);
                _currentPoint++;
            }
            if (!cinemachineGroup.tracking)
            {
                if (cinemachineGroup.cinemachineSmoothPath.PathLength != 0)
                {
                    if (cinemachineGroup.dolly.m_Path.PathLength != cinemachineGroup.cinemachineSmoothPath.PathLength)
                    {
                        cinemachineGroup.dolly.m_Path = cinemachineGroup.cinemachineSmoothPath;
                    }
                    var part = (float)playable.GetTime() / (float)playable.GetDuration();
                    cinemachineGroup.dolly.m_PathPosition = part * cinemachineGroup.cinemachineSmoothPath.PathLength;
                }
                if (cinemachineGroup.trackLookAt)
                {
                    if (cinemachineGroup.cinemachineSmoothPathForLook.PathLength != 0)
                    {
                        var part = (float)playable.GetTime() / (float)playable.GetDuration();
                        cinemachineGroup.cinemachineDollyCartForLook.m_Position = part * cinemachineGroup.cinemachineSmoothPathForLook.PathLength;
                    }
                }
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_liveCameraContainer != null)
            {
                cinemachineGroup.cinemachineSmoothPath.InvalidateDistanceCache();
            }
            if (cinemachineGroup.trackLookAt && cinemachineGroup.cinemachineSmoothPathForLook != null)
            {
                cinemachineGroup.cinemachineSmoothPathForLook.InvalidateDistanceCache();
            }
            _firstFrameHappened = false;
            _nextPointTime = 0;
            _currentPoint = 0;
            base.OnBehaviourPause(playable, info);
        }


        private void CreateSmoothPathWithCartForLook()
        {
            var dollyTrackGO = new GameObject("Dolly Track Look At");
            var dollyCartGO = new GameObject("Dolly Cart Look At");
            var targetPoint = new GameObject("Target Point Look At");
            targetPoint.transform.parent = cinemachineCameraActor.transform;
            targetPoint.transform.localPosition = new Vector3(0, 0, 5);
            targetPoint.transform.localRotation = new Quaternion(0, 0, 0, 0);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = dollyCartGO.transform;
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localRotation = new Quaternion(0, 0, 0, 0);

            cinemachineGroup.cinemachineSmoothPathForLook = dollyTrackGO.AddComponent<CinemachineSmoothPath>();
            cinemachineGroup.cinemachineDollyCartForLook = dollyCartGO.AddComponent<CinemachineDollyCart>();
            cinemachineGroup.cinemachineDollyCartForLook.m_Path = cinemachineGroup.cinemachineSmoothPathForLook;
            cinemachineGroup.cinemachineVirtualCamera.LookAt = cube.transform;
            cinemachineGroup.targetForLook = targetPoint;

        }

        #region CinemachineMethods
        [MenuItem("Cinemachine/Create Dolly Camera with Track CUSTOM", false, 1)]
        public static void CreateDollyCameraWithPathCustom()
        {
            CinemachineVirtualCamera vcam = InternalCreateVirtualCamera(
                    "CM vcam", true, typeof(CinemachineComposer), typeof(CinemachineTrackedDolly));
            GameObject go = InspectorUtility.CreateGameObject("DollyTrack",
                    typeof(CinemachineSmoothPath));
            if (SceneView.lastActiveSceneView != null)
                go.transform.position = SceneView.lastActiveSceneView.pivot;
            Undo.RegisterCreatedObjectUndo(go, "create track");
            CinemachineSmoothPath path = go.GetComponent<CinemachineSmoothPath>();
            var dolly = vcam.GetCinemachineComponent<CinemachineTrackedDolly>();
            Undo.RecordObject(dolly, "create track");
            dolly.m_Path = path;
        }
        private void CreateDollyCameraWithPath()
        {
            CinemachineVirtualCamera vcam = InternalCreateVirtualCamera(
                    "CM vcam", true, typeof(CinemachineComposer), typeof(CinemachineTrackedDolly));
            GameObject go = InspectorUtility.CreateGameObject("DollyTrack",
                    typeof(CinemachineSmoothPath));
            if (SceneView.lastActiveSceneView != null)
                go.transform.position = SceneView.lastActiveSceneView.pivot;
            Undo.RegisterCreatedObjectUndo(go, "create track");
            CinemachineSmoothPath path = go.GetComponent<CinemachineSmoothPath>();
            var dolly = vcam.GetCinemachineComponent<CinemachineTrackedDolly>();
            Undo.RecordObject(dolly, "create track");
            dolly.m_Path = path;
            cinemachineGroup.cinemachineSmoothPath = path;
            cinemachineGroup.cinemachineVirtualCamera = vcam;
            dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
            cinemachineGroup.dolly = dolly;
        }

        static CinemachineVirtualCamera InternalCreateVirtualCamera(
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

        static CinemachineBrain CreateCameraBrainIfAbsent()
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

        #endregion
    }


    [Serializable]
    public class CinemachineObjects
    {
        public CinemachineSmoothPath cinemachineSmoothPath;
        public CinemachineVirtualCamera cinemachineVirtualCamera;
        [HideInInspector] public CinemachineTrackedDolly dolly;
        public bool tracking = false;
        public bool trackLookAt = false;

        public CinemachineSmoothPath cinemachineSmoothPathForLook;
        public CinemachineDollyCart cinemachineDollyCartForLook;
        public GameObject targetForLook;
        public float pointsPerSecond = 5;
        public float scale = 1;
    }
}