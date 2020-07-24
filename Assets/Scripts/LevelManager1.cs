using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager1 : LevelManager
    {
        override public async Task ResetLevel()
        {
            FreezeGameObjects();
            await PlayTextAudio("LM1-1");
            await SpawnPlayer();
            await PlayTextAudio("LM1-2");
            await MoveItHandleToTreasure();
            await PlayTextAudio("LM1-3");
            UnfreezeGameObjects();
        }

        override public async Task StartLevel()
        {
            FreezeGameObjects();
            await PlayTextAudio("LM1-4");
            await PlayTextAudio("LM1-1");
            await SpawnPlayer();
            await PlayTextAudio("LM1-2");
            await MoveItHandleToTreasure();
            await PlayTextAudio("LM1-3");
            UnfreezeGameObjects();
        }

        override public async Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 2");
        }

        override public async Task OnFirstObstacleHit()
        {
            await PlayTextAudio("LM1-5");
        }
    }
}