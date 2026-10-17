using System.Collections;
using UnityEngine;
using YooAsset;

public class Hello
{
    public static bool doOnce = true;   //时候生成一次的标记
    // public static string squareAssetPath1 = "Assets/HotUpdateDlls/Square1.prefab";    //Square的Asset路径
    // public static string squareAssetPath2 = "Assets/HotUpdateDlls/Square2.prefab";    //Square的Asset路径
    public static string swordAssetPath = "Assets/Knights/Knight_01/Prefabs/Sword1.prefab";    //Square的Asset路径
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
        //影藏背景图  防止看不到模型
        GameObject.Find("Square").SetActive(false);
        // //句柄 根据squareAssetPath加载资源
        // var  dllHandle1 = package.LoadAssetAsync<GameObject>(squareAssetPath1);
        // var  dllHandle2 = package.LoadAssetAsync<GameObject>(squareAssetPath2);
        // //等待句柄加载完成 
        // yield return dllHandle1;
        // yield return dllHandle2;
        //判断是否加载成功
        // if(dllHandle1.Status != EOperationStatus.Succeeded)
        // {
        //     Debug.Log("Load asset failed" + dllHandle1.Status);
        //     //释放资源
        //     dllHandle1.Release();
        //     yield break;
        // }
        // //获得预制体
        // GameObject prefab1 = dllHandle1.AssetObject as GameObject;
        // GameObject prefab2 = dllHandle2.AssetObject as GameObject;
        // //实例化Square 
        // GameObject newSquare1 = GameObject.Instantiate(prefab1,new Vector3(0,0,0),Quaternion.identity);
        // GameObject newSquare2 = GameObject.Instantiate(prefab2,new Vector3(0,0,0),Quaternion.identity);

        var  dllHandle3 = package.LoadAssetAsync<GameObject>(swordAssetPath);
        yield return dllHandle3;
        if(dllHandle3.Status != EOperationStatus.Succeeded)
        {
            Debug.Log("Load asset failed" + dllHandle3.Status);
            //释放资源
            dllHandle3.Release();
            yield break;
        }
        GameObject prefab3 = dllHandle3.AssetObject as GameObject;
        GameObject newSword1 = GameObject.Instantiate(prefab3,new Vector3(0,-3.5f,0),Quaternion.Euler(new Vector3(-90,-90,0)));
        newSword1.transform.localScale = new Vector3(8,8,8);
        

    }
    
}
