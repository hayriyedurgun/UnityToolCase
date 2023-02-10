using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Editor
{
    public class ModelProcessor : AssetPostprocessor
    {
        public void OnPreprocessModel()
        {
            var modelImporter = (ModelImporter)assetImporter;

            modelImporter.globalScale = 1;
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            modelImporter.importBlendShapes = false;
            modelImporter.importLights = false;
            modelImporter.importVisibility = false;
            modelImporter.importCameras = false;
            modelImporter.preserveHierarchy = false;
            modelImporter.sortHierarchyByName = true;
            modelImporter.isReadable = false;
        }

    }
}
