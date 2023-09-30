using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdTransformation : MonoBehaviour
{
    public Texture2D GetThresholdTransformation(Texture2D sourceTexture, float threshold)
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

                // Check if the brightness is above the threshold value
                if (brightness > threshold)
                {
                    pixels[x + y * width] = Color.white;
                }
                else
                {
                    pixels[x + y * width] = Color.black;
                }
            }
        }

        // Set pixels and update texture
        Texture2D newTexture = new Texture2D(width, height, sourceTexture.format, false, true);
        newTexture.SetPixels(pixels, 0);
        newTexture.Apply();

        Texture2D readableNewTexture = new Texture2D(width, height, sourceTexture.format, false);
        Graphics.CopyTexture(newTexture, readableNewTexture);
        readableNewTexture.Apply();
        UnityEngine.Object.Destroy(newTexture);
        return readableNewTexture;
    }
}
