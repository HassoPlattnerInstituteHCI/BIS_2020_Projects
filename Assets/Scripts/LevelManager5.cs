using System.Threading.Tasks;

namespace Stealth
{
    public class LevelManager5 : LevelManager
    {
        override public async Task StartLevel()
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await speechOut.Speak(
                "You found a sword with which you can fight back enemies. Once enemies have spotted you, you will no longer die, but a battle will start. Rotate the knob on the me handle to swing your sword around you and kill your enemy.");
            UnfreezeGameObjects();
            ListenToSwitch();
        }

        override public async Task ResetLevel()
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await speechOut.Speak("Swing your sword by turing the knob on the me handle. That way you can defeat an enemy that has noticed you.");
            UnfreezeGameObjects();
            ListenToSwitch();
        }

        override async public Task Success()
        {
            await speechOut.Speak(
                "Congratutations. You finished level 5! That's the end of the game for now. Thanks for playing Stealth Panto.");
        }
    }
}