using System;
using EasyButtons;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    [Button]
    public void TestCustomClass(CustomClass typeRef)
    {
        Debug.Log(typeRef);
    }
}

[Serializable]
public class CustomClass
{
    [SerializeField] private string _stringField;
    public bool BoolField;
}
