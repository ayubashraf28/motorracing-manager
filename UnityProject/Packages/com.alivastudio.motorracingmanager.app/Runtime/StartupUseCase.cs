namespace MotorracingManager.App
{
    public interface IStartupUseCase
    {
        string GetInitialScreenId();
    }

    public sealed class StartupUseCase : IStartupUseCase
    {
        public string GetInitialScreenId()
        {
            return "MainMenu";
        }
    }
}
