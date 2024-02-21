using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum IntersectionType
{
    NONE = 0,
    STOP, //우선 멈춤 구간
    TRAFFIC_LIGHT, // 신호등 교차로 구간
    TRAFFIC_SLOW, // 감속 구간 
    EMERGENCY, // 이머어얼전시 
}


public class TrafficIntersection : MonoBehaviour
{
    public IntersectionType IntersectionType = IntersectionType.NONE;
    public int ID = -1;

    public List<TrafficSegment> priortySegments = new List<TrafficSegment>();
    public float lightDuration = 8f;
    private float lastChangeLightTime = 0f;
    private Coroutine lightRoutine;
    public float lightRepeatRate = 8f;
    public float orangeLightDuration = 2f;
    // 빨간 불 구간 
    public List<TrafficSegment> lightGroup1 = new List<TrafficSegment>();
    public List<TrafficSegment> lightGroup2 = new List<TrafficSegment>();
    //교차로 영역에 있는 자동차들을 가지고 있다
    private List<GameObject> vehicleQueue;
    private List<GameObject> vehicleInItersection;
    private TrafficHeadquarter trafficHeadquarter;
    //현재 빨간불 그룹    
    public int currentRedLightGroup = 1;

    //빨간불 구간입니까?
    bool IsRedLightSegment(int vehicleSegment)
    {
        if (currentRedLightGroup == 1)
        {
            foreach (var segment in lightGroup1)
            {
                if (segment.ID == vehicleSegment)
                {
                    return true;
                }
            }
        }
        else if (currentRedLightGroup == 2)
        {
            foreach (var segment in lightGroup2)
            {
                if (segment.ID == vehicleSegment)
                {
                    return true;
                }
            }
        }

        return false;
    }
    void MoveVehicleQueue()
    {
        //큐에 있는 빨간불 신호 구간이 아닌 자동차들을 이동시킨다 .
        List<GameObject> newVehicleQueue = new List<GameObject>(vehicleQueue);
        foreach (var vehicle in vehicleQueue)
        {
            VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
            int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();
            //빨간 신호를 받지 않은 차량이라면 
            if (IsRedLightSegment(vehicleSegment) == false)
            {
                vehicleControl.vehicleStatus = VehicleControl.Status.GO;
                newVehicleQueue.Remove(vehicle);
            }
        }
        vehicleQueue = newVehicleQueue;
    }
    // 신호 변경해주고 , 차량 이동까지 해줍니다. 
    void SwitchLights()
    {
        if (currentRedLightGroup == 1)
        {
            currentRedLightGroup = 2;
        }
        else if (currentRedLightGroup == 2)
        {
            currentRedLightGroup = 1;
        }
        else
        {
            currentRedLightGroup = 1;
        }
        //다른 차량을 움직이게 하기 전에 신호 전환 ㅎ후 몇 초동안 기다리게 해줌 
        Invoke("MoveVehicleQueue", orangeLightDuration);
    }
    private void Start()
    {
        vehicleQueue = new List<GameObject>();
        vehicleInItersection = new List<GameObject>();
        lastChangeLightTime = Time.time;
    }
    private IEnumerator OnTrafficLight()
    {
        SwitchLights();
        yield return new WaitForSeconds(lightRepeatRate);
    }

    private void Update()
    {
        switch (IntersectionType)
        {
            case IntersectionType.TRAFFIC_LIGHT:
                if (Time.time > lastChangeLightTime + lightDuration)
                {
                    lastChangeLightTime = Time.time;
                    lightRoutine = StartCoroutine("OnTrafficLight");
                }
                break;
            case IntersectionType.EMERGENCY:
                if (lightRoutine != null)
                {
                    StopCoroutine(lightRoutine);
                    currentRedLightGroup = 0;
                }
                break;
            case IntersectionType.STOP:
                break;
            default:
                break;
        }
    }

    // 이미 교차로 안에 있는 차량인지 ?
    bool IsAlreadyInIntersection(GameObject target)
    {
        foreach (var vehicle in vehicleInItersection)
        {
            if (vehicle.GetInstanceID() == target.GetInstanceID())
            {
                return true;
            }
        }
        foreach (var vehicle in vehicleQueue)
        {
            if (vehicle.GetInstanceID() == target.GetInstanceID())
            {
                return true;
            }
        }
        return false;
    }

    bool IsPrioritySegment(int vehicleSegment)
    {
        foreach (var segment in priortySegments)
        {
            if (vehicleSegment == segment.ID)
            {
                return true;
            }
        }
        return false;
    }

    //우선 멈춤 구간 트리거 
    void TriggerStop(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        //웨이포인트 임계값에 따라 자동차는 대상 구간 또는 바로 직전 구간에 있을 수 있습니다. 
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();


        if (IsPrioritySegment(vehicleSegment) == false)
        {
            if (vehicleQueue.Count > 0 || vehicleInItersection.Count > 0)
            {
                vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
                vehicleInItersection.Add(vehicle);
            }
            //교차로에 차가 없다면
            else
            {
                vehicleInItersection.Add(vehicle);
                vehicleControl.vehicleStatus = VehicleControl.Status.SLOW_DOWN;
            }
        }
        else
        {
            vehicleControl.vehicleStatus = VehicleControl.Status.SLOW_DOWN;
            vehicleInItersection.Add(vehicle);
        }
    }
    void ExitStop(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
        vehicleInItersection.Remove(vehicle);
        vehicleQueue.Remove(vehicle);

        if (vehicleQueue.Count > 0 && vehicleInItersection.Count == 0)
        {
            vehicleQueue[0].GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
        }
    }

    //신호 교차로 트리거  차량을 멈추거나 이동시키거나 
    void TriggerLight(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();

        if (IsRedLightSegment(vehicleSegment))
        {
            vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
            vehicleQueue.Add(vehicle);
        }
        else
        {
            vehicleControl.vehicleStatus = VehicleControl.Status.GO;
        }
    }

    //신호등교차로 구간을 빠져나갔다면 그대로 이동 
    void ExitLight(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
    }
    //긴급 상황 발생 트리거 
    void TriggerEmergency(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();

        vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
        vehicleQueue.Add(vehicle);
    }
    //빠져 나갔다면 긴급상황이 해제되었을 경우 
    private void ExitEmergency(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 차량이 이미 목록에 있는지 확인하고 , 그렇다면 처리 안함 
        //방금 시작한 앱이라면 처리안함(아예 시작시 교차로에 차량이 있는 경우).
        if (IsAlreadyInIntersection(other.gameObject) || Time.timeSinceLevelLoad < 0.5f)
        {
            return;
        }
        // 차량이 아니면 무시
        if(other.tag.Equals(TrafficHeadquarter.VehicleTagLayer) == false)
        {
            return;
        }

        // 이 교차로의 타입에 따라 처리를 분리합니다
        switch (IntersectionType)
        {
            case IntersectionType.STOP:
                TriggerStop(other.gameObject);
                break;
            case IntersectionType.TRAFFIC_LIGHT:
                TriggerLight(other.gameObject);
                break;
            case IntersectionType.EMERGENCY:
                TriggerEmergency(other.gameObject);
                break;
        }
    }

    //트리거에서 빠져 나갔을때 
    private void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals(TrafficHeadquarter.VehicleTagLayer) == false)
        {
            return;
        }
        switch (IntersectionType)
        {
            case IntersectionType.STOP:
                ExitStop(other.gameObject);
                break;
               case IntersectionType.TRAFFIC_LIGHT:

                ExitLight(other.gameObject);
                break ;
            case IntersectionType.EMERGENCY:
                ExitEmergency(other.gameObject);
                break;
        }
    }
}
