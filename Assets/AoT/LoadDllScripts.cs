using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using YooAsset;
using UnityEngine.SceneManagement;
public class LoadDllScripts : MonoBehaviour
{
    private string packageName = "HotUpdateDlls";
    private string dllFullAssetPath = "Assets/HotUpdateDlls/HotUpdate.dll.bytes";
    private string defaultHostServer = "http://127.0.0.1:8080/CDN/PC/"; //本地服务器路径 配合Tomcat使用
    private string defaultVersion = "v1.0";

    //单例模式
    public LoadDllScripts Instance {get;private set;}
    public Assembly HotUpdateAssembly {get;private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

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
        //创建服务器远程服务实例
        var remoteService = new RemoteService(defaultHostServer + defaultVersion);

        ///配置并初始化文件系统
        //离线模式 -> 联机模式
        //var createParameters = new OfflinePlayModeOptions();
        var createParameters = new HostPlayModeOptions();
        //创建一个默认的系统文件组  方便读取
        createParameters.BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();
        //拷贝到沙盒
        createParameters.BuiltinFileSystemParameters.AddParameter(EFileSystemParameter.CopyBuiltinPackageManifest,true);
        //绑定远程服务
        createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultSandboxFileSystemParameters(remoteService);
        //设置最大并发数
        createParameters.CacheFileSystemParameters.AddParameter(EFileSystemParameter.DownloadMaxConcurrency,5);
        //设置每帧最大请求数
        createParameters.CacheFileSystemParameters.AddParameter(EFileSystemParameter.DownloadMaxRequestPerFrame,1);
        //设置最大的超时时间，超时后会重新下载
        createParameters.CacheFileSystemParameters.AddParameter(EFileSystemParameter.DownloadWatchdogTimeout,10);


        //初始化资源包
        var initOp = package.InitializePackageAsync(createParameters);
        yield return initOp;
        //加载失败  挂起
        if(initOp.Status != EOperationStatus.Succeeded)
        {
            Debug.LogError("初始化资源包加载失败" + initOp.Error);
            yield break;
        }
        Debug.Log("初始化资源包成功");

        //向远端请求的版本号
        var versionOp = package.RequestPackageVersionAsync();
        yield return versionOp;
        if(versionOp.Status != EOperationStatus.Succeeded)
        {
            Debug.LogError("请求版本号失败" + versionOp.Error);
            yield break;
        }
        Debug.Log("请求版本号成功" + versionOp.PackageVersion);

        //更新资源清单
        var options = new LoadPackageManifestOptions(versionOp.PackageVersion,60);
        var mainifestOp = package.LoadPackageManifestAsync(options);
        yield return mainifestOp;
        if(mainifestOp.Status != EOperationStatus.Succeeded)
        {
            Debug.LogError("更新清单失败" + mainifestOp.Error);
            yield break;
        }
        Debug.Log("更新清单成功");

        //创建下载器
        var downloaderOption = new ResourceDownloaderOptions(10,3); //最大并发数量  重试次数
        var downloader = package.CreateResourceDownloader(downloaderOption);
        if(downloader.TotalDownloadCount == 0)
        {Debug.Log("资源包已经是最新的");}
        else
        {
            //开始下载
            downloader.StartDownload();
            yield return downloader;
            if(downloader.Status != EOperationStatus.Succeeded)
            {
                Debug.Log("下载失败" + downloader.Error);
                yield break;
            }
            Debug.Log("下载成功");
        }

        //根据dllFullAssetPath加载资源
        var dllHandle = package.LoadAssetAsync<TextAsset>(dllFullAssetPath);
        yield return dllHandle;
        //加载dll
        TextAsset dllText = dllHandle.AssetObject as TextAsset;
        HotUpdateAssembly = Assembly.Load(dllText.bytes);
        //通过反射找到名为'Hello'的类
        // Type helloType = hotUpdataAss.GetType("Hello");
        // //在Hello类里  找到一个名为Run的方法
        // MethodInfo helloMethod = helloType.GetMethod("Run");
        // //执行方法
        // helloMethod.Invoke(null,null);
        // //卸载资源包 
        // dllHandle.Release();
        //等待三秒  不然后续一瞬间直接加载关卡
        yield return new WaitForSeconds(3);

        //加载游戏关卡
        SceneManager.LoadScene("GameScene");
    }
}
