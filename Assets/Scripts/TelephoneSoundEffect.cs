using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;
using DualPantoFramework;

public class TelephoneSoundEffect : MonoBehaviour
{
    public AudioClip ringClip;

    SpeechOut speechOut;

    public AudioSource audioSource;
    GameManager gameManager;

    void Start()
    {

        speechOut = new SpeechOut();
        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
    }


    public void startPhoneRing(GameObject phoneBox){
        
        audioSource = phoneBox.GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = ringClip;
        audioSource.Play();
    }

    public async Task startPhoneTalks(){
        Debug.Log(gameManager.currentLevel);
        if(gameManager.currentLevel == 1){
            gameManager.currentLevel = 2;
            await speechOut.Speak("Yo Claude, it's Johnny Zoo. Wow, this city changed so much during the time you spent in prison. I guess walking around without seeing will be difficult for you. Use the drone that i have hidden here for you. You can use it to spot things around you. Do not run into obstacles yourself! I will call you on the other telephone booth to see if that works for you." ); 
            gameManager.StartLevel2();
        }
        else if(gameManager.currentLevel == 2){
            gameManager.currentLevel = 3;
            await speechOut.Speak("Yeah you made it! Lets see how quick you can navigate. Remember the place where you started? Its your safe house. Your drone will show you where it is. I just called the police that some crazy guy with a drone is running around here. Get to the safe house before they arrive. My buddy Danny is waiting there for you." ); // 
            gameManager.StartLevel3();
        }
        else if(gameManager.currentLevel == 3){
            gameManager.currentLevel = 4;
            await speechOut.Speak("Hi Claude. I am Danny. So you managed to escape from the police - Congrats! I guess you want to earn some money now, right? As Johnny told you, this city is going crazy. It is full of assholes! They are standing all around Downtown talking to their phones all the time. Here is a baseball bat. You can swing it by rotating the upper handle. Go, hit one of them assholes and come back here." ); //  
            gameManager.StartLevel4();
        }
        else if(gameManager.currentLevel == 4){
            Debug.Log("Level 5 Intro played");
            gameManager.currentLevel = 5;
            await speechOut.Speak(" Good Job, you hit an asshole. You own 1 Dollar. I see why you are the right man for the job! Next time you go out be aware: Police heard that something is goin on here. After you hit a few people you will hear them comin. Thats why we will pay you more money if you go an a streak and hit many assholes before returning. Go out, hit some assholes and return back to the safe house as soon as you hear the sirens"); //
            gameManager.currentObjectiveReached = false;
            gameManager.StartLevel5();
        }else if(gameManager.currentLevel == 5){
            Debug.Log("Level 6 Intro played");
            if(gameManager.hitCount == 0){
                await speechOut.Speak("Ouch, try again and hit more people until you hear the sirens");
            }else{
                gameManager.currentLevel = 6;
                await speechOut.Speak("Good Job, you hit " +gameManager.hitCount + " assholes. You own " + gameManager.cash + " Dollars. When the sirens stop and you hear some car doors closing the policemen arrived. You will hear them speak to their radio. If you get to close to them they will shout at you. If you dont run away fast enough or knock them out they will arrest you. Hit them 3 times to knock them out. This will give you extra money but more cops will come! Go and earn as much money as possible without getting arrested or dying! The Cops and the hospital will take all the money away from you if you let them get you");
            }
            gameManager.currentObjectiveReached = false;
            gameManager.StartLevel6();
        }else if(gameManager.currentLevel == 6){
            Debug.Log("Free game Intro played");
            if(!(gameManager.hitCount == 0)){
                await speechOut.Speak("Good Job, you hit " +gameManager.hitCount + " assholes. You own " + gameManager.cash + " Dollars.");
            }
            

        }
    }


    public void StopPlayback()
    {
        audioSource.Stop();
    }

}