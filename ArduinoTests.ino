



void printSizeOf (const char *name, int value)
{
  Serial.print ("sizeof (");
  Serial.print (name);
  Serial.print (") = ");
  Serial.println (value);
}

void printType (const char *typeExpression, const char *typeName)
{
  Serial.print (typeExpression);
  Serial.print (" = ");
  Serial.println (typeName);
}

void printValue (const char *name, int value)
{
  Serial.print (name);
  Serial.print (" = ");
  Serial.println (value);
}

void setup ()
{
  Serial.begin(9600);
}

const char *name(char val) {  return "c";}
const char *name(unsigned char val){  return "uc";}
const char *name(short val){  return "s";}
const char *name(unsigned short val){  return "us";}
const char *name(int val){  return "i";}
const char *name(unsigned int val){  return "ui";}
const char *name(long val){  return "l";}
const char *name(unsigned long val) { return "ul"; }

#define SHOW(type, op) \
  printType (#type #op "c", name ((type) op (c))); \
  printType (#type #op "uc", name ((type) op (uc))); \
  printType (#type #op "s", name ((type) op (s))); \
  printType (#type #op "us", name ((type) op (us))); \
  printType (#type #op "i", name ((type) op (i))); \
  printType (#type #op "ui", name ((type) op (ui))); \
  printType (#type #op "l", name ((type) op (l))); \
  printType (#type #op "ul", name ((type) op (ul)));


void loop ()
{
  char c = 1;
  unsigned char uc = 1;
  short s = 1;
  unsigned short us = 1;
  int i = 1;
  unsigned int ui = 1;
  long l = 1;
  unsigned long ul = 1;
  
  Serial.println ("=============================================");
  
  printSizeOf ("char", sizeof (char));
  printSizeOf ("short int", sizeof (short int));
  printSizeOf ("long int", sizeof (long int));
  printSizeOf ("long long int", sizeof (long long int));
  printSizeOf ("int", sizeof (int));
  printSizeOf ("int*", sizeof (int*));
  printSizeOf ("long*", sizeof (long*));
  
  printValue ("HIGH", HIGH);
  printValue ("LOW", LOW);
  printValue ("INPUT", INPUT);
  printValue ("INPUT_PULLUP", INPUT_PULLUP);
  printValue ("OUTPUT", OUTPUT);
  printValue ("true", true);
  printValue ("false", false);
  
  SHOW (c, +);
  SHOW (uc, +);
  SHOW (s, +);
  SHOW (us, +);
  SHOW (i, +);
  SHOW (ui, +);
  SHOW (l, +);
  SHOW (ul, +);
  
  /*SHOW (c, &&);
  SHOW (uc, &&);
  SHOW (s, &&);
  SHOW (us, &&);
  SHOW (i, &&);
  SHOW (ui, &&);
  SHOW (l, &&);
  SHOW (ul, &&);

  SHOW (c, |);
  SHOW (uc, |);
  SHOW (s, |);
  SHOW (us, |);
  SHOW (i, |);
  SHOW (ui, |);
  SHOW (l, |);
  SHOW (ul, |);*/

}

