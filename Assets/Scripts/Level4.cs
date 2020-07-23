using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;

namespace PantoDrawing
{   
    public class Level4 : LevelMaster
    {
        // Start is called before the first frame update
        public override async Task StartLevel(LineDraw lineDraw, SpeechIn speechIn, SpeechOut speechOut)
        {
            await speechOut.Speak("Here you can find the first half of a face.");
            LineRenderer face = GameObject.Find("Face").GetComponent<LineRenderer>();
            GameObject.Find("Panto").GetComponent<GameManager>().AddVoiceCommand("Face", () =>
                    {
                        lineDraw.TraceLine(face);
                    });
            await speechOut.Speak("Draw the second half.");   
            lineDraw.canDraw = true;
            await speechOut.Speak("Say yes when you're ready.");
            await WaitFunction(ready);
            lineDraw.canDraw = false;
            await speechOut.Speak("Congrats you just drew a face!");
        }
    }
}
