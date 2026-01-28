using UnityEngine;
using UnityEditor;
using System.IO;

namespace Vs.Editor
{
    public static class CheckerTextureGenerator
    {
        [MenuItem("Tools/VS/Generate Checker Ground Material")]
        public static void GenerateCheckerGroundMaterial()
        {
            // 체크무늬 텍스처 생성
            int size = 256;
            int tileSize = 32;
            var texture = new Texture2D(size, size, TextureFormat.RGB24, false);

            Color color1 = new Color(0.4f, 0.4f, 0.4f); // 진한 회색
            Color color2 = new Color(0.5f, 0.5f, 0.5f); // 연한 회색

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isEvenX = (x / tileSize) % 2 == 0;
                    bool isEvenY = (y / tileSize) % 2 == 0;
                    texture.SetPixel(x, y, (isEvenX == isEvenY) ? color1 : color2);
                }
            }

            texture.Apply();
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Point;

            // 텍스처 저장
            string texturePath = "Assets/_Project/Materials/CheckerTexture.png";
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(texturePath, bytes);
            AssetDatabase.Refresh();

            // 텍스처 임포트 설정
            var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null)
            {
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            // 메터리얼 생성/수정
            string materialPath = "Assets/_Project/Materials/GroundChecker.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

            if (material == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, materialPath);
            }

            // 텍스처 적용
            var checkerTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            material.SetTexture("_BaseMap", checkerTexture);
            material.SetTextureScale("_BaseMap", new Vector2(10, 10)); // 타일링

            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();

            Debug.Log("[CheckerTextureGenerator] Created checker ground material at: " + materialPath);

            // Ground 오브젝트에 적용
            var ground = GameObject.Find("Ground");
            if (ground != null)
            {
                var renderer = ground.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = material;
                    Debug.Log("[CheckerTextureGenerator] Applied to Ground object");
                }
            }
        }
    }
}
