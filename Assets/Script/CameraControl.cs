using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Transform myTransfrom = null;
    // 타켓으로 부터의 떨어진 거리
    public float distance = 5f;
    //타켓으로 부터의 높이
    public float height = 1.5f;
    // 높이값 변경 속도 
    public float heightDamping = 2.0f;
    // 회전값 변경 속도
    public float rotataionDamping = 3.0f;

    //타겟 
    public Transform target = null;

    void Start()
    {
    myTransfrom = GetComponent<Transform>();
        //타겟이 없다면 Player 라는 태그를 가지고 있는 게임오브젝트가 타겟이다
        if (target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }
    }

    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }
        //카메라가 목표로 하고 있는 회전Y값과 높이값
        float wantedRotationAngel = target.localEulerAngles.y;
        float wantedHeigth = target.position.y + height;
        //현재 카메라가 바라보고 있는 회전Y값과 높이값 
        float currentRotationAngel = myTransfrom.eulerAngles.y;
        float currentHeight = myTransfrom.position.y;
        //현재 카메라가 바라보고 있느 ㄴ회전값과 높이값을 보간해서 새로운 값으로 계산
        currentRotationAngel = Mathf.LerpAngle(a:currentRotationAngel, b:wantedRotationAngel, t:rotataionDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(a:currentHeight, b:wantedHeigth, t:heightDamping * Time.deltaTime);
        //위에서 계산한 회전값으로 쿼너티언 회전값을 생성
        Quaternion currentRotation = Quaternion.Euler(x: 0.0f, y:currentRotationAngel, z: 0.0f);
        //카메라가 타겟의 위치에서 회전하고자 하는 벡터만큼 뒤로 이동한다.
        myTransfrom.position = target.position;
        myTransfrom.position -= currentRotation * Vector3.forward * distance;
        //이동한 위치에서 원하는 높이값으로 올라간다.
        myTransfrom.position = new Vector3(myTransfrom.position.x, y: currentHeight, myTransfrom.position.z);
        //타켓을 항상 바라보도록 한다 (forward - > target)
        myTransfrom.LookAt(target);
    }
}
