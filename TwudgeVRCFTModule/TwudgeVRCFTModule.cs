namespace NvidiaMaxineVRCFTModule
{
    using Microsoft.Extensions.Logging;
    using VRCFaceTracking;
    using VRCFaceTracking.Core.Library;
    using VRCFaceTracking.Core.Params.Data;
    using VRCFaceTracking.Core.Params.Expressions;

    public class TwudgeVRCFTModule : ExtTrackingModule
    {
        public bool debug = false;
        OscReceiver reciever = new OscReceiver(9000);

        UnifiedExpressionShape frown = new UnifiedExpressionShape();
        UnifiedExpressionShape mouthUpperUp = new UnifiedExpressionShape();
        UnifiedExpressionShape mouthLowerDown = new UnifiedExpressionShape();
        UnifiedExpressionShape MouthStretch = new UnifiedExpressionShape();


        UnifiedExpressionShape browDown = new UnifiedExpressionShape();
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
            Stream stream = a.GetManifestResourceStream("TwudgeVRCFTModule.Assets.Thundercats-logo.png");
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
                // ... Execute update cycle.
                if (debug) Logger.LogInformation(reciever.data);

                frown.Weight = reciever.frown + (-reciever.smile);
                mouthUpperUp.Weight = reciever.smile;
                mouthLowerDown.Weight = Math.Clamp(reciever.smile + reciever.cringe,0,1);
                browDown.Weight = reciever.anger;
                MouthStretch.Weight = reciever.cringe;

                
                UnifiedTracking.Data.Eye.Left.Openness = 1-reciever.eyeClosed;
                UnifiedTracking.Data.Eye.Right.Openness = 1-reciever.eyeClosed;

                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthFrownRight] = frown;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthFrownLeft] = frown;

                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthUpperUpLeft] = mouthUpperUp;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthUpperUpRight] = mouthUpperUp;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthLowerDownLeft] = mouthLowerDown;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthLowerDownRight] = mouthLowerDown;

                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.BrowLowererLeft] = browDown;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.BrowLowererRight] = browDown;

                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthStretchLeft] = MouthStretch;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthStretchRight] = MouthStretch;

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