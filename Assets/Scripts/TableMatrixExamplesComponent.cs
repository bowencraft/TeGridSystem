// TableMatrixExamplesComponent.cs
using Sirenix.OdinInspector;
using UnityEngine;

//#if UNITY_EDITOR // Editor namespaces can only be used in the editor.
//using Sirenix.OdinInspector.Editor.Examples;
//#endif

public class TableMatrixExamplesComponent : SerializedMonoBehaviour
{
    [TableMatrix(HorizontalTitle = "Square Celled Matrix", SquareCells = true)]
    public Texture2D[,] SquareCelledMatrix;
    
    [TableMatrix(SquareCells = true)]
    public Mesh[,] PrefabMatrix;
    
//#if UNITY_EDITOR // Editor-related code must be excluded from builds
//    [OnInspectorInit]
//    private void CreateData()
//    {
//        SquareCelledMatrix = new Texture2D[8, 4]
//        {
//            { ExampleHelper.GetTexture(), null, null, null },
//            { null, ExampleHelper.GetTexture(), null, null },
//            { null, null, ExampleHelper.GetTexture(), null },
//            { null, null, null, ExampleHelper.GetTexture() },
//            { ExampleHelper.GetTexture(), null, null, null },
//            { null, ExampleHelper.GetTexture(), null, null },
//            { null, null, ExampleHelper.GetTexture(), null },
//            { null, null, null, ExampleHelper.GetTexture() },
//        };
    
//        PrefabMatrix = new Mesh[8, 4]
//        {
//            { ExampleHelper.GetMesh(), null, null, null },
//            { null, ExampleHelper.GetMesh(), null, null },
//            { null, null, ExampleHelper.GetMesh(), null },
//            { null, null, null, ExampleHelper.GetMesh() },
//            { null, null, null, ExampleHelper.GetMesh() },
//            { null, null, ExampleHelper.GetMesh(), null },
//            { null, ExampleHelper.GetMesh(), null, null },
//            { ExampleHelper.GetMesh(), null, null, null },
//        };
//    }
//#endif
}