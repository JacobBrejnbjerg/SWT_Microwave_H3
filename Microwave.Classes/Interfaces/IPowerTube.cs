namespace Microwave.Classes.Interfaces
{
    public interface IPowerTube
    {
        public int MaxPower { get; }

        void TurnOn(int power);
        void TurnOff();
    }
}
