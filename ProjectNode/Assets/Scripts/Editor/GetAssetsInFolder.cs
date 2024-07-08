// using UnityEditor;
// using UnityEngine;
//  
// public class OnAssetsEvent : UnityEditor.AssetModificationProcessor
// {
//     [InitializeOnLoadMethod]
//     static void EditorApplication_projectChanged()
//     {
//         //--projectWindowChanged已过时
//         //--全局监听Project视图下的资源是否发生变化（添加 删除 移动等）
//         EditorApplication.projectChanged += delegate ()
//         {
//             Debug.Log("资源状态发生变化！");
//         };
//     }
//     //--监听“双击鼠标左键，打开资源”事件
//     public static bool IsOpenForEdit(string assetPath, out string message)
//     {
//         message = null;
//         Debug.Log("双击鼠标左键，打开资源 assetPath:" + assetPath);
//         return true;
//     }
//     //--监听“资源即将被创建”事件
//     public static void OnWillCreateAsset(string path)
//     {
//         Debug.Log("资源即将被创建 path：" + path);
//     }
//     //--监听“资源即将被保存”事件
//     public static string[] OnWillSaveAssets(string[] paths)
//     {
//         if (paths != null)
//         {
//             Debug.Log("资源即将被保存 path :" + string.Join(",", paths));
//         }
//         return paths;
//     }
//     //--监听“资源即将被移动”事件
//     public static AssetMoveResult OnWillMoveAsset(string oldPath,string newPath)
//     {
//         Debug.Log("资源即将被移动 form:" + oldPath + " to:" + newPath);
//         return AssetMoveResult.DidNotMove;
//     }
//     //--监听“资源即将被删除”事件
//     public static AssetDeleteResult OnWillDeleteAsset(string assetPath,RemoveAssetOptions option)
//     {
//         Debug.Log("资源即将被删除 : " + assetPath);
//         return AssetDeleteResult.DidNotDelete;
//     }
// }