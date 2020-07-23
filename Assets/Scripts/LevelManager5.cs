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
            await PlayTextAudio("LM5-1");
            UnfreezeGameObjects();
            ListenToSwitch();
        }

        override public async Task ResetLevel()
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await PlayTextAudio("LM5-2");
            UnfreezeGameObjects();
            ListenToSwitch();
        }

        override async public Task Success()
        {
            await PlayTextAudio("LM5-3");
        }
    }
}