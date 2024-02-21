using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class EditorHelper
{
    public static void SetUpdoGroup(string label)
    {

        //이 뒤로 나오는 모든 변화를 하나의 그룹으로 묶는다는 뜻 
        //컨트롤 z 를 하면 그룹단위로 취소 됩니다 
        Undo.SetCurrentGroupName(label);
    }
    public static void BeginUndoGroup(string undoName, TrafficHeadquarter trafficHeadquarter)
    {
        //Undo 그룹 세팅 
        Undo.SetCurrentGroupName(undoName);
        // headquater 에서 발생하는 모든 변화를 등록하게 됩니다. 
        Undo.RegisterFullObjectHierarchyUndo(trafficHeadquarter.gameObject, undoName);
    }
    public static GameObject CreateGameObject(string name, Transform parent = null)
    {
        GameObject newGameObject = new GameObject(name);
        newGameObject.transform.position = Vector3.zero;
        newGameObject.transform.localScale = Vector3.one;
        newGameObject.transform.localRotation = Quaternion.identity;

        Undo.RegisterFullObjectHierarchyUndo(newGameObject, "Spawn Create GameObject");
        Undo.SetTransformParent(newGameObject.transform, parent, "Set Parent");
        return newGameObject;
    }

    //컴포넌트 붙이는 작업도 undo가 가능하도록 세팅   
    public static T AddComponent<T>(GameObject target) where T : Component
    {
        return Undo.AddComponent<T>(target);
    }

    //레이와 구의 충돌 판별식입니다 값이 true 라면 구 반경에 레이가 hit 되었다는 뜻 
    public static bool SphereHit(Vector3 center, float radius, Ray ray)
    {
        Vector3 originToCenter = ray.origin - center;
        float a = Vector3.Dot(ray.direction, ray.direction);
        float b = 2f * Vector3.Dot(originToCenter, ray.direction);
        float c = Vector3.Dot(originToCenter, originToCenter) - (radius * radius);
        float discriminant = b * b - 4f * a * c;

        if (discriminant < 0f)
        {
            return false;
        }

        //한점이상 충돌되었어요 
        float sqrt = Mathf.Sqrt(discriminant);
        return -b - sqrt > 0f || -b + sqrt > 0f;
    }

    //없는 게임 오브젝트 레이어 생성하는 함수 
    public static void CreateLayer(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("name", "새로운 레이어를 추가할려면 이름을 꼭 입력해주세요 .");
        }
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetRepresentationsAtPath("ProjectSettings/TagMAnager.asset")[0]);
        var layerProps = tagManager.FindProperty("layers");
        var propCount = layerProps.arraySize;

        SerializedProperty firstEmptyProp = null;
        for (var i = 0; i < propCount; ++i)
        {
            var layerProp = layerProps.GetArrayElementAtIndex(i);
            var stringValue = layerProp.stringValue;
            if (stringValue == name)
            {
                return;
            }
            //builtin, 이미 다른 레이어가 자리를 차지하고 있다면 
            if (i < 8 || stringValue != string.Empty)
            {
                continue;
            }
            if (firstEmptyProp == null)
            {
                firstEmptyProp = layerProp;
                break;
            }
        }
        if (firstEmptyProp != null)
        {
            Debug.LogError($"레이어가 최대 갯구에 도달하였습니다 . 그래서{name}를 생성하지 못하였습니다  ");
            return;
        }
        firstEmptyProp.stringValue = name;
        tagManager.ApplyModifiedProperties();


    }
    /// <summary>
    /// GameObject에 레이어를 세팅합니다. 원하면 자식들도 전부다 세팅합니다
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layer"></param>
    /// <param name="자식도 교체합니까?"></param>
    public static void SetLayer(this GameObject gameObject, int layer, bool includeChildren = false)
    {
        if (!includeChildren)
        {
            gameObject.layer = layer;
            return;
        }
        foreach (var child in gameObject.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = layer;
        }
    }
}