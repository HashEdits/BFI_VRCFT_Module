namespace BFI_VRCFT_Module
{
    using Microsoft.Extensions.Logging;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using VRCFaceTracking;
    using VRCFaceTracking.Core.Library;
    using VRCFaceTracking.Core.Params.Data;
    using VRCFaceTracking.Core.Params.Expressions;
    using VRCFaceTracking.Core.Types;

    public class BFI_VRCFT_Module : ExtTrackingModule
    {

        private static string tagEyeClosed = "eyeclosed";
        private static string tagSmile = "smile";
        private static string tagFrown = "frown";
        private static string tagAnger = "anger";
        private static string tagCringe = "cringe";
        private static string tagCheekPuff = "cheekpuff";
        private static string tagApeShape = "apeshape";

        public static bool debug = false;
        OscReceiver reciever = new OscReceiver(8999);

        UnifiedExpressionShape frown = new UnifiedExpressionShape();
        UnifiedExpressionShape mouthUpperUp = new UnifiedExpressionShape();
        UnifiedExpressionShape mouthLowerDown = new UnifiedExpressionShape();
        UnifiedExpressionShape MouthStretch = new UnifiedExpressionShape();
        UnifiedExpressionShape browDown = new UnifiedExpressionShape();
        UnifiedExpressionShape cheekPuff = new UnifiedExpressionShape();
        UnifiedExpressionShape apeShape = new UnifiedExpressionShape();


        // What your interface is able to send as tracking data.
        public override (bool SupportsEye, bool SupportsExpression) Supported => (true, true);

        // This is the first function ran by VRCFaceTracking. Make sure to completely initialize 
        // your tracking interface or the data to be accepted by VRCFaceTracking here. This will let 
        // VRCFaceTracking know what data is available to be sent from your tracking interface at initialization.
        public override (bool eyeSuccess, bool expressionSuccess) Initialize(bool eyeAvailable, bool expressionAvailable)
        {

            reciever.StartListening();//starts OSC listener
            var state = (eyeAvailable, expressionAvailable);

            ModuleInformation.Name = "BFI Module";

            // Example of an embedded image stream being referenced as a stream
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            Stream stream = a.GetManifestResourceStream("BFI_VRCFT_Module.Assets.BFI_logo.png");
            // Setting the stream to be referenced by VRCFaceTracking.
            ModuleInformation.StaticImages = stream != null ? new List<Stream> { stream } : ModuleInformation.StaticImages;
            if (debug) Logger.LogInformation("is stream to picture null: " + (stream == null).ToString());
            //... Initializing module. Modify state tuple as needed (or use bool contexts to determine what should be initialized).


            //parsing json file for expressions
            try
            {

                JsonParser parser = new JsonParser();
                SupportedExpressions expressions = parser.ParseJson();
                reciever.expressions = expressions;
                if (expressions != null && expressions.Expressions != null)
                {
                    if (expressions.Expressions != null)
                    {
                        foreach (var expression in expressions.Expressions)
                        {
                            Logger.LogInformation($"Expression: {expression.Key}, Id: {expression.Value.Id}, Weight: {expression.Value.Weight}");
                        }
                    }
                }
                else
                {
                    Logger.LogInformation($"No expressions found in the JSON file");
                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"Error parsing JSON file: {ex.Message}");
                return (false, false);
            }
            return state;


        }

        // Polls data from the tracking interface.
        // VRCFaceTracking will run this function in a separate thread;
        public override void Update()
        {
            reciever.EvaluateTimout();
            // Get latest tracking data from interface and transform to VRCFaceTracking data.

            if (Status == ModuleState.Active) // Module Status validation
            {
                // ... Execute update cycle.


                if (debug) Logger.LogInformation(reciever.OSCDebugData);

                //UpdateValues();
                UpdateValuesExpressions();

                if (reciever.EvaluateTimout())
                {
                    UnifiedTracking.Data.Eye.Left.Gaze = new Vector2(-.75f, 0);
                    UnifiedTracking.Data.Eye.Right.Gaze = new Vector2(.75f, 0);

                    UnifiedTracking.Data.Eye.Left.Openness = 1f;
                    UnifiedTracking.Data.Eye.Right.Openness = 1f;
                }
                else
                {
                    UnifiedTracking.Data.Eye.Left.Gaze = new Vector2(0, 0);
                    UnifiedTracking.Data.Eye.Right.Gaze = new Vector2(0, 0);

                    if (reciever.expressions.Expressions.ContainsKey(tagEyeClosed))
                    {

                        UnifiedTracking.Data.Eye.Left.Openness = 1 - reciever.expressions.Expressions[tagEyeClosed].Weight;
                        UnifiedTracking.Data.Eye.Right.Openness = 1 - reciever.expressions.Expressions[tagEyeClosed].Weight;

                    }
                    else
                    {

                        UnifiedTracking.Data.Eye.Left.Openness = 1f;
                        UnifiedTracking.Data.Eye.Right.Openness = 1f;
                    }
                }

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

                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.CheekPuffRight] = cheekPuff;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.CheekPuffLeft] = cheekPuff;

                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.JawOpen] = apeShape;
                UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.MouthClosed] = apeShape;

            }

            // Add a delay or halt for the next update cycle for performance. eg: 
            Thread.Sleep(10);
        }

        private void UpdateValues()
        {
            frown.Weight = reciever.frown + (-reciever.smile);

            mouthUpperUp.Weight = reciever.smile;

            mouthLowerDown.Weight = Math.Clamp(reciever.smile + reciever.cringe, 0, 1);

            browDown.Weight = reciever.anger;

            MouthStretch.Weight = reciever.cringe;
        }

        private void UpdateValuesExpressions()
        {
            try
            {

                if (reciever.expressions.Expressions.ContainsKey(tagSmile))
                {
                    frown.Weight = (-reciever.expressions.Expressions[tagSmile].Weight);
                    mouthUpperUp.Weight = reciever.expressions.Expressions[tagSmile].Weight;
                    mouthLowerDown.Weight = reciever.expressions.Expressions[tagSmile].Weight;
                }
                if (reciever.expressions.Expressions.ContainsKey(tagFrown))
                {
                    if (reciever.expressions.Expressions.ContainsKey(tagSmile))
                    {
                        frown.Weight = (-reciever.expressions.Expressions[tagSmile].Weight) + reciever.expressions.Expressions[tagFrown].Weight;
                    }
                    else
                    {
                        frown.Weight = reciever.expressions.Expressions[tagFrown].Weight;
                    }
                }
                if (reciever.expressions.Expressions.ContainsKey(tagCringe))
                {
                    if (reciever.expressions.Expressions.ContainsKey(tagSmile))
                    {
                        mouthLowerDown.Weight = Math.Clamp(reciever.expressions.Expressions[tagSmile].Weight + reciever.expressions.Expressions[tagCringe].Weight, 0, 1);
                    }
                    else
                    {
                        mouthLowerDown.Weight = reciever.expressions.Expressions[tagCringe].Weight;
                    }
                    MouthStretch.Weight = reciever.expressions.Expressions[tagCringe].Weight;
                }
                if (reciever.expressions.Expressions.ContainsKey(tagAnger))
                {
                    browDown.Weight = reciever.expressions.Expressions[tagAnger].Weight;
                }
                if (reciever.expressions.Expressions.ContainsKey(tagCheekPuff))
                {
                    cheekPuff.Weight = reciever.expressions.Expressions[tagCheekPuff].Weight;
                }
                if (reciever.expressions.Expressions.ContainsKey(tagApeShape))
                {
                    apeShape.Weight = reciever.expressions.Expressions[tagCheekPuff].Weight;
                }

            }
            catch (Exception ex)
            {
                Logger.LogInformation($"Error trying to acces values: {ex.Message}");
            }
        }

        // Called when the module is unloaded or VRCFaceTracking itself tears down.
        public override void Teardown()
        {
            //... Deinitialize tracking interface; dispose any data created with the module.

            //resets the face to neutral uppon closing the app
            UnifiedTracking.Data.Eye.Left.Gaze = new Vector2(0, 0);
            UnifiedTracking.Data.Eye.Right.Gaze = new Vector2(0, 0);

            frown.Weight = 0;
            mouthUpperUp.Weight = 0;
            mouthLowerDown.Weight = 0;
            browDown.Weight = 0;
            MouthStretch.Weight = 0;
            cheekPuff.Weight = 0;

            UnifiedTracking.Data.Eye.Left.Openness = 1;
            UnifiedTracking.Data.Eye.Right.Openness = 1;

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

            UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.CheekPuffRight] = cheekPuff;
            UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.CheekPuffLeft] = cheekPuff;

        }

        float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

    }

}