using MotorracingManager.Core;

namespace MotorracingManager.Domain
{
    public sealed class Team
    {
        public Team(string name, SeasonNumber foundedIn)
        {
            Name = name;
            FoundedIn = foundedIn;
        }

        public string Name { get; }

        public SeasonNumber FoundedIn { get; }
    }
}
