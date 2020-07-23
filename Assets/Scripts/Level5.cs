using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;

namespace PantoDrawing
{   
    public class Level5 : LevelMaster
    {
        // Start is called before the first frame update
        public override async Task StartLevel(LineDraw lineDraw, SpeechIn speechIn, SpeechOut speechOut)
        {
            await speechOut.Speak("Now you have an empty paper, draw your own face. Feel free to use the options command for all tools.");
            lineDraw.canDraw = true;
            await speechOut.Speak("Say yes when you're ready.");
            await WaitFunction(ready);
            lineDraw.canDraw = false;
            speechOut.Speak("Congratulations! You completed your first own drawing!");
        }
    }
}
