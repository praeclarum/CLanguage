using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Syntax;
using CLanguage.Types;
using static CLanguage.CLanguageService;
using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Tests
{
    [TestClass]
    public class ArduinoInterpreterTests
    {
        ArduinoTestMachineInfo.TestArduino Run (string code)
        {
            var machine = new ArduinoTestMachineInfo ();
            var fullCode = code + "\n\nvoid main() { __cinit(); setup(); while(1){loop();}}";
            var i = CLanguageService.CreateInterpreter (fullCode, machine, new TestPrinter ());
            i.Reset ("main");
            i.Step ();
            return machine.Arduino;
        }

        [TestMethod]
        public void Sizes ()
        {
            var printer = new TestPrinter ();
            var mi = new TestMachineInfo ();
            var ec = new ExecutableContext (new Executable (mi), new Report (printer));

            Interpreter.CompiledVariable ParseVariable (string code)
            {
                var exe = Compile (code, mi, printer);
                return exe.Globals.Skip (1).First ();
            }

            var charV = ParseVariable ("char v;");
            Assert.AreEqual (1, charV.VariableType.GetByteSize (ec));

            var intV = ParseVariable ("int v;");
            Assert.AreEqual (2, intV.VariableType.GetByteSize (ec));

            var shortIntV = ParseVariable ("short int v;");
            Assert.AreEqual (2, shortIntV.VariableType.GetByteSize (ec));

            var unsignedLongV = ParseVariable ("unsigned long v;");
            Assert.AreEqual (4, unsignedLongV.VariableType.GetByteSize (ec));

            var unsignedLongLongV = ParseVariable ("unsigned long long v;");
            Assert.AreEqual (8, unsignedLongLongV.VariableType.GetByteSize (ec));

            var intPV = ParseVariable ("int *v;");
            Assert.AreEqual (2, intPV.VariableType.GetByteSize (ec));

            var longPV = ParseVariable ("long *v;");
            Assert.AreEqual (2, longPV.VariableType.GetByteSize (ec));
        }

        public const string BlinkCode = @"
void setup() {                
  // initialize the digital pin as an output.
  // Pin 13 has an LED connected on most Arduino boards:
  pinMode(13, OUTPUT);     
}

void loop() {
  digitalWrite(13, HIGH);   // set the LED on
  delay(1000);              // wait for a second
  digitalWrite(13, LOW);    // set the LED off
  delay(1000);              // wait for a second
}
";

        [TestMethod]
        public void Blink ()
        {
            var arduino = Run (BlinkCode);

            Assert.AreEqual (1, arduino.Pins[13].Mode);
        }

        [TestMethod]
        public void InternalLocalCtorTest ()
        {
            var code = @"
void setup() {
  Serial.begin(9600);
}

void loop() {
  CtorTest ctor(1234);
  Serial.println(ctor.x);
  delay(1);
}";
            var arduino = Run (code);
            Assert.AreEqual ("1234", arduino.SerialOut.ToString ().Split ("\n").First ().Trim ());
        }

        [TestMethod]
        public void InternalGlobalCtorTest ()
        {
            var code = @"
void setup() {
  Serial.begin(9600);
}
CtorTest ctor(5678);
void loop() {  
  Serial.println(ctor.x);
  delay(1);
}";
            var arduino = Run (code);
            Assert.AreEqual ("5678", arduino.SerialOut.ToString ().Split ("\n").First ().Trim ());
        }

        [TestMethod]
        public void AnalogReadSerial ()
        {
            var code = @"
void setup() {
  Serial.begin(9600);
}

void loop() {
  int sensorValue = analogRead(A0);
  Serial.println(sensorValue);
  delay(1);
}";
            var arduino = Run (code);
            Assert.AreEqual ("42", arduino.SerialOut.ToString ().Split ("\n").First ().Trim ());
        }

        [TestMethod]
        public void DigitalReadSerial ()
        {
            var code = @"
void setup() {
  Serial.begin(9600);
  pinMode(2, INPUT);
}

void loop() {
  int sensorValue = digitalRead(2);
  Serial.println(sensorValue, DEC);
}
";
            var arduino = Run (code);
            Assert.AreEqual ("0", arduino.SerialOut.ToString ().Split ("\n").First ().Trim ());
        }

        [TestMethod]
        public void DigitalRead ()
        {
            var code = @"
void setup() {
  pinMode(2, INPUT);
  pinMode(3, OUTPUT);
}

void loop() {
  int sensorValue = digitalRead(2);
  digitalWrite(3, sensorValue);
}
";
            var arduino = Run (code);
            Assert.AreEqual (0, arduino.Pins[2].Mode);
            Assert.AreEqual (1, arduino.Pins[3].Mode);
        }

        public const string FadeCode = @"
int brightness = 0;    // how bright the LED is
int fadeAmount = 5;    // how many points to fade the LED by

void setup()  { 
  // declare pin 9 to be an output:
  pinMode(9, OUTPUT);
} 

void loop()  { 
  // set the brightness of pin 9:
  analogWrite(9, brightness);    

  // change the brightness for next time through the loop:
  brightness = brightness + fadeAmount;

  // reverse the direction of the fading at the ends of the fade: 
  if (brightness == 0 || brightness == 255) {
    fadeAmount = -fadeAmount ; 
  }     
  // wait for 30 milliseconds to see the dimming effect    
  delay(30);                            
}
";

        [TestMethod]
        public void Fade ()
        {
            var arduino = Run (FadeCode);
        }

        [TestMethod]
        public void Tone ()
        {
            var code = @"
int melody[] = {
  NOTE_C4, NOTE_G3,NOTE_G3, NOTE_A3, NOTE_G3,0, NOTE_B3, NOTE_C4};

// note durations: 4 = quarter note, 8 = eighth note, etc.:
int noteDurations[] = {
  4, 8, 8, 4,4,4,4,4 };

void setup() {
  // iterate over the notes of the melody:
  for (int thisNote = 0; thisNote < 8; thisNote++) {

    // to calculate the note duration, take one second 
    // divided by the note type.
    //e.g. quarter note = 1000 / 4, eighth note = 1000/8, etc.
    int noteDuration = 1000/noteDurations[thisNote];
    tone(8, melody[thisNote],noteDuration);

    // to distinguish the notes, set a minimum time between them.
    // the note's duration + 30% seems to work well:
    int pauseBetweenNotes = noteDuration * 1.30;
    delay(pauseBetweenNotes);
    // stop the tone playing:
    noTone(8);
  }
}

void loop() {
  // no need to repeat the melody.
}
";
            var arduino = Run (code);
        }

        [TestMethod]
        public void Calibration ()
        {
            var code = @"
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
  while (millis() < 1) {
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
";
            var arduino = Run (code);
        }

        [TestMethod]
        public void StateChangeDetection ()
        {
            var code = @"

// this constant won't change:
const int  buttonPin = 2;    // the pin that the pushbutton is attached to
const int ledPin = 13;       // the pin that the LED is attached to

// Variables will change:
int buttonPushCounter = 0;   // counter for the number of button presses
int buttonState = 0;         // current state of the button
int lastButtonState = 0;     // previous state of the button

void setup() {
  // initialize the button pin as a input:
  pinMode(buttonPin, INPUT);
  // initialize the LED as an output:
  pinMode(ledPin, OUTPUT);
  // initialize serial communication:
  Serial.begin(9600);
  Serial.println(""start"");
}


void loop() {
  // read the pushbutton input pin:
  buttonState = digitalRead(buttonPin);

  // compare the buttonState to its previous state
   if (buttonState != lastButtonState) {
    // if the state has changed, increment the counter
     if (buttonState == HIGH) {
      // if the current state is HIGH then the button went from off to on:
       buttonPushCounter++;
       Serial.println(""on"");
       Serial.print (""number of button pushes: "");
            Serial.println (buttonPushCounter);
        } else {
      // if the current state is LOW then the button went from on to off:
       Serial.println(""off"");
    }
    // Delay a little bit to avoid bouncing
     delay (50);
    }
// save the current state as the last state, for next time through the loop
lastButtonState = buttonState;


  // turns on the LED every four button pushes by checking the modulo of the
  // button push counter. the modulo function gives you the remainder of the
  // division of two numbers:
   if (buttonPushCounter % 4 == 0) {
     digitalWrite (ledPin, HIGH);
   } else {
     digitalWrite (ledPin, LOW);
   }

}";
            var arduino = Run (code);
            Assert.AreEqual ("start", arduino.SerialOut.ToString ().Split ("\n").First ().Trim ());
        }

        [TestMethod]
        public void UserBug0 ()
        {
            var code = @"

/* 
7 Segment LED Display Pin Connects to Arduino Digital Terminal ... 
a 2 
b 3 
c 4 
d 6 
e 7 
f 9 
g 8 
DP 5 
*/ 
#define DIST_LEFT_PIN A4 
#define DIST_RIGHT_PIN A5 
#define PWM_PIN 10 
float sensorValue, E, wireTemperature, amps, pwmVal,prevLeft, prevRight, distRight, distLeft, power; 
float Vcc = 5000.0; 
boolean warnLeft = false, warnRight = false; 
// a b c d e f g 
// 1 1 1 0 1 1 0 0 0 
//bits representing numerals 0-9 
const byte numeral[18] = { 
B11111100, //0 
B01100000, //1 
B11011010, //2 
B11110010, //3 
B01100110, //4 
B10110110, //5 
B00111110, //6 
B11100000, //7 
B11111110, //8 
B11100110, //9 
B11101100, //A 
B00111110, //B 
B10011100, //C 
B01111100, //D 
B10011110, //E 
B10001110, //F 
B00000000, //shows nothing 
};

//pins for each segment (a-g) on the 7 segment LED display with the corresponding arduino connection 
const int segmentPins[8] = { 5, 8, 9, 7, 6, 4, 3, 2 };

void setup() 
{ 
// initialize serial communication at 9600 bits per second: 
Serial.begin(9600);

for (int i = 0; i < 8; i++) 
{ 
pinMode(segmentPins[i], OUTPUT); 
} 
for (int i = 0; i <= 18; i++) 
{ 
showDigit(i); 
Serial.print(""\n setup: ""); 
Serial.print(i); 
delay(50); 
}

} 
void loop() 
{ int count = 0; 
float currentV, prevV;

distLeft = ReadDistance_cm(DIST_LEFT_PIN); 
distRight = ReadDistance_cm(DIST_RIGHT_PIN); 
if ((distRight <= 300) && (distRight <= prevRight)) { 
warnRight = true; 
} else warnRight = false;

if ((distLeft <= 300) && (distLeft <= prevLeft)) { 
warnLeft = true; 
} else warnLeft = false;

prevRight = distRight; 
prevLeft = distLeft; 
// display warning L or R

// read the input on analog pin 0: (this is the temperature adjustment pot) 
for (int i = 0; i <= 1000; i++) { 
sensorValue = analogRead(A0); 
E = E + sensorValue; 
count = i; 
} 
E = E / 1000; 
sensorValue = E; 
float voltage = sensorValue * (5.0 / 1023.0); 
float factor = (voltage) / 5.0; 
pwmVal = 255 * factor; 
if (pwmVal > 255) { 
pwmVal = 255; 
} 
analogWrite(PWM_PIN, pwmVal); 
delay(10);

for (int i = 0; i <= 18; i++) 
{ 
showDigit(i); 
Serial.print(""main loop: ""); 
Serial.print(i); 
Serial.print(""\n"");

delay(10); 
} 
Serial.print(""!!!!!!!!!!!!*************!!!!!!!!!\n""); 
delay(2000); //after LED segment shuts off, there is a 2-second delay 
}

void showDigit (int number) 
{ 
boolean isBitSet;

for (int segment = 1; segment < 8; segment++) 
{ 
isBitSet = bitRead(numeral[number], segment); 
digitalWrite(segmentPins[segment], isBitSet); 
} 
}

//returns the distance in cm 
float ReadDistance_cm(int pin) { 
int i; 
float volt = 0;

for (i = 0; i < 25; i++) { 
volt += analogRead(pin); 
} 
volt = volt / i; 
return ((volt * (Vcc / 1023)) / 9.765) * 2.54; 
}

float readVolts(int pin) { 
int i; 
float volt = 0;

for (i = 0; i < 25; i++) { 
volt += analogRead(pin); 
} 
volt = volt / i; 
return volt * (Vcc / 1023); 
}";
            var arduino = Run (code);
            Assert.IsTrue (arduino.SerialOut.ToString ().TrimStart ().StartsWith ("setup:", StringComparison.Ordinal));
        }
    }
}
