using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class 选中材质球批量贴图
{
    public static void 材质球批量贴图()
    {
        // 分别获取选中的材质和贴图
        Material[] selectedMaterials = Selection.objects.OfType<Material>().ToArray();
        Texture2D[] selectedTextures = Selection.objects.OfType<Texture2D>().ToArray();

        Debug.Log($"选中材质球数量: {selectedMaterials.Length}, 选中贴图数量: {selectedTextures.Length}");

        // 如果没有选中材质或贴图，则结束
        if (selectedMaterials.Length == 0 || selectedTextures.Length == 0)
        {
            Debug.LogWarning("请确保同时选中了材质球和贴图。");
            return;
        }

        foreach (Texture2D texture in selectedTextures)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            Debug.Log($"处理贴图: {assetPath}");

            // 解析贴图名称，尝试匹配材质名称并识别贴图类型
            string textureNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            char channelIndicator = textureNameWithoutExtension[^1]; // 获取最后一个字符作为通道指示符
            string materialNameCandidate = textureNameWithoutExtension.Substring(0, textureNameWithoutExtension.Length - 1); // 移除通道指示符

            Debug.Log($"贴图通道指示符: {channelIndicator}, 材质球候选名称: {materialNameCandidate}");

            // 查找与贴图名称（去除后缀）匹配的材质球
            Material matchingMaterial = selectedMaterials.FirstOrDefault(mat => mat.name == materialNameCandidate);

            if (matchingMaterial != null)
            {
                Debug.Log($"找到了匹配的材质球: {matchingMaterial.name}");

                // 根据通道指示符查找对应的材质属性
                var assignment = 多功能工具集.赋值数据.FirstOrDefault(a => a.后缀 == channelIndicator);
                if (assignment != null && matchingMaterial.HasProperty(assignment.属性名称))
                {
                    matchingMaterial.SetTexture(assignment.属性名称, texture);
                    Debug.Log($"为材质球 {matchingMaterial.name} 分配了 {assignment.属性名称.Replace("_", "").ToLower()} 贴图: {texture.name}");
                }
                else
                {
                    Debug.LogWarning($"未知贴图通道指示符 '{channelIndicator}' 或材质球不支持该属性.");
                }
            }
            else
            {
                Debug.LogWarning($"没有找到与贴图 {textureNameWithoutExtension} 匹配的材质球.");
            }
        }

        Debug.Log("批量分配贴图操作完成.");
    }

    public static List<string> 获取所有贴图通道名称(Material[] materials)
    {
        List<string> texturePropertyNames = new List<string>();

        foreach (Material material in materials)
        {
            // 直接获取材质的所有属性名称，然后检查是否为贴图属性
            foreach (string propertyName in material.GetTexturePropertyNames())
            {
                texturePropertyNames.Add(propertyName);
            }
        }

        return texturePropertyNames;
    }

    public static void 将选中材质球的贴图通道添加到列表()
    {
        Material[] selectedMaterials = Selection.objects.OfType<Material>().ToArray();

        if (selectedMaterials.Length == 0)
        {
            Debug.LogWarning("未选中任何材质球。");
            return;
        }

        // 确保赋值数据已经被初始化
        if (多功能工具集.赋值数据 == null)
        {
            多功能工具集.赋值数据 = new List<多功能工具集.贴图分配>();
        }
        else
        {
            // 清空现有的赋值数据列表
            多功能工具集.赋值数据.Clear();
        }

        List<string> allTextureProperties = 获取所有贴图通道名称(selectedMaterials);

        // 将获取到的贴图通道名称添加到赋值数据列表中
        foreach (string propertyName in allTextureProperties.Distinct())
        {
            多功能工具集.赋值数据.Add(new 多功能工具集.贴图分配
            {
                后缀 = ' ', // 初始化为空格，用户可以之后手动设置
                属性名称 = propertyName
            });
        }

        Debug.Log($"已将 {多功能工具集.赋值数据.Count} 个贴图通道添加到列表中。");
    }
}