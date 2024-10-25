using UnityEngine;

namespace VtubeLighting.Utility
{
    public static class ExtensionMethods
    {
        private static RenderTexture renderTextureCache;

        public static Texture2D ConvertToTexture2D(this Texture texture)
        {
            if (texture is Texture2D texture2D)
                return texture2D;

            // Recycle or create a new RenderTexture with the appropriate dimensions
            if (renderTextureCache == null || renderTextureCache.width != texture.width || renderTextureCache.height != texture.height)
            {
                if (renderTextureCache != null)
                {
                    renderTextureCache.Release();
                    Object.Destroy(renderTextureCache);
                }
                renderTextureCache = new RenderTexture(texture.width, texture.height, 32);
            }

            RenderTexture currentRT = RenderTexture.active;
            Graphics.Blit(texture, renderTextureCache);
            RenderTexture.active = renderTextureCache;

            Texture2D newTexture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            newTexture2D.ReadPixels(new Rect(0, 0, renderTextureCache.width, renderTextureCache.height), 0, 0);
            newTexture2D.Apply();

            // Restore the previous active RenderTexture
            RenderTexture.active = currentRT;

            return newTexture2D;
        }
    }
}
