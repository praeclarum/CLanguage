#include <stdio.h>


void printType (const char *name, const char *ty)
{
  printf ("%s = %s\n", name, ty);
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


int main()
{
  printf("%zu\n", sizeof(int));
}

int main2 ()
{
  char c = 1;
  unsigned char uc = 1;
  short s = 1;
  unsigned short us = 1;
  int i = 1;
  unsigned int ui = 1;
  long l = 1;
  unsigned long ul = 1;
  
  SHOW (c, +);
  SHOW (uc, +);
  SHOW (s, +);
  SHOW (us, +);
  SHOW (i, +);
  SHOW (ui, +);
  SHOW (l, +);
  SHOW (ul, +);
  
  SHOW (c, -);
  SHOW (uc, -);
  SHOW (s, -);
  SHOW (us, -);
  SHOW (i, -);
  SHOW (ui, -);
  SHOW (l, -);
  SHOW (ul, -);
  
  SHOW (c, *);
  SHOW (uc, *);
  SHOW (s, *);
  SHOW (us, *);
  SHOW (i, *);
  SHOW (ui, *);
  SHOW (l, *);
  SHOW (ul, *);
  
  SHOW (c, /);
  SHOW (uc, /);
  SHOW (s, /);
  SHOW (us, /);
  SHOW (i, /);
  SHOW (ui, /);
  SHOW (l, /);
  SHOW (ul, /);

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
