using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VehicleSettingEditor : Editor
{ 
    private static void SetupWheelCollider(WheelCollider collider)
    {
        collider.mass = 20f;
        collider.radius = 0.175f;
        collider.wheelDampingRate = 0.25f;
        collider.suspensionDistance = 0.05f;
        collider.forceAppPointDistance = 0f;

        JointSpring JointSpring = new JointSpring();
        JointSpring.spring = 70000f;
        JointSpring.damper = 3500f;
        JointSpring.targetPosition = 1f;
        collider.suspensionSpring = JointSpring;

        WheelFrictionCurve frictionCurve= new WheelFrictionCurve();
        frictionCurve.extremumSlip = 1f;
        frictionCurve.extremumValue = 1f;
        frictionCurve.asymptoteValue = 1f;
        frictionCurve.asymptoteSlip = 1f;
        frictionCurve.stiffness = 1f;
        collider.forwardFriction = frictionCurve;
        collider.sidewaysFriction = frictionCurve;
    }

    [MenuItem("Componest/TrafficTool/Setup Vehicle")]
    private static void SetUpVehicle()
    {
        EditorHelper.SetUpdoGroup("Setup Vehicle");
        //현재 차를 선택했다면 (아직 세팅되지 않은)
        GameObject selected = Selection.activeGameObject;
        //오리지널 프리팹과 연결이 끊겨서 내 맘대로 수정이 가능합니다 / 예를 들어 차일드의 추가, 삭제같은 것 
        PrefabUtility.UnpackPrefabInstance(selected, PrefabUnpackMode.Completely,
            InteractionMode.AutomatedAction);

        //1. 레이캐스트 앵커 
        GameObject anchor = EditorHelper.CreateGameObject("Raycast Anchor",
            selected.transform);
        anchor.transform.localPosition = Vector3.zero;
        anchor.transform.localRotation = Quaternion.identity;
        //2. 스크립트들 설정 
        VehicleControl vehicleControl = EditorHelper.AddComponent<VehicleControl>(selected);
        vehicleControl.raycastAnchor = anchor.transform;
        //3. 바퀴 메쉬 찾아주고, 
        Transform tireBackLeft = selected.transform.Find("Tire BackLeft");
        Transform tireBackRight = selected.transform.Find("Tire BackRight");
        Transform tireFrontLeft = selected.transform.Find("Tire FrontLeft");
        Transform tireFrontRight = selected.transform.Find("Tire FrontRight");
        //4. 휠 콜라이더 세팅 하고 바퀴를 휠 콜라이더의 차일드로 붙여줍니다.
        GameObject backLeftWheel = EditorHelper.CreateGameObject("TireBackLeft Wheel",
            selected.transform);
        backLeftWheel.transform.position = tireBackLeft.position;
        GameObject backRightWheel = EditorHelper.CreateGameObject("TireBackRight Wheel",
            selected.transform);
        backRightWheel.transform.position = tireBackRight.position;
        GameObject frontLeftWheel = EditorHelper.CreateGameObject("TirefrontLeft Wheel",
            selected.transform);
        frontLeftWheel.transform.position = tireFrontLeft.position;
        GameObject frontRightWheel = EditorHelper.CreateGameObject("TirefrontRight Wheel",
            selected.transform);
        frontRightWheel.transform.position = tireFrontRight.position;

        WheelCollider wheelCollider1 = EditorHelper.AddComponent<WheelCollider>(backLeftWheel);
        WheelCollider wheelCollider2 = EditorHelper.AddComponent<WheelCollider>(backRightWheel);
        WheelCollider wheelCollider3 = EditorHelper.AddComponent<WheelCollider>(frontLeftWheel);
        WheelCollider wheelCollider4 = EditorHelper.AddComponent<WheelCollider>(frontRightWheel);


        SetupWheelCollider(wheelCollider1);
        SetupWheelCollider(wheelCollider2);
        SetupWheelCollider(wheelCollider3);
        SetupWheelCollider(wheelCollider4);

        tireBackLeft.parent = backLeftWheel.transform;
        tireBackLeft.localPosition = Vector3.zero;
        tireBackRight.parent = backRightWheel.transform;
        tireBackRight.localPosition = Vector3.zero;
        tireFrontLeft.parent = frontLeftWheel.transform;
        tireFrontLeft.localPosition = Vector3.zero;
        tireFrontRight.parent = frontRightWheel.transform;
        tireFrontRight.localPosition = Vector3.zero;

        //4.5 휠 드라이버컨트롤러 스크립트 붙여주기 

        WheelDriveControl wheelDriveControl = EditorHelper.AddComponent<WheelDriveControl>(selected);
        wheelDriveControl.Init();
        //5. 리지드바디 세팅 
        Rigidbody rb = selected.GetComponent<Rigidbody>();
        rb.mass = 900f;
        rb.drag = 0.1f;
        rb.angularDrag = 3f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;


        //6 헤더쿼터 연결 
        TrafficHeadquarter headquarter = FindObjectOfType<TrafficHeadquarter>();
        if (headquarter != null)
        {
            vehicleControl.trafficHeadquarter = headquarter;

        }

        //7 바디 콜라이더 붙여주고 
        BoxCollider boxCollider = EditorHelper.AddComponent<BoxCollider>(selected);
        boxCollider.isTrigger = true;

        GameObject colliders = EditorHelper.CreateGameObject("Colliders", 
            selected.transform );
        colliders.transform.localPosition = Vector3.zero;
        colliders.transform.localRotation = Quaternion.identity;
        colliders.transform.localScale = Vector3.one;
        GameObject Body = EditorHelper.CreateGameObject("Body", colliders.transform );
        Body.transform.localPosition = Vector3.zero;
        Body.transform.localRotation = Quaternion.identity;
        Body.transform.localScale = Vector3.one;

        BoxCollider bodyCollider = EditorHelper.AddComponent<BoxCollider>(Body);
        bodyCollider.center = new Vector3(0f, 0.4f, 0f);
        bodyCollider.size = new Vector3(0.95f, 0.54f, 2.0f);
        //만약 레이어가 없다면 엔진에 추가합니다 
        EditorHelper.CreateGameObject(TrafficHeadquarter.VehicleTagLayer);
        selected.tag = TrafficHeadquarter.VehicleTagLayer;
        EditorHelper.SetLayer(selected, LayerMask.NameToLayer(TrafficHeadquarter.VehicleTagLayer)
          ,  true);
        //undo 그룹 단위 
        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        //8 레이어까지 자동주행 레이어 / AutonomousVehicle set  

    }
}
