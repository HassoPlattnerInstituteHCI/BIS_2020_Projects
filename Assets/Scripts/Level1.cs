using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;

namespace PantoDrawing
{   
    public class Level1 : LevelMaster
    {
        // Start is called before the first frame update
        public override async Task StartLevel(LineDraw lineDraw, SpeechIn speechIn, SpeechOut speechOut)
        {
            await speechOut.Speak("Welcome to Panto Drawing");
            await speechOut.Speak("Explore your drawing area. Say yes when you're ready.");
            Debug.Log(drawn);
            await WaitFunction(ready);
            Debug.Log(drawn);
            await speechOut.Speak("Lets make your first drawing.");

            LineRenderer mouth = GameObject.Find("Mouth").GetComponent<LineRenderer>();
            GameObject.Find("Panto").GetComponent<GameManager>().AddVoiceCommand("Mouth", () =>
                    {
                        lineDraw.TraceLine(mouth);
                    });
            
            await speechOut.Speak("Here you can feel the first half of a mouth.");
            await lineDraw.TraceLine(mouth);
            await speechOut.Speak("Draw the second half. Turn the upper Handle to start drawing.");
            lineDraw.FindStartingPoint(mouth);
            lineDraw.canDraw = true;
            await speechOut.Speak("Say yes when you're ready.");
            Debug.Log(drawn);
            await WaitFunction(ready);
            Debug.Log(drawn);
            lineDraw.canDraw = false;
            LineRenderer secondMouth = lineDraw.lines["line"+(lineDraw.lineCount-1)];
            lineDraw.CombineLines(mouth, secondMouth, true); //they will be both one line in "Mouth", invert the second line
            await lineDraw.TraceLine(mouth);
        }
    }
}
