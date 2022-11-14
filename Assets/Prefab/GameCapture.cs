using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GameCapture
{
    [MenuItem("Component/Screen Capture")]
    public static void Capture()
    {
        // Game 画面のサイズを取得
        var size = new Vector2Int((int)Handles.GetMainGameViewSize().x, (int)Handles.GetMainGameViewSize().y);
        var render = new RenderTexture(size.x, size.y, 24);
        var texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
        var cemara = Camera.main;

        try
        {
            // カメラ画像を RenderTexture に描画
            cemara.targetTexture = render;
            cemara.Render();

            // RenderTexture の画像を読み取る
            RenderTexture.active = render;
            texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
            texture.Apply();
        }
        finally
        {
            cemara.targetTexture = null;
            RenderTexture.active = null;
        }

        // PNG 画像としてファイル保存
        File.WriteAllBytes(
            $"{Application.dataPath}/images/image.png",
            texture.EncodeToPNG());
    }
}