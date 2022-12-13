using UnityEngine;
using System.Collections;
using UnityEditor;  //!< UnityEditorを使うよ！

public class SampleWindow : EditorWindow    //!< EditorWindowを継承してね！
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    Vector3 position;
    [SerializeField]GameObject target;
    
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
            //target.transform.position = position;
        }
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        position = EditorGUILayout.Vector3Field("position", position, null);
        EditorGUILayout.EndToggleGroup();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Input.mousePosition;
            position = pos;
           
        }
    }
}
