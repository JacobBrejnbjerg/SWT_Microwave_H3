namespace Microwave.Classes.Interfaces
{
    public interface ICookController
    {
        public int MaxPower { get; }

        void StartCooking(int power, int time);
        void Stop();
    }
}
