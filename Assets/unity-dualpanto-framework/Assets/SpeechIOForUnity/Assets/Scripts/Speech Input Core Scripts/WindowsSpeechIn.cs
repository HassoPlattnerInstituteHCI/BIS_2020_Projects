﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class WindowsSpeechIn : VoiceCommandBase {
    private KeywordRecognizer recognizer;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    public WindowsSpeechIn(OnRecognized onRecognized):base(VoiceCommandBase.onRecognized){
        VoiceCommandBase.onRecognized = onRecognized;
        VoiceCommandBase.commands = new string[]{}; //default
    }

    public WindowsSpeechIn(OnRecognized　onRecognized, string[] commands):base(VoiceCommandBase.onRecognized,commands){
      VoiceCommandBase.onRecognized = onRecognized;
      VoiceCommandBase.commands = commands;
    }

    public override void StartListening(){
        if (VoiceCommandBase.commands != null) //???
        {
            if (recognizer != null) {
                recognizer.Dispose();
            }
            recognizer = new KeywordRecognizer(VoiceCommandBase.commands, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }
    }

    public override void StartListening(string[] commands){
        if (commands != null) //???
        {
            if (recognizer != null) {
                recognizer.Dispose();
            }
            recognizer = new KeywordRecognizer(commands, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
            string list = "";
            foreach(string c in commands){list+= c + ",";}
            Debug.Log("[WinSpeech] awaiting:"+list);
        }
    }
    public override void PauseListening(){
        if (recognizer != null) {
            recognizer.Stop(); //only makes a difference in macOS in windows Stop has same functionality
        }
    }
    public override void StopListening(){
        if (recognizer != null) {
            recognizer.Stop();
        }
    }
    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        string word = args.text;
        VoiceCommandBase.onRecognized(word);
        VoiceCommandBase.setRecognized(true);
        VoiceCommandBase.recognized = word;
    }
    public override void KeyTriggered(string command)
    {
        VoiceCommandBase.onRecognized(command);
        VoiceCommandBase.setRecognized(true);
        VoiceCommandBase.recognized = command;
    }
}