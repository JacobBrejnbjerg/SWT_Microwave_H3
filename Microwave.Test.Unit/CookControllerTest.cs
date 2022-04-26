using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;

namespace Microwave.Test.Unit
{
    [TestFixture]
    public class CookControllerTest
    {
        private CookController uut;

        private IUserInterface ui;
        private ITimer timer;
        private IDisplay display;
        private IPowerTube powerTube;
        private IBuzzer buzzer;

        [SetUp]
        public void Setup()
        {
            ui = Substitute.For<IUserInterface>();
            timer = Substitute.For<ITimer>();
            display = Substitute.For<IDisplay>();
            powerTube = Substitute.For<IPowerTube>();
            buzzer = Substitute.For<IBuzzer>();

            uut = new CookController(timer, display, powerTube, ui, buzzer);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(30)]
        [TestCase(120)]
        public void WhileCooking_PositiveTimeAdded_CallsTimerAddTime(int sec)
        {
            uut.StartCooking(50, 60);
            uut.AddTime(sec);

            timer.Received(1).AddTime(sec);
        }

        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-30)]
        [TestCase(-120)]
        public void WhileCooking_NegativeTimeAdded_ThrowsException(int sec)
        {
            uut.StartCooking(50, 60);

            Assert.Throws<ArgumentOutOfRangeException>(() => uut.AddTime(sec));
        }


        [Test]
        public void StartCooking_ValidParameters_TimerStarted()
        {
            uut.StartCooking(50, 60);

            timer.Received().Start(60);
        }

        [Test]
        public void StartCooking_ValidParameters_PowerTubeStarted()
        {
            uut.StartCooking(50, 60);

            powerTube.Received().TurnOn(50);
        }

        [Test]
        public void Cooking_TimerTick_DisplayCalled()
        {
            uut.StartCooking(50, 60);

            timer.TimeRemaining.Returns(115);
            timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            display.Received().ShowTime(1, 55);
        }

        [Test]
        public void Cooking_TimerExpired_PowerTubeOff()
        {
            uut.StartCooking(50, 60);

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            powerTube.Received().TurnOff();
        }

        [Test]
        public void Cooking_TimerExpired_UICalled()
        {
            uut.StartCooking(50, 60);

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            ui.Received().CookingIsDone();
        }

        [Test]
        public void Cooking_TimerExpired_BurstBuzzCalled()
        {
            uut.StartCooking(50, 60);

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            buzzer.Received().BurstBuzz();
        }

        [Test]
        public void Cooking_Stop_PowerTubeOff()
        {
            uut.StartCooking(50, 60);
            uut.Stop();

            powerTube.Received().TurnOff();
        }

        [Test]
        public void Cooking_PowerTube_ReadsCorrectMaxPower()
        {
            // Set MaxPower for PowerTube to 500
            powerTube.MaxPower.Returns(500);
            uut = new CookController(timer, display, powerTube, ui, buzzer);

            Assert.That(uut.MaxPower, Is.EqualTo(500));
        }
    }
}