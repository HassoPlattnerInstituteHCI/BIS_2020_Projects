﻿using System.Collections;
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
            //await StartCoroutine(Finish());
            await speechOut.Speak("Introduction finished, start level one.");
            LineRenderer mouth = GameObject.Find("Mouth").GetComponent<LineRenderer>();
            lineDraw.TraceLine(mouth);
            await speechOut.Speak("Here you can feel the first half of a mouth.");
            lineDraw.FindStartingPoint(mouth);
            await speechOut.Speak("Draw the second half. Turn the lower Handle to start drawing.");
            lineDraw.canDraw = true;
            await speechOut.Speak("Say yes when you're ready.");
            //await StartCoroutine(Finish());
            lineDraw.canDraw = false;
            LineRenderer secondMouth = lineDraw.lines["line"+(lineDraw.lineCount-1)];
            secondMouth.name = "Mouth2";
            lineDraw.CombineLines(mouth, secondMouth, true); //they will be both one line in "Mouth", invert the second line
            await lineDraw.TraceLine(mouth);
        }

         IEnumerator Finish() {
            yield return new WaitUntil(() => ready == true);
        }
    }
}
