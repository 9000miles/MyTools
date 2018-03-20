using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace YouLe
{
    ///  <summary>
    ///  需要将脚本放在Editor文件夹下
    ///  </summary>
    public class AssetBundleTest : MonoBehaviour
    {
        //在Unity编辑器中添加菜单
        [MenuItem("Assets/Build AssetBundle From Selection")]
        private static void ExportResourceRGB2()
        {
            // 打开保存面板，获得用户选择的路径
            string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");

            if (path.Length != 0)
            {
                // 选择的要保存的对象
                Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
                //打包
                BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows);
            }
        }
    }
}