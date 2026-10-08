using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class RemoteService : IRemoteService
{
    private string _defaultUrl;
    public IReadOnlyList<string> GetRemoteUrls(string fileName)
    {
        //将路径地址存入列表并传出
        List<string> result = new List<string>();
        result.Add($"{_defaultUrl}/{fileName}");
        return result;
    }

    //构造函数 赋值传入的路径地址
    public RemoteService(string defaultUrl)
    {
        _defaultUrl = default;
    }

}
