using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;




[CustomEditor(typeof(TrafficHeadquarter))]
public class TrafficHQEditor : Editor
{
    private TrafficHeadquarter headquarter;
    //웨이포인트 설치할때 필요한 임시 저장소들
    private Vector3 startPosition;
    private Vector3 lastPosition;
    private TrafficHeadquarter lastWaypoint;

    // 프래픽 시뮬레이터의 기반이되는 스크립트들 생성 
    [MenuItem("Component/TrafficTool/Create Traffic System")]

   private static void CrateTrafficSystem()
    {
        EditorHelper.SetUpdoGroup("Create Traffic System");

        GameObject headquarterObject = EditorHelper.CreateGameObject("Traffic Headquarter");
        EditorHelper.AddComponent<TrafficHeadquarter>(headquarterObject);

        GameObject segmentsObject = EditorHelper.CreateGameObject("Segments",
            headquarterObject.transform);

        GameObject intersectionObject = EditorHelper.CreateGameObject("Intersections",
            headquarterObject.transform);

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }
 }

