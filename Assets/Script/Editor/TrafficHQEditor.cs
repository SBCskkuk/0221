using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




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

    private void OnEnable()
    {
        headquarter = target as TrafficHeadquarter;

    }
    //��������Ʈ �߰� 
    private void AddWaypoint(Vector3 position)
    {
        //��������Ʈ ���ӿ�����Ʈ�� ���� ����.
        GameObject go = EditorHelper.CreateGameObject(
            "Waypoint-" + headquarter.curSegment.Waypoints.Count, headquarter.curSegment.transform);
        //��ġ�� ���� Ŭ���� ������ �մϴ�.
        go.transform.position = position;
        TrafficWaypoint waypoint = EditorHelper.AddComponent<TrafficWaypoint>(go);
        waypoint.Refresh(headquarter.curSegment.Waypoints.Count,
            headquarter.curSegment);

        Undo.RecordObject(headquarter.curSegment, "");
        //HQ�� ������ ��������Ʈ�� ���� �۾����� ���׸�Ʈ�� �߰��մϴ�. 
        headquarter.curSegment.Waypoints.Add(waypoint);

    }
    //���׸�Ʈ �߰� 
    private void AddSegment(Vector3 position)
    {
        int segId = headquarter.segments.Count;
        //���׸�Ʈ��� ���� �� ���ӿ�����Ʈ�� ���ϵ�� ���׸�Ʈ ���ӿ�����Ʈ�� �����մϴ�. 
        GameObject segGameObject = EditorHelper.CreateGameObject("Segment-" + segId, headquarter.transform.GetChild(0).transform);

        //���� ���� Ŭ���� ��ġ�� ���׸�Ʈ�� �̵���ŵ�ϴ� 
        segGameObject.transform.position = position;
        //HQ�� ���� �۾����� ���׸�Ʈ�� ���� ���� ���׸�Ʈ ��ũ��Ʈ�� �������ݴϴ�
        //���� �߰��Ǵ� ��������Ʈ�� ���� �۾����� ���׸�Ʈ�� �߰��ǰ� �˴ϴ�.
        headquarter.curSegment = EditorHelper.AddComponent<TrafficSegment>(segGameObject);
        headquarter.curSegment.ID = segId;
        headquarter.curSegment.Waypoints = new List<TrafficWaypoint>();
        headquarter.curSegment.nextSegments = new List<TrafficSegment>();

        Undo.RecordObject(headquarter, "");
        headquarter.segments.Add(headquarter.curSegment);
    }
    //���ͼ��� �߰� 

    private void AddIntersection(Vector3 position)
    {
        int intId = headquarter.intersections.Count;
        //���ο� �����α����� ����I ntersections ���ӿ�����Ʈ ���ϵ�� �ٿ��ݴϴ�
        GameObject intersection = EditorHelper.CreateGameObject("Intersection-" + intId, headquarter.transform.GetChild(1).transform);


        intersection.transform.position = position;

        BoxCollider boxCollider = EditorHelper.AddComponent<BoxCollider>(intersection);
        boxCollider.isTrigger = true;
        TrafficIntersection trafficIntersection = EditorHelper.AddComponent<TrafficIntersection>(intersection);
        trafficIntersection.ID = intId;

        Undo.RecordObject(headquarter, "");
        headquarter.intersections.Add(trafficIntersection);

    }


}

