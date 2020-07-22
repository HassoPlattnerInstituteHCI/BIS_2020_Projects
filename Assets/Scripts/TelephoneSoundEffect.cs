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
            await speechOut.Speak("Yo Claude");//, it's Johnny Zoo.  Congratulations on breaking out of prison! Welcome back to Downtown Anywhere City. Dude. This city went crazy during the time you spent in prison. You may have some trouble navigating without being able to see. I will help you out. Grab the baseball bat that I have hidden here in the telephone booth. You can use it to spot objects around you. I will call you on the other telephone booth to see if that works for you." ); //
            gameManager.StartLevel2();
        }
        else if(gameManager.currentLevel == 2){
            //await speechOut.Speak("Yeah you made it! Lets see how quick you can navigate. Remember the place where you started? Its your safe house. Its in the south of downtown. I just called the police that some crazy guy with a baseball bat is running around. Get to the safe house before they arrive. My buddy Danny is waiting for you there." ); // 
            gameManager.StartLevel3();
        }
        else if(gameManager.currentLevel == 3){
            //await speechOut.Speak("Hi Claude. I am Danny. So you managed to escape from the police - Congrats! I guess you want to earn some money now, right? As Johnny told you, this city is going crazy. It is full of assholes! They are standing all around Downtown talking to their phones all the time. Go and hit one of them with your baseball bat. We pay you for it!" ); //  
            gameManager.StartLevel4();
        }
        else if(gameManager.currentLevel == 4){
            Debug.Log("Level 5 Intro played");
            //await speechOut.Speak(" Good Job, you hit an asshole. You own 1 Dollar. I see why you are the right man for the job! You showed it to them assholes. Next time you go out be aware: Police heard that something is goin on here. After you hit a few people you will hear them comin. Thats why we will pay you more money if you go an a streak and hit many assholes before returning. Go out, hit some assholes and return back to the safe house as soon as you here the sirens"); //
            gameManager.currentObjectiveReached = false;
            gameManager.StartLevel5();
        }else if(gameManager.currentLevel == 5 || gameManager.currentLevel == 6){
            Debug.Log("Level 6 Intro played");
            await speechOut.Speak("Good Job, you hit " +gameManager.hitCount + " assholes. You own " + gameManager.cash + " Dollars" );
            gameManager.currentObjectiveReached = false;
            gameManager.StartLevel6();
        }
    }


    public void StopPlayback()
    {
        audioSource.Stop();
    }

}