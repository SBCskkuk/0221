using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum IntersectionType
{
    NONE = 0,
    STOP, //�켱 ���� ����
    TRAFFIC_LIGHT, // ��ȣ�� ������ ����
    TRAFFIC_SLOW, // ���� ���� 
    EMERGENCY, // �̸Ӿ������ 
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
    // ���� �� ���� 
    public List<TrafficSegment> lightGroup1 = new List<TrafficSegment>();
    public List<TrafficSegment> lightGroup2 = new List<TrafficSegment>();
    //������ ������ �ִ� �ڵ������� ������ �ִ�
    private List<GameObject> vehicleQueue;
    private List<GameObject> vehicleInItersection;
    private TrafficHeadquarter trafficHeadquarter;
    //���� ������ �׷�    
    public int currentRedLightGroup = 1;

    //������ �����Դϱ�?
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
        //ť�� �ִ� ������ ��ȣ ������ �ƴ� �ڵ������� �̵���Ų�� .
        List<GameObject> newVehicleQueue = new List<GameObject>(vehicleQueue);
        foreach (var vehicle in vehicleQueue)
        {
            VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
            int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();
            //���� ��ȣ�� ���� ���� �����̶�� 
            if (IsRedLightSegment(vehicleSegment) == false)
            {
                vehicleControl.vehicleStatus = VehicleControl.Status.GO;
                newVehicleQueue.Remove(vehicle);
            }
        }
        vehicleQueue = newVehicleQueue;
    }
    // ��ȣ �������ְ� , ���� �̵����� ���ݴϴ�. 
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
        //�ٸ� ������ �����̰� �ϱ� ���� ��ȣ ��ȯ ���� �� �ʵ��� ��ٸ��� ���� 
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

    // �̹� ������ �ȿ� �ִ� �������� ?
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

    //�켱 ���� ���� Ʈ���� 
    void TriggerStop(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        //��������Ʈ �Ӱ谪�� ���� �ڵ����� ��� ���� �Ǵ� �ٷ� ���� ������ ���� �� �ֽ��ϴ�. 
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();


        if (IsPrioritySegment(vehicleSegment) == false)
        {
            if (vehicleQueue.Count > 0 || vehicleInItersection.Count > 0)
            {
                vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
                vehicleInItersection.Add(vehicle);
            }
            //�����ο� ���� ���ٸ�
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

    //��ȣ ������ Ʈ����  ������ ���߰ų� �̵���Ű�ų� 
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

    //��ȣ����� ������ ���������ٸ� �״�� �̵� 
    void ExitLight(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
    }
    //��� ��Ȳ �߻� Ʈ���� 
    void TriggerEmergency(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();

        vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
        vehicleQueue.Add(vehicle);
    }
    //���� �����ٸ� ��޻�Ȳ�� �����Ǿ��� ��� 
    private void ExitEmergency(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ������ �̹� ��Ͽ� �ִ��� Ȯ���ϰ� , �׷��ٸ� ó�� ���� 
        //��� ������ ���̶�� ó������(�ƿ� ���۽� �����ο� ������ �ִ� ���).
        if (IsAlreadyInIntersection(other.gameObject) || Time.timeSinceLevelLoad < 0.5f)
        {
            return;
        }
        // ������ �ƴϸ� ����
        if(other.tag.Equals(TrafficHeadquarter.VehicleTagLayer) == false)
        {
            return;
        }

        // �� �������� Ÿ�Կ� ���� ó���� �и��մϴ�
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

    //Ʈ���ſ��� ���� �������� 
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
