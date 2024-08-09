using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VRCFaceTracking.Core.OSC;
using VRCFaceTracking.OSC;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NvidiaMaxineVRCFTModule
{
    
public class OscReceiver
{
    private UdpClient _udpClient;
    private IPEndPoint _endPoint;
        private int Port;

        public string data;

        public float eyeClosed = 0;
        public float smile = 0;
        public float frown = 0;
        public float anger = 0;
        public float cringe = 0;


        public OscReceiver(int port)
    {
        Port = port;
        _endPoint = new IPEndPoint(IPAddress.Any, port);
        _udpClient = new UdpClient(_endPoint);
    }

    public async Task StartListening()
    {
        Console.WriteLine($"Listening for OSC messageson port {Port} ...");
        while (true)
        {
            var result = await _udpClient.ReceiveAsync();
            HandleOscMessage(result.Buffer);
        }
    }

    private void HandleOscMessage(byte[] bytes)
    {
        int messageIndex = 0;
            OscMessage oscMessage = new OscMessage(bytes, bytes.Length, ref messageIndex);
        
        if (oscMessage != null)
        {
                if (oscMessage.Address.ToString().Contains("BFI/MLAction/Action1"))
                {
                    eyeClosed = (float)oscMessage.Value;
                }
                else if (oscMessage.Address.ToString().Contains("BFI/MLAction/Action2"))
                {
                    smile = (float)oscMessage.Value;
                }
                else if (oscMessage.Address.ToString().Contains("BFI/MLAction/Action3"))
                {
                    frown = (float)oscMessage.Value;

                }
                else if (oscMessage.Address.ToString().Contains("BFI/MLAction/Action4"))
                {
                    anger = (float)oscMessage.Value;

                }
                else if (oscMessage.Address.ToString().Contains("BFI/MLAction/Action5"))
                {
                    cringe = (float)oscMessage.Value;

                }



                data = $"Correct Data Recieved\nEyeClosed = {eyeClosed.ToString()} \nSmile = {smile.ToString()}\nFrown = {frown.ToString()}\nAnger = {anger.ToString()}\ncringe = {cringe.ToString()}\n\n";

            }
        else
        {
                data = "Failed to parse OSC message.";
        }
    }
}
}
