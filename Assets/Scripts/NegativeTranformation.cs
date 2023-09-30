using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeTranformation : MonoBehaviour
{
    public Texture2D GetNegativeTransformation(Texture2D sourceTexture)
    {
        if (sourceTexture == null)
        {
            Debug.LogWarning("Source texture is null.");
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
