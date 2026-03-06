using System.Text;
using Spout.Interop;

namespace VtubeLighting.Utility;

public static class SpoutUtility
{
    public static unsafe List<string> GetSenderList(SpoutReceiver receiver)
    {
        const int MAX_SIZE = 256;

        List<string> senders = new();

        int count = receiver.SenderCount;

        for (int i = 0; i < count; i++)
        {
            byte[] buffer = new byte[MAX_SIZE];

            fixed (byte* pBuffer = buffer)
            {
                if (receiver.GetSender(i, (sbyte*)pBuffer, MAX_SIZE))
                {
                    string name = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                    senders.Add(name);
                }
            }
        }

        return senders;
    }
}