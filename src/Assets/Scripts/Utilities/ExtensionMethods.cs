using UnityEngine;

namespace VtubeLighting.Utility
{
    public static class ExtensionMethods
    {
        public static Texture2D ConvertToTexture2D(this Texture texture)
        {
            // Check if the texture is already a Texture2D
            if (texture is Texture2D texture2D)
            {
                return texture2D;
            }

            // If not, create a new Texture2D
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 32);

            // Blit the original texture into the RenderTexture
            Graphics.Blit(texture, renderTexture);
            RenderTexture.active = renderTexture;

            // Create a new Texture2D with the appropriate dimensions and format
            Texture2D newTexture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

            // Read the RenderTexture into the new Texture2D
            newTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            newTexture2D.Apply();

            // Clean up
            RenderTexture.active = currentRT;
            renderTexture.Release();

            return newTexture2D;
        }
    }
}