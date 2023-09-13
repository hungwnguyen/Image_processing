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
    public RawImage rawImage;
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
        string path = UnityEditor.EditorUtility.SaveFilePanel("Save file...", "", fileName + "MakeByNTH", fileExtension);
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log("File saved: " + path);
#elif UNITY_WEBGL
        DownloadFileJsLib(bytes, bytes.Length, fileName + "MakeByNTH" + "." + fileExtension);
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
                fileName = resWidth.ToString() + "x" + resHeight.ToString() + "px_" + System.DateTime.Now.ToString(dateFormat);
            }
            
            Texture2D screenShot = CreateReadableTexture(rawImage.texture as Texture2D);
            //Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(screenshotUpscale);
            if (imageFormat == ImageFormat.jpg) DownloadFile(screenShot.EncodeToJPG(), fileName, "jpg");
            else if (imageFormat == ImageFormat.png) DownloadFile(screenShot.EncodeToPNG(), fileName, "png");
            Object.Destroy(screenShot);
        }
        catch (System.Exception e)
        {
            Debug.Log("Original error: " + e.Message);
        }
        _isRecording = false;
    }

    private Texture2D CreateReadableTexture(Texture2D sourceTexture)
    {
        if (sourceTexture == null)
        {
            Debug.LogError("Source texture is null.");
            return null;
        }
        Texture2D readableTexture = new Texture2D(sourceTexture.width, sourceTexture.height, sourceTexture.format, false);
        Graphics.CopyTexture(sourceTexture, readableTexture);
        readableTexture.Apply();

        // Get pixels
        Color[] pixels = readableTexture.GetPixels();

        // Width and height
        int width = readableTexture.width;
        int height = readableTexture.height;

        // Loop through each pixel
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Get current pixel
                Color pixel = pixels[x + y * width];

                // Calculate brightness
                float brightness = 0.299f * pixel.r + 0.587f * pixel.g + 0.114f * pixel.b;

                // Calculate the opposite color
                Color opposite = new Color(1.0f - pixel.r, 1.0f - pixel.g, 1.0f - pixel.b);

                // Calculate the brightness of the opposite color
                float oppositeBrightness = 0.299f * opposite.r + 0.587f * opposite.g + 0.114f * opposite.b;

                // Modify the pixel value
                pixel = new Color(
                    brightness + (oppositeBrightness - brightness),
                    brightness + (oppositeBrightness - brightness),
                    brightness + (oppositeBrightness - brightness),
                    pixel.a);

                // Set new pixel value
                pixels[x + y * width] = pixel;
            }
        }
        Object.Destroy(readableTexture);
        // Set pixels and update texture
        Texture2D newTexture = new Texture2D(width, height, sourceTexture.format, false, true);
        newTexture.SetPixels(pixels, 0);
        newTexture.Apply();

        Texture2D readableNewTexture = new Texture2D(width, height, sourceTexture.format, false);
        Graphics.CopyTexture(newTexture, readableNewTexture);
        readableNewTexture.Apply();
        Object.Destroy(newTexture);
        return readableNewTexture;
    }
}