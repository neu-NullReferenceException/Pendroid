using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    private Camera camera;
    public List<GameObject> sceneObjects = new List<GameObject>();
    public string pathFolder;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        foreach(GameObject @object in sceneObjects)
        {
            @object.SetActive(false);
        }
    }

    [ContextMenu("ScreenshotLoop")]
    public void Make()
    {
        StartCoroutine(ScreenshotLoop());
    }

    IEnumerator ScreenshotLoop()
    {
        foreach(GameObject gobject in sceneObjects)
        {
            gobject.SetActive(true);
            yield return null;
            TakeScreenshot($"{Application.dataPath}/{pathFolder}/{gobject.name}_Icon.png");
            yield return null;
            gobject.SetActive(false);
        }
    }

    private void TakeScreenshot(string fullPath)
    {
        RenderTexture rt = new RenderTexture(256, 256, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
            DestroyImmediate(rt);
        else
            Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
