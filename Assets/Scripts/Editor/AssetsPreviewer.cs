using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Food))]
public class AssetsPreviewer : Editor
{
    Food food;
    Texture2D tex1;
    Texture2D tex2;

    public override void OnInspectorGUI()
    {
        food = (Food)target;
        GUILayout.BeginHorizontal();

        tex1 = AssetPreview.GetAssetPreview(food.foodImage);
        GUILayout.Label(tex1);

        tex2 = AssetPreview.GetAssetPreview(food.unknownFoodImage);
        GUILayout.Label(tex2);

        GUILayout.EndHorizontal();
        DrawDefaultInspector();
    }

}
