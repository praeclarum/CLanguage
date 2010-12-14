using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Repl
{
    
    class Program
    {
        class Pr : Report.ReportPrinter
        {
            public Pr(Program p)
            {
            }

            public override void Print(Report.AbstractMessage msg)
            {
                if (!msg.IsWarning)
                {
                    var o = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Print(msg, Console.Out);
                    Console.ForegroundColor = o;
                }
            }
        }

        class Context : CompilerContext
        {
            public Context(Program p)
                : base(new Report(new Pr(p)))
            {
            }
        }

        Compiler _i;

        void Run()
        {
            _i = new Compiler(new Context(this), MachineInfo.WindowsX86);

            //_i.Add();

            _i.Add(@"

// These constants won't change:
const int sensorPin = A0;    // pin that the sensor is attached to
const int ledPin = 9;        // pin that the LED is attached to

// variables:
int sensorValue = 0;         // the sensor value
int sensorMin = 1023;        // minimum sensor value
int sensorMax = 0;           // maximum sensor value


void setup() {
  // turn on LED to signal the start of the calibration period:
  pinMode(13, OUTPUT);
  digitalWrite(13, HIGH);

  // calibrate during the first five seconds 
  while (millis() < 5000) {
    sensorValue = analogRead(sensorPin);

    // record the maximum sensor value
    if (sensorValue > sensorMax) {
      sensorMax = sensorValue;
    }

    // record the minimum sensor value
    if (sensorValue < sensorMin) {
      sensorMin = sensorValue;
    }
  }

  // signal the end of the calibration period
  digitalWrite(13, LOW);
}

void loop() {
  // read the sensor:
  sensorValue = analogRead(sensorPin);

  // apply the calibration to the sensor reading
  sensorValue = map(sensorValue, sensorMin, sensorMax, 0, 255);

  // in case the sensor value is outside the range seen during calibration
  sensorValue = constrain(sensorValue, 0, 255);

  // fade the LED using the calibrated value:
  analogWrite(ledPin, sensorValue);
}

");
            
            
            /*

            for (; ; )
            {
                var code = "";
                var ready = false;
                var head = "";
                while (!ready)
                {
                    var line = Console.ReadLine();
                    code += head;
                    code += line;
                    head = "\n";
                    ready = line.Trim().EndsWith(";");
                }
                _i.Add(code);
            }*/
        }

        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
