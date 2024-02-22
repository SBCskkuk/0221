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
        //���� ���� �����ߴٸ� (���� ���õ��� ����)
        GameObject selected = Selection.activeGameObject;
        //�������� �����հ� ������ ���ܼ� �� ����� ������ �����մϴ� / ���� ��� ���ϵ��� �߰�, �������� �� 
        PrefabUtility.UnpackPrefabInstance(selected, PrefabUnpackMode.Completely,
            InteractionMode.AutomatedAction);

        //1. ����ĳ��Ʈ ��Ŀ 
        GameObject anchor = EditorHelper.CreateGameObject("Raycast Anchor",
            selected.transform);
        anchor.transform.localPosition = Vector3.zero;
        anchor.transform.localRotation = Quaternion.identity;
        //2. ��ũ��Ʈ�� ���� 
        VehicleControl vehicleControl = EditorHelper.AddComponent<VehicleControl>(selected);
        vehicleControl.raycastAnchor = anchor.transform;
        //3. ���� �޽� ã���ְ�, 
        Transform tireBackLeft = selected.transform.Find("Tire BackLeft");
        Transform tireBackRight = selected.transform.Find("Tire BackRight");
        Transform tireFrontLeft = selected.transform.Find("Tire FrontLeft");
        Transform tireFrontRight = selected.transform.Find("Tire FrontRight");
        //4. �� �ݶ��̴� ���� �ϰ� ������ �� �ݶ��̴��� ���ϵ�� �ٿ��ݴϴ�.
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
        //5. ������ٵ� ���� 

        //6 ������� ���� 

        //7 �ٵ� �ݶ��̴� �ٿ��ְ� 

        //8 ���̾���� �ڵ����� ���̾� / AutonomousVehicle set  

    }
}
