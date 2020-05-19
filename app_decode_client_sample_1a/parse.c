#if defined(WIN32) || defined(_WIN32) || defined(WIN64) || defined(_WIN64)

#include <Windows.h>

#ifndef STRCASECMP
#define STRCASECMP  _stricmp
#endif
#ifndef STRNCASECMP
#define STRNCASECMP _strnicmp
#endif
#else // Linux
#ifndef STRCASECMP
#define STRCASECMP  strcasecmp
#endif
#ifndef STRNCASECMP
#define STRNCASECMP strncasecmp
#endif
#endif

// avoid linux compile warnings
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdbool.h>

//------------------------------------------------------------------------------

int stringRemoveDelimiter(char delimiter, const char *string)
{
   int string_start = 0;

   while (string[string_start] == delimiter)
   {
      string_start++;
   }

   if (string_start >= (int)strlen(string) - 1)
   {
      return 0;
   }

   return string_start;
}

//------------------------------------------------------------------------------

int checkCmdLineFlag(const int argc, const char **argv, const char *string_ref)
{
   bool bFound = false;

   if (argc >= 1)
   {
      for (int i = 1; i < argc; i++)
      {
         int string_start = stringRemoveDelimiter('-', argv[i]);
         const char *string_argv = &argv[i][string_start];

         const char *equal_pos = strchr(string_argv, '=');
         int argv_length = (int)(equal_pos == 0 ? strlen(string_argv) : equal_pos - string_argv);

         int length = (int)strlen(string_ref);

         if (length == argv_length && !STRNCASECMP(string_argv, string_ref, length))
         {

            bFound = true;
            break;
         }
      }
   }
   return (int)bFound;
}

//------------------------------------------------------------------------------

bool getCmdLineArgumentString(const int argc, const char **argv,
   const char *string_ref, char **string_retval)
{
   bool bFound = false;

   if (argc >= 1)
   {
      for (int i = 1; i < argc; i++)
      {
         int string_start = stringRemoveDelimiter('-', argv[i]);
         char *string_argv = (char *)&argv[i][string_start];
         int length = (int)strlen(string_ref);

         if (!STRNCASECMP(string_argv, string_ref, length))
         {
            *string_retval = &string_argv[length + 1];
            bFound = true;
            break;
         }
      }
   }

   if (!bFound)
      *string_retval = NULL;

   return bFound;
}

//------------------------------------------------------------------------------

int getCmdLineArgumentInt(const int argc, const char **argv, const char *string_ref)
{
   bool bFound = false;
   int value = -1;

   if (argc >= 1)
   {
      for (int i = 1; i < argc; i++)
      {
         int string_start = stringRemoveDelimiter('-', argv[i]);
         const char *string_argv = &argv[i][string_start];
         int length = (int)strlen(string_ref);

         if (!STRNCASECMP(string_argv, string_ref, length))
         {
            if (length + 1 <= (int)strlen(string_argv))
            {
               int auto_inc = (string_argv[length] == '=') ? 1 : 0;
               value = atoi(&string_argv[length + auto_inc]);
            }
            else
            {
               value = 0;
            }

            bFound = true;
            break;
         }
      }
   }

   if (bFound)
      return value;
   else
      return 0;
}

//------------------------------------------------------------------------------