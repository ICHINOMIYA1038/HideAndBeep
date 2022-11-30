using UnityEngine;
using System.Collections;
using UnityEditor;  //!< UnityEditorを使うよ！

public class SampleWindow : EditorWindow    //!< EditorWindowを継承してね！
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    
    //! MenuItem("メニュー名/項目名") のフォーマットで記載してね
    [MenuItem("Custom/SampleWindow")]
    static void ShowWindow()
    {
        // ウィンドウを表示！
        EditorWindow.GetWindow<SampleWindow>();
    }

    void Start()
    {

    }


    /**
     * ウィンドウの中身
     */
    void OnGUI()
    {
        if (GUILayout.Button("ボタン"))
        {
            Debug.Log("押された！");
        }
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }

    void Update()
    {

    }
}
