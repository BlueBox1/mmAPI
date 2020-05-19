#pragma once

extern "C" {

int checkCmdLineFlag(const int argc, const char **argv, const char *string_ref);
bool getCmdLineArgumentString(const int argc, const char **argv,
   const char *string_ref, char **string_retval);
int getCmdLineArgumentInt(const int argc, const char **argv, const char *string_ref);

}