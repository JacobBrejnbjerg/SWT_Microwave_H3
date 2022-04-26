using Microwave.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microwave.Classes.Boundary
{
    public class Buzzer : IBuzzer
    {
        private IOutput myOutput;

        public Buzzer(IOutput output)
        {
            myOutput = output;
        }

        public void BurstBuzz()
        {
            myOutput.OutputLine("Buzz Buzz Buzz");
        }
    }
}
