using Raylib_cs;
using Spout.Interop.Spoututils;
using System.Numerics;
using VtubeLighting.Utility;

namespace VtubeLighting.Core;

public class Renderer
{
    public const int BASE_WIDTH = 1280;
    public const int BASE_HEIGHT = 720;

    private static RenderTexture2D renderTexture;
    private static Texture2D logo;
    private static Sender? spoutSender;
    private static RenderTexture2D spoutSenderTexture;
    private static Receiver? spoutReceiver;

    public static void Start(string[] args)
    {
        Raylib.InitWindow(BASE_WIDTH, BASE_HEIGHT, "VtubeLighting");
        Raylib.SetTargetFPS(60);

        renderTexture = Raylib.LoadRenderTexture(BASE_WIDTH, BASE_HEIGHT);
        Raylib.SetTextureFilter(renderTexture.Texture, TextureFilter.Bilinear);
        logo = Raylib.LoadTexture("Resources/Icon.jpg");

        spoutSender = new("VtubeLighting", 1920, 1080, 60);
        spoutSenderTexture = spoutSender.GetTexture();

        spoutReceiver = new("(OBS) Game", 60);

        var senders = SpoutUtility.GetSenderList(spoutReceiver.GetSpoutReceiver());

        Console.WriteLine($"[Spout] Found {senders.Count} senders: {string.Join(", ", senders)}");

        while (!Raylib.WindowShouldClose())
        {
            Update(Raylib.GetFrameTime());

            Raylib.BeginTextureMode(spoutSenderTexture);
            Raylib.ClearBackground(Color.DarkPurple);
            Render();
            Raylib.EndTextureMode();
            Raylib.BeginDrawing();
            // Raylib.DrawTextureRec(
            // spoutSenderTexture.Texture,
            //     new Rectangle(
            //         0,
            //         0,
            //         spoutSenderTexture.Texture.Width,
            //         -spoutSenderTexture.Texture.Height // 👈 flip vertically
            //     ),
            //     new Vector2(0, 0),
            //     Color.White
            // );
            //Raylib.DrawTexture(spoutReceiver.GetTexture(), 0, 0, Color.White);

            Raylib.DrawTextureRec(
                spoutReceiver.GetTexture(),
                new Rectangle(
                    0,
                    0,
                    spoutReceiver.GetTexture().Width,
                    spoutReceiver.GetTexture().Height
                ),
                new Vector2(0, 0),
                Color.White
            );

            Raylib.DrawText($"Memory: {BytesToString(GC.GetTotalMemory(true))}", 5, 5, 24, Color.White);
            Raylib.EndDrawing();
        }

        Dispose();
    }

    private static void Update(float delta)
    {
        spoutSender?.Update(delta);
        spoutReceiver?.Update(delta);
    }

    private static void Render()
    {
        Raylib.DrawTexture(logo, 0, 0, Color.White);

        Raylib.DrawText($"Memory: {BytesToString(GC.GetTotalMemory(true))}", 5, 5, 24, Color.White);
    }

    private static void Dispose()
    {
        spoutSender?.Dispose();
        spoutReceiver?.Dispose();

        Raylib.UnloadTexture(renderTexture.Texture);
        Raylib.UnloadTexture(logo);

        Raylib.CloseWindow();
    }

    private static string BytesToString(long byteCount)
    {
        const int unit = 1024;
        if (byteCount < unit) return $"{byteCount} B";
        int exp = (int)Math.Log(byteCount, unit);
        return $"{byteCount / Math.Pow(unit, exp):F2} {("KMGTPEZY"[exp - 1])}B";
    }

    public static RenderTexture2D GetRenderTexture() => renderTexture;
}
