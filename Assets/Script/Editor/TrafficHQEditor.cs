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
    //��������Ʈ ��ġ�Ҷ� �ʿ��� �ӽ� ����ҵ�
    private Vector3 startPosition;
    private Vector3 lastPosition;
    private TrafficHeadquarter lastWaypoint;

    // ������ �ùķ������� ����̵Ǵ� ��ũ��Ʈ�� ���� 
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

