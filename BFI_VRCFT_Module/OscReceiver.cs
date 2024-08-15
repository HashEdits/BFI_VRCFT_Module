using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VRCFaceTracking.Core.OSC;
using VRCFaceTracking.OSC;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BFI_VRCFT_Module
{
    
    public class OscReceiver
    {
        private UdpClient _udpClient;
        private IPEndPoint _endPoint;
        private static string _oscAddress = "/BFI/MLAction/Action";//adress to listen for OSC messages

        private Stopwatch timer = new Stopwatch();//timout timer
        private double timeoutTime = 3;//timout time in seconds

        public string debugString;//debug string to display in the console


        public SupportedExpressions expressions;


        public OscReceiver(IPAddress address, int port, double timouttime)
        {
            _endPoint = new IPEndPoint(address, port);
            _udpClient = new UdpClient(_endPoint);
            timouttime = timeoutTime;
        }

        public async Task StartListening()//starts listening for OSC messages
        {
            debugString = ($"Listening for OSC messages on ip: {_endPoint.Address.ToString()} port: {_endPoint.Port} ...");
            timer.Start();
            while (true)
            {
                var result = await _udpClient.ReceiveAsync();
                HandleOscMessage(result.Buffer);
            }
        }
        public bool EvaluateTimout()//returns true if the timer has exceeded the timeout time
        {
            return timer.Elapsed.TotalSeconds > timeoutTime;
        }

        private void HandleOscMessage(byte[] bytes)//treats data from OSC messages
        {
            int messageIndex = 0;
            OscMessage oscMessage = new OscMessage(bytes, bytes.Length, ref messageIndex);
        
            if (oscMessage != null)
            {

                    foreach (var expression in expressions.Expressions)
                    {
                        if(oscMessage.Address.ToString().Contains(_oscAddress + expression.Value.Id))
                        {
                            expression.Value.Weight = (float)oscMessage.Value;
                            resetStopwatch();
                        }
                        
                    }

                    debugString = $"message recieved: {oscMessage.Address.ToString()} = {oscMessage.Value}";

                    
                }
            else
            {
                   debugString = "Failed to parse OSC message.";
            }
        }

        private void resetStopwatch()//function that resets the timout timer when a message is recieved
        {
            timer.Reset();
            timer.Restart();
        }
    }
}
