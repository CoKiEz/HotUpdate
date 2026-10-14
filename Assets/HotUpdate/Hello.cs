using System.Collections;
using UnityEngine;
using YooAsset;

public class Hello
{
    public static bool doOnce = true;   //时候生成一次的标记
    public static string squareAssetPath = "Assets/HotUpdateDlls/Square1.prefab";    //Square的Asset路径
    public static void Run()
    {
        //查找物体  并使得Square向左或向右移动
        GameObject.Find("SquareLeft").transform.Translate(-0.1f,0,0f);
        GameObject.Find("SquareRight").transform.Translate(0.1f,0,0f);

        //只动态生成一次物体
        if(doOnce)
        {   
            //标记
            doOnce = false;
            //开启协程
            LoadDllScripts.Instance.StartCoroutine(SpawnNewSquare());
        }
    }

    public static IEnumerator SpawnNewSquare()
    {
        //得到package包
        var package = YooAssets.GetPackage("HotUpdateDlls");
        //判断是否加载成功
        if(package == null)
        {
            Debug.LogError("Package is null");
            yield break;
        }
        //句柄 根据squareAssetPath加载资源
        var dllHandle = package.LoadAssetAsync<GameObject>(squareAssetPath);
        //等待句柄加载完成 
        yield return dllHandle;
        //判断是否加载成功
        if(dllHandle.Status != EOperationStatus.Succeeded)
        {
            Debug.Log("Load asset failed" + dllHandle.Status);
            //释放资源
            dllHandle.Release();
            yield break;
        }
        //获得预制体
        GameObject prefab1 = dllHandle.AssetObject as GameObject;
        //实例化Square
        GameObject newSquare = GameObject.Instantiate(prefab1,new Vector3(0,0,0),Quaternion.identity);

    }
    
}
