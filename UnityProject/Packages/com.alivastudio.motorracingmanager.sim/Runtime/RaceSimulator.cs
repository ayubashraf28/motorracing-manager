using MotorracingManager.Domain;

namespace MotorracingManager.Sim
{
    public interface IRaceSimulator
    {
        int SimulateGridPosition(Team team);
    }

    public sealed class RaceSimulator : IRaceSimulator
    {
        public int SimulateGridPosition(Team team)
        {
            return team.Name.Length % 20 + 1;
        }
    }
}
