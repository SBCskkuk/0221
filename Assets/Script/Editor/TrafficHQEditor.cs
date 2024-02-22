using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




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

    private void OnEnable()
    {
        headquarter = target as TrafficHeadquarter;

    }
    //웨이포인트 추가 
    private void AddWaypoint(Vector3 position)
    {
        //웨이포인트 게임오브젝트를 새로 생성.
        GameObject go = EditorHelper.CreateGameObject(
            "Waypoint-" + headquarter.curSegment.Waypoints.Count, headquarter.curSegment.transform);
        //위치는 내가 클릭한 곳으로 합니다.
        go.transform.position = position;
        TrafficWaypoint waypoint = EditorHelper.AddComponent<TrafficWaypoint>(go);
        waypoint.Refresh(headquarter.curSegment.Waypoints.Count,
            headquarter.curSegment);

        Undo.RecordObject(headquarter.curSegment, "");
        //HQ에 생성한 웨이포인트를 현재 작업중인 세그먼트에 추가합니다. 
        headquarter.curSegment.Waypoints.Add(waypoint);

    }
    //세그먼트 추가 
    private void AddSegment(Vector3 position)
    {
        int segId = headquarter.segments.Count;
        //세그먼트라고 만든 빈 게임오브젝트의 차일드로 세그먼트 게임오브젝트를 생성합니다. 
        GameObject segGameObject = EditorHelper.CreateGameObject("Segment-" + segId, headquarter.transform.GetChild(0).transform);

        //내가 지금 클릭한 위치에 세그먼트를 이동시킵니다 
        segGameObject.transform.position = position;
        //HQ에 현재 작업중인 세그먼트에 새로 만든 세그먼트 스크립트를 연결해줍니다
        //이후 추가되는 웨이포인트는 현재 작업중인 세그먼트에 추가되게 됩니다.
        headquarter.curSegment = EditorHelper.AddComponent<TrafficSegment>(segGameObject);
        headquarter.curSegment.ID = segId;
        headquarter.curSegment.Waypoints = new List<TrafficWaypoint>();
        headquarter.curSegment.nextSegments = new List<TrafficSegment>();

        Undo.RecordObject(headquarter, "");
        headquarter.segments.Add(headquarter.curSegment);
    }
    //인터섹션 추가 

    private void AddIntersection(Vector3 position)
    {
        int intId = headquarter.intersections.Count;
        //새로운 교차로구간을 만들어서I ntersections 게임오브젝트 차일드로 붙여줍니다
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

