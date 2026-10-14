using System;
using System.Reflection;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //通过反射找到名为'Hello'的类
        Type helloType = LoadDllScripts.Instance.HotUpdateAssembly.GetType("Hello");
        //在Hello类里  找到一个名为Run的方法
        MethodInfo helloMethod = helloType.GetMethod("Run");
        //执行方法
        helloMethod.Invoke(null,null);
    }
}
