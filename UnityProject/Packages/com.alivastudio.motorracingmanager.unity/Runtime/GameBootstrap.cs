using MotorracingManager.App;
using MotorracingManager.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MotorracingManager.Unity
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private IStartupUseCase _startupUseCase;

        private void Awake()
        {
            Compose();
            LaunchInitialScreen();
        }

        private void Compose()
        {
            IGameStateStore store = new InMemoryGameStateStore();
            _startupUseCase = new StartupUseCase();
            _ = store;
        }

        private void LaunchInitialScreen()
        {
            var screenId = _startupUseCase.GetInitialScreenId();
            if (!string.IsNullOrWhiteSpace(screenId) && Application.CanStreamedLevelBeLoaded(screenId))
            {
                SceneManager.LoadScene(screenId);
                return;
            }

            Debug.LogWarning(string.Format("[GameBootstrap] Startup complete, but scene '{0}' is not in Build Settings yet.", screenId));
        }
    }
}
