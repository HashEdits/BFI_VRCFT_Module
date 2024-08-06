namespace NvidiaMaxineVRCFTModule
{
    using Microsoft.Extensions.Logging;
    using VRCFaceTracking;
    using VRCFaceTracking.Core.Library;
    using VRCFaceTracking.Core.Params.Data;
    using VRCFaceTracking.Core.Params.Expressions;

    public class TwudgeVRCFTModule : ExtTrackingModule
    {
        OscReceiver reciever = new OscReceiver(8999);
        /*
        UnifiedExpressionShape jawopen = new UnifiedExpressionShape();
        UnifiedExpressionShape jawright = new UnifiedExpressionShape();
        UnifiedExpressionShape MouthCornerSlantRight = new UnifiedExpressionShape();
        UnifiedExpressionShape MouthCornerPullRight = new UnifiedExpressionShape();*/

        UnifiedExpressionShape frown = new UnifiedExpressionShape();
        UnifiedExpressionShape mouthOpen = new UnifiedExpressionShape();
        // What your interface is able to send as tracking data.
        public override (bool SupportsEye, bool SupportsExpression) Supported => (true, true);

        // This is the first function ran by VRCFaceTracking. Make sure to completely initialize 
        // your tracking interface or the data to be accepted by VRCFaceTracking here. This will let 
        // VRCFaceTracking know what data is available to be sent from your tracking interface at initialization.
        public override (bool eyeSuccess, bool expressionSuccess) Initialize(bool eyeAvailable, bool expressionAvailable)
        {

            reciever.StartListening();
            var state = (eyeAvailable, expressionAvailable);

            ModuleInformation.Name = "Twudge BFI Module";

            // Example of an embedded image stream being referenced as a stream
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            Stream stream = a.GetManifestResourceStream("ExampleExtTrackingInterface.Assets.Nvidia_logo.png");
            // Setting the stream to be referenced by VRCFaceTracking.
            ModuleInformation.StaticImages = stream != null ? new List<Stream> { stream } : ModuleInformation.StaticImages;
            Logger.LogInformation("is stream null: " + (stream == null).ToString());
            //... Initializing module. Modify state tuple as needed (or use bool contexts to determine what should be initialized).
            return state;
        }

        // Polls data from the tracking interface.
        // VRCFaceTracking will run this function in a separate thread;
        public override void Update()
        {
            // Get latest tracking data from interface and transform to VRCFaceTracking data.

            if (Status == ModuleState.Active) // Module Status validation
            {

                Logger.LogInformation(reciever.data);
                /*                jawopen.Weight = 1f;
                                jawright.Weight = 1f;
                                MouthCornerSlantRight.Weight = 1f;
                                MouthCornerPullRight.Weight = 1f;*/

                frown.Weight = reciever.frown + (-reciever.smile);
                mouthOpen.Weight = reciever.smile;
                // ... Execute update cycle.
                UnifiedTracking.Data.Eye.Left.Openness = reciever.eyeClosed;
                UnifiedTracking.Data.Eye.Right.Openness = reciever.eyeClosed;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthFrownRight] = frown;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthFrownLeft] = frown;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthUpperUpLeft] = mouthOpen;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthUpperUpRight] = mouthOpen;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthLowerDownLeft] = mouthOpen;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthLowerDownRight] = mouthOpen;




                /*
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthFrownLeft] = jawopen;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.JawOpen] = jawopen;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.JawRight] = jawright;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthCornerSlantRight] = MouthCornerSlantRight;//Mouthright
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthCornerPullRight] = MouthCornerPullRight;//Mouthright
                */
            }

            // Add a delay or halt for the next update cycle for performance. eg: 
            Thread.Sleep(10);
        }

        // Called when the module is unloaded or VRCFaceTracking itself tears down.
        public override void Teardown()
        {
            //... Deinitialize tracking interface; dispose any data created with the module.
        }

        float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}