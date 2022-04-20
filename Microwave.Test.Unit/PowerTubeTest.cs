using Microwave.Classes.Boundary;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Unit
{
    [TestFixture]
    public class PowerTubeTest
    {
        private PowerTube uut;
        private IOutput output;
        private readonly int _maxPower = 700;

        [SetUp]
        public void Setup()
        {
            output = Substitute.For<IOutput>();
            uut = new PowerTube(output, _maxPower);
        }

        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        [TestCase(699)]
        [TestCase(700)]
        public void TurnOn_WasOffCorrectPower_CorrectOutput(int power)
        {
            uut.TurnOn(power);
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"{power}")));
        }

        [TestCase(-5)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(701)]
        [TestCase(750)]
        public void TurnOn_WasOffOutOfRangePower_ThrowsException(int power)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => uut.TurnOn(power));
        }

        [Test]
        public void TurnOff_WasOn_CorrectOutput()
        {
            uut.TurnOn(50);
            uut.TurnOff();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("off")));
        }

        [Test]
        public void TurnOff_WasOff_NoOutput()
        {
            uut.TurnOff();
            output.DidNotReceive().OutputLine(Arg.Any<string>());
        }

        [Test]
        public void TurnOn_WasOn_ThrowsException()
        {
            uut.TurnOn(50);
            Assert.Throws<System.ApplicationException>(() => uut.TurnOn(60));
        }

        /////////// New unit tests ///////////
        // Makes sure that the maxpower can be changed, and the outside limits of maxpower also changes
        [TestCase(500, -1)]
        [TestCase(500, 0)]
        [TestCase(500, 501)]
        [TestCase(500, 2000)]
        [TestCase(1000, -1)]
        [TestCase(1000, 0)]
        [TestCase(1000, 1001)]
        [TestCase(1000, 1002)]
        public void TurnOn_OutsideMaxPower_ExceptionThrown(int maxPower, int turnOnPower)
        {
            uut = new PowerTube(output, maxPower);

            Assert.Throws<System.ArgumentOutOfRangeException>(() => uut.TurnOn(turnOnPower));
        }

        // Makes sure that the maxpower can be changed, and the within limits also changes
        [TestCase(500, 1)]
        [TestCase(500, 2)]
        [TestCase(500, 499)]
        [TestCase(500, 500)]
        [TestCase(1000, 1)]
        [TestCase(1000, 2)]
        [TestCase(1000, 999)]
        [TestCase(1000, 1000)]
        public void TurnOn_WithinMaxPower_NoExceptionThrown(int maxPower, int turnOnPower)
        {
            uut = new PowerTube(output, maxPower);

            Assert.DoesNotThrow(() => uut.TurnOn(turnOnPower));
        }

        [Test]
        public void Initialization_MaxPower_IsDefault700W()
        {
            uut = new PowerTube(output);
            Assert.That(uut.MaxPower == 700);
        }
    }
}