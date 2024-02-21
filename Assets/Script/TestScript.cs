using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TestScript : MonoBehaviour
{
    public TextMeshProUGUI textLabel;
    //sphere
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        // ������ ���̴� �������� �ʽ��ϴ�.
        if (target == null)
        {
            return;
        }
        //cube transform
        Vector3 lhs = transform.forward;
        //target ���� ���ϴ� ����, ũ�⸦ ��ֶ������ؼ� ���⸸ ����ϴ�.
        Vector3 rhs = (target.position - transform.position).normalized;
        //������ ���մϴ�. �ִ� 1 , �ּ� -1.(Dot)
        float dot = Mathf.Clamp(Vector3.Dot(lhs, rhs), -1, 1);
        //Ÿ�� ���������� ������ �����͸� ���մϴ�.(���� ��ǥ�踦 ����ϸ� �θ� ���ϴ����� ������ �ϰ����ְ� �˼�����)
        Vector3 lineVector = transform.InverseTransformPoint(target.position);
        //���̸� �׷����ϴ�. Ÿ������ ���ϴ� ����, ť���� forward �� ��Ÿ���� ����.
        Debug.DrawRay(transform.position, lineVector, Color.red);
        Debug.DrawRay(transform.position, transform.forward, Color.cyan);
        //�ؽ�Ʈ�� ������ ���� ����մϴ�.
        textLabel.text = dot.ToString("F1");


    }
}