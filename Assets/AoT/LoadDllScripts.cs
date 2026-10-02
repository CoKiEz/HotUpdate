using System;
using System.Reflection;
using UnityEngine;

public class LoadDllScripts : MonoBehaviour
{
    public TextAsset dllText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Assembly hotUpdataAss = Assembly.Load(dllText.bytes);

        Type helloType = hotUpdataAss.GetType("Hello");

        MethodInfo helloMethod = helloType.GetMethod("Run");

        helloMethod.Invoke(null,null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
