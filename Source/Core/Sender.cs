using Raylib_cs;
using Spout.Interop;
using VtubeLighting.Core.Interfaces;

namespace VtubeLighting.Core;

public class Sender : IUpdatable, IDisposable
{
    private const uint GL_TEXTURE_2D = 0x0DE1;

    private SpoutSender spoutSender;
    private RenderTexture2D texture;
    private readonly uint width;
    private readonly uint height;
    private readonly float sendInterval;
    private float sendAccumulator = 0f;

    public Sender(string senderName, uint width, uint height, int pollingRate)
    {
        this.width = width;
        this.height = height;
        sendInterval = 1f / pollingRate;

        texture = Raylib.LoadRenderTexture((int)width, (int)height);
        Raylib.SetTextureFilter(texture.Texture, TextureFilter.Bilinear);

        spoutSender = new();
        spoutSender.CreateSender(senderName, width, height, 0);
    }

    public void Update(float delta)
    {
        // call poll X times a second no matter how low or high the framerate is
        sendAccumulator += delta;
        if (sendAccumulator >= sendInterval)
        {
            sendAccumulator -= sendInterval;
            Poll();
        }
    }

    public RenderTexture2D GetTexture() => texture;

    private void Poll()
    {
        spoutSender.SendTexture(texture.Texture.Id, GL_TEXTURE_2D, width, height, true, 0);
    }

    public void Dispose()
    {
        Raylib.UnloadRenderTexture(texture);
        spoutSender.Dispose();
    }
}