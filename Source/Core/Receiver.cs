using Raylib_cs;
using Spout.Interop;
using VtubeLighting.Core.Interfaces;

namespace VtubeLighting.Core;

public class Receiver : IUpdatable, IDisposable
{
    private const uint GL_TEXTURE_2D = 0x0DE1;

    private SpoutReceiver spoutReceiver;

    private Texture2D texture;

    private uint width;
    private uint height;

    private readonly float receiveInterval;
    private float receiveAccumulator = 0f;

    public Receiver(string senderName, int pollingRate)
    {
        receiveInterval = 1f / pollingRate;

        unsafe
        {
            spoutReceiver = new();

            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(senderName + '\0');

            fixed (byte* pName = nameBytes)
            {
                sbyte* pSenderName = (sbyte*)pName;

                spoutReceiver.CreateReceiver(pSenderName, ref width, ref height);
            }
        }

        texture = Raylib.LoadTextureFromImage(
            Raylib.GenImageColor((int)width, (int)height, Color.White)
        );

        spoutReceiver.SetReceiverName(senderName);
    }

    public Texture2D GetTexture() => texture;
    public SpoutReceiver GetSpoutReceiver() => spoutReceiver;

    public void Update(float delta)
    {
        // call poll X times a second no matter how low or high the framerate is
        receiveAccumulator += delta;
        if (receiveAccumulator >= receiveInterval)
        {
            receiveAccumulator -= receiveInterval;
            Poll();
        }
    }

    private void Poll()
    {
        uint newWidth = width;
        uint newHeight = height;

        bool received = spoutReceiver.ReceiveTexture(
            texture.Id,
            GL_TEXTURE_2D,
            false,
            0
        );

        if (!received)
        {
            Console.WriteLine("[Spout] Failed to receive frame");
            return;
        }

        if (!spoutReceiver.IsConnected)
        {
            Console.WriteLine("[Spout] Not connected");
            return;
        }

        // handle sender resize
        if (newWidth != width || newHeight != height)
        {
            Console.WriteLine($"[Spout] Sender resized to {newWidth}x{newHeight}");
            width = newWidth;
            height = newHeight;

            Raylib.UnloadTexture(texture);

            texture = Raylib.LoadTextureFromImage(
                Raylib.GenImageColor((int)width, (int)height, Color.White)
            );
        }
    }

    public void Dispose()
    {
        Raylib.UnloadTexture(texture);
        spoutReceiver.Dispose();
    }
}