namespace MotorracingManager.Persistence
{
    public interface IGameStateStore
    {
        void Save(byte[] payload);
    }

    public sealed class InMemoryGameStateStore : IGameStateStore
    {
        private byte[] _lastPayload = new byte[0];

        public void Save(byte[] payload)
        {
            _lastPayload = payload;
        }

        public byte[] Snapshot()
        {
            return _lastPayload;
        }
    }
}
