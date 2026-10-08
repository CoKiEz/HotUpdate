using System;
using System.Collections;
using System.Reflection;
using UnityEditor.Build.Reporting;
using UnityEngine;
using YooAsset;
public class LoadDllScripts : MonoBehaviour
{
    private string packageName = "HotUpdateDlls";
    private string dllFullAssetPath = "Assets/HotUpdateDlls/HotUpdateDlls.dll.bytes";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartHot());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartHot()
    {
        //初始YooAssets的全局资源系统
        YooAssets.Initialize();
        //获取资源包 没有的话就直接创建一个新的
        var package = YooAssets.TryGetPackage(packageName,out var packageObj) ? packageObj : YooAssets.CreatePackage(packageName);
        
        ///配置并初始化文件系统
        //离线模式
        var createParameters = new OfflinePlayModeOptions();
        //创建一个默认的系统文件组  方便读取
        createParameters.BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();
        //拷贝到沙盒
        createParameters.BuiltinFileSystemParameters.AddParameter(EFileSystemParameter.CopyBuiltinPackageManifest,true);
        //初始化资源包
        var initOp = package.InitializePackageAsync(createParameters);
        yield return initOp;

        //请求资源包对应的版本号
        var versionOp = package.RequestPackageVersionAsync();
        yield return versionOp;

        //获取版本资源  创建加载参数
        string packageVersion = versionOp.PackageVersion;
        var manifestOptions = new LoadPackageManifestOptions(packageVersion,60);
        var manifestop = package.LoadPackageManifestAsync(manifestOptions);
        yield return manifestop;

        //根据dllFullAssetPath加载资源
        var dllHandle = package.LoadAssetAsync<TextAsset>(dllFullAssetPath);
        yield return dllHandle;
        //加载dll
        TextAsset dllText = dllHandle.AssetObject as TextAsset;
        Assembly hotUpdataAss = Assembly.Load(dllText.bytes);
        //通过反射找到名为'Hello'的类
        Type helloType = hotUpdataAss.GetType("Hello");
        //在Hello类里  找到一个名为Run的方法
        MethodInfo helloMethod = helloType.GetMethod("Run");
        //执行方法
        helloMethod.Invoke(null,null);
        //卸载资源包
        dllHandle.Release();
    }
}
