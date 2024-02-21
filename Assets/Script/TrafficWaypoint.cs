using UnityEngine;

public class TrafficWaypoint : MonoBehaviour
{
    public TrafficSegment segment;
    public void RemoveCollider()
    {
        if (GetComponent<SphereCollider>())
        {
            Debug.Log("Remove Collider");
            DestroyImmediate(gameObject.GetComponent<SphereCollider>());
        }
    }
    public void Refresh (int newID, TrafficSegment newSegment)
    {
        segment = newSegment;  
        //waypoint-1 , waypoint-10 
        name = "WayPoint-"  + newID.ToString();
        tag = "WayPoint";
        gameObject.layer = LayerMask.NameToLayer("Default");
        RemoveCollider();
    }

    public Vector3 GetVisulPos()
    {
        return transform.position + new Vector3(0.0f, 0.5f , 0.0f);
    }
}
