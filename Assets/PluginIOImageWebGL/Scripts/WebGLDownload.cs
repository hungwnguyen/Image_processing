using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class WebGLDownload : MonoBehaviour
{
    public enum ImageFormat
    {
        jpg,
        png
    }
    [SerializeField] private RawImage _targetImage = null;
    private bool _isRecording = false;
    [DllImport("__Internal")]
    private static extern void DownloadFileJsLib(byte[] byteArray, int byteLength, string fileName);
    // The rect transform to capture.
    /// <summary>
    /// ___
    /// <para>bytes -> The bytes to be downloaded</para>
    /// <para>fileName -> The downloaded file name (without extension)</para>
    /// <para>fileExtension -> WebGLDownload.FileExtension.jpg/png/zip/</para>
    /// </summary>
    public void DownloadFile(byte[] bytes, string fileName, string fileExtension)
    {
        if (fileName == "") fileName = "UnnamedFile";
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.SaveFilePanel("Save file...", "", fileName + "MadeByNTH", fileExtension);
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log("File saved: " + path);
#elif UNITY_WEBGL
        DownloadFileJsLib(bytes, bytes.Length, "MadeByHungwNguyen" + "." + fileExtension);
#endif
    }

    /// <summary>
    /// ___
    /// <para>imageFormat -> WebGLDownload.ImageFormat.jpg/png</para>
    /// <para>screenshotUpscale -> Upscale the frame. default = 1</para>
    /// <para>fileName -> Optional filename. Empty filename creates a name texture.width x texture.height in pixel + current datetime</para>
    /// </summary>
    public void GetImage(ImageFormat imageFormat, int screenshotUpscale, string fileName = "")
    {
        if (!_isRecording) StartCoroutine(RecordUpscaledFrame(imageFormat, screenshotUpscale, fileName));
    }

    IEnumerator RecordUpscaledFrame(ImageFormat imageFormat, int screenshotUpscale, string fileName)
    {
        _isRecording = true;
        yield return new WaitForEndOfFrame();
        try
        {
            if (fileName == "")
            {
                int resWidth = Camera.main.pixelWidth * screenshotUpscale;
                int resHeight = Camera.main.pixelHeight * screenshotUpscale;
                string dateFormat = "yyyy-MM-dd-HH-mm-ss";
                fileName = System.DateTime.Now.ToString(dateFormat);
            }
            
            Texture2D screenShot = CreateReadableTexture(_targetImage.texture as Texture2D);
            //Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(screenshotUpscale);
            this.DownloadTexture(screenShot, imageFormat, fileName);
            Object.Destroy(screenShot);
        }
        catch (System.Exception e)
        {
            Debug.Log("Original error: " + e.Message);
        }
        _isRecording = false;
    }

    public Texture2D CreateReadableTexture(Texture2D sourceTexture)
    {
        if (sourceTexture == null)
        {
            Debug.LogError("Source texture is null.");
            return null;
        }

        RenderTexture rt = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
        Graphics.Blit(sourceTexture, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTexture = new Texture2D(sourceTexture.width, sourceTexture.height, sourceTexture.format, false);
        readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTexture;
    }

    private void DownloadTexture(Texture2D tex, WebGLDownload.ImageFormat imageFormat, string fileName)
    {
        byte[] texBytes;
        if (imageFormat == WebGLDownload.ImageFormat.png) texBytes = tex.EncodeToPNG();
        else texBytes = tex.EncodeToJPG();
        DownloadFile(texBytes, fileName, imageFormat.ToString());
    }
}