#pragma once

extern char gURL[];
extern LONG gx;
extern LONG gy;
extern LONG gw;
extern LONG gh;
extern BOOL gBorders;
extern unsigned gReserved;

BOOL parseDefaultsandCmdLine(int argc, char *argv[], MM_CLIENT_OPEN* pOpenParms, MM_CLIENT_PLAY* pPlayParms);
void displayHelp();
