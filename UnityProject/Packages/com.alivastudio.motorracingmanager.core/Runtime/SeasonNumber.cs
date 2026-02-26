namespace MotorracingManager.Core
{
    public readonly struct SeasonNumber
    {
        public SeasonNumber(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
