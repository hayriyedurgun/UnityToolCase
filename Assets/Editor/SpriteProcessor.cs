using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class SpriteProcessor : AssetPostprocessor
{
    private readonly string m_Path = "Assets/1_Graphics/Store";

    private void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;
        var path = importer.assetPath;

        var subStr = path.Substring(0, path.LastIndexOf("/"));
        if (subStr.Equals(m_Path))
        {
            importer.textureType = TextureImporterType.Sprite;
        }

        importer.maxTextureSize = 256;
        importer.mipmapEnabled = false;
        importer.filterMode = UnityEngine.FilterMode.Bilinear;
    }
}
