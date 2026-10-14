using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"UnityEngine.CoreModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Collections.Generic.IEnumerator<object>
	// }}

	public void RefMethods()
	{
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string,uint)
	}
}