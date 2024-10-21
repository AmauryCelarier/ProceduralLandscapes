using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateCubeMultiRes))]
public class EditorScript_ChoixRes : Editor
{
    public override void OnInspectorGUI()
    {
        // R�f�rence vers l'instance de MyComponent
        CreateCubeMultiRes myComponent = (CreateCubeMultiRes)target;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(myComponent), typeof(CreateCubeMultiRes), false);
        EditorGUI.EndDisabledGroup();


        // Afficher le param�tre de base
        myComponent.widthCube = EditorGUILayout.FloatField("Largeur du cube", myComponent.widthCube);

        myComponent.typeCube = (CreateCubeMultiRes.TypeCube)EditorGUILayout.EnumPopup("Type de cube", myComponent.typeCube);


        // selon le choix, si multiresolution , affichage d'autre option  
        if (myComponent.typeCube == CreateCubeMultiRes.TypeCube.CubeMultiRes) { 
            // Afficher le param�tre de base
            myComponent.resLevel = (CreateCubeMultiRes.ResLevel)EditorGUILayout.EnumPopup("choix de r�solution", myComponent.resLevel);

            // Si chois libre de la r�solution , afficher un autre champ
            if (myComponent.resLevel == CreateCubeMultiRes.ResLevel.Perso){
                EditorGUILayout.LabelField("Pr�cisions r�solution Perso ", EditorStyles.boldLabel);
                myComponent.resolution = EditorGUILayout.IntField("R�solution", value: myComponent.resolution);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.IntField("Nombre de vertices ", value: myComponent.nb_vertices);
                EditorGUILayout.IntField("Nombre de triangles ", value: myComponent.nb_triangles);
                EditorGUILayout.IntField("Nombre de vertices par face ", value: myComponent.nb_vertices_par_face);
                EditorGUILayout.IntField("Nombre de triangles par face ", value: myComponent.nb_triangles_par_face);
                EditorGUI.EndDisabledGroup();
            }
        }
        // Appliquer les changements si quelque chose a �t� modifi�
        if (GUI.changed)
            EditorUtility.SetDirty(myComponent);

    }
}