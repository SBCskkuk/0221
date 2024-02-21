using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Transform myTransfrom = null;
    // Ÿ������ ������ ������ �Ÿ�
    public float distance = 5f;
    //Ÿ������ ������ ����
    public float height = 1.5f;
    // ���̰� ���� �ӵ� 
    public float heightDamping = 2.0f;
    // ȸ���� ���� �ӵ�
    public float rotataionDamping = 3.0f;

    //Ÿ�� 
    public Transform target = null;

    void Start()
    {
    myTransfrom = GetComponent<Transform>();
        //Ÿ���� ���ٸ� Player ��� �±׸� ������ �ִ� ���ӿ�����Ʈ�� Ÿ���̴�
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
        //ī�޶� ��ǥ�� �ϰ� �ִ� ȸ��Y���� ���̰�
        float wantedRotationAngel = target.localEulerAngles.y;
        float wantedHeigth = target.position.y + height;
        //���� ī�޶� �ٶ󺸰� �ִ� ȸ��Y���� ���̰� 
        float currentRotationAngel = myTransfrom.eulerAngles.y;
        float currentHeight = myTransfrom.position.y;
        //���� ī�޶� �ٶ󺸰� �ִ� ��ȸ������ ���̰��� �����ؼ� ���ο� ������ ���
        currentRotationAngel = Mathf.LerpAngle(a:currentRotationAngel, b:wantedRotationAngel, t:rotataionDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(a:currentHeight, b:wantedHeigth, t:heightDamping * Time.deltaTime);
        //������ ����� ȸ�������� ����Ƽ�� ȸ������ ����
        Quaternion currentRotation = Quaternion.Euler(x: 0.0f, y:currentRotationAngel, z: 0.0f);
        //ī�޶� Ÿ���� ��ġ���� ȸ���ϰ��� �ϴ� ���͸�ŭ �ڷ� �̵��Ѵ�.
        myTransfrom.position = target.position;
        myTransfrom.position -= currentRotation * Vector3.forward * distance;
        //�̵��� ��ġ���� ���ϴ� ���̰����� �ö󰣴�.
        myTransfrom.position = new Vector3(myTransfrom.position.x, y: currentHeight, myTransfrom.position.z);
        //Ÿ���� �׻� �ٶ󺸵��� �Ѵ� (forward - > target)
        myTransfrom.LookAt(target);
    }
}
