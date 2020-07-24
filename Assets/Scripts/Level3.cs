using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;

namespace PantoDrawing
{   
    public class Level3 : LevelMaster
    {
        // Start is called before the first frame update
        public override async Task StartLevel(LineDraw lineDraw, SpeechIn speechIn, SpeechOut speechOut)
        {
            await speechOut.Speak("Now you can find your Mouth and the two Eyes in the picture. Draw a nose in the right spot");
            await speechOut.Speak("Use the voice commands Mouth, Eye, 0 and 1 and see whats happening.");   
            lineDraw.canDraw = true;
            await speechOut.Speak("Can you find your drawn nose? Say yes when you're ready.");
            await WaitFunction(ready);
            lineDraw.canDraw = false;
            await lineDraw.TraceLine(lineDraw.lines["line"+(lineDraw.lineCount-1)]);
        }
    }
}
