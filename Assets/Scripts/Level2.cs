using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;

namespace PantoDrawing
{   
    public class Level2 : LevelMaster
    {
        // Start is called before the first frame update
        public override async Task StartLevel(LineDraw lineDraw, SpeechIn speechIn, SpeechOut speechOut)
        {
            LineRenderer eye = GameObject.Find("Eye").GetComponent<LineRenderer>();
            GameObject.Find("Panto").GetComponent<GameManager>().AddVoiceCommand("Eye", () =>
                    {
                        lineDraw.TraceLine(eye);
                    });
            lineDraw.TraceLine(eye);
            await speechOut.Speak("Here you can feel a human eye.");
            await speechOut.Speak("Draw the second eye now.");
            lineDraw.canDraw = true;
            await speechOut.Speak("Say circle when you're ready, then yes.");
            await WaitFunction(ready);
            lineDraw.canDraw = false;
            LineRenderer secondEye = lineDraw.lines["line"+(lineDraw.lineCount-1)];

            await lineDraw.TraceLine(secondEye);
        }
    }
}
