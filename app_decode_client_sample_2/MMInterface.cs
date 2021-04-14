using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Text;

using MultiMedia;

namespace MM.SDK
{
   public enum MM_TASK_ITEM // priority order - low to high
   {
      MM_NONE = 0xFF,
      MM_STOP = 0x0,
      MM_CLOSE = 0x1,
      MM_CLOSED = 0x11,
      MM_OPEN = 0x2,
      MM_OPENED = 0x22,
      MM_PLAY = 0x4,
      MM_PLAYING = 0x44,
      MM_PAUSE = 0x8,
      MM_PAUSED = 0x88,
      MM_DICT = 0x10,
   };
   public class MM_TASK
   {
      public MM_TASK(MM_TASK_ITEM item, mmSessionDictionaryKeys dictKey, IntPtr lpData, int dataSize)
      {
         _item = item;
         _dictKey = dictKey;
         if (dataSize > 0) // not using internal parameters (_parms)
         {
            try
            {
               _lpData = Marshal.AllocHGlobal(dataSize);
               MMInterop.memcpy(_lpData, lpData, new UIntPtr((uint)dataSize));
            }
            catch { Debug.Assert(false); }
         }
      }
      public MM_TASK_ITEM _item;
      public mmSessionDictionaryKeys _dictKey;
      public IntPtr _lpData;
   };

   public class MMInterface
   {
      public IntPtr _hSession = IntPtr.Zero;
      public MMParameters _parms = null;
      public MM_TASK_ITEM _state = MM_TASK_ITEM.MM_NONE;
      private WindowParent _windowParent = null;
      private List<MM_TASK> _taskList;
      private readonly object _taskLock = new object();
      private Thread _thread = null;
      private EventWaitHandle _ewh = null;
      private bool _run = true;
      
      public MMInterface(MMParameters parmsBlock, WindowParent window)
      {
         _parms = parmsBlock;
         _windowParent = window;
         _taskList = new List<MM_TASK>();
         _thread = new Thread(threadLoop);
         _thread.Name = "mmInterface";
         _ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
         _thread.Start();
      }
      public void InvokeMMTask(MM_TASK newTask)
      {
         if (!_run)
            return;
     
         lock (_taskLock)
         {
            // at this point, drop all other tasks for the session
            if (newTask._item == MM_TASK_ITEM.MM_CLOSE || newTask._item ==  MM_TASK_ITEM.MM_STOP)
            {
               foreach (var task in _taskList)
                  FreeTask(task);

               _taskList.Clear();
               _taskList.Add(newTask);
            }
            else
            {
               MM_TASK tasksToReplace = null; // ie multiple PLAY commands in the list

               foreach (var task in _taskList)
                  if (task._item == newTask._item &&
                     task._dictKey == newTask._dictKey &&
                     ReplaceableTask(task)) // optomize our list of tasks
                  {
                     tasksToReplace = task;
                     break;
                  }

               if (tasksToReplace != null) // keep the order and drop any out of date tasks
               {
#if REPLACE_TASK_IN_ITS_ORG_ORDER
                  int index = _taskList.IndexOf(tasksToReplace); // replace a task
                  if (index != -1)
                  {
                     if (tasksToReplace._lpData != IntPtr.Zero)
                        FreeTask(tasksToReplace);
                     _taskList[index] = newTask; // keep the tasks original order
                  }
                  else
                     Debug.Assert(false)
#else
                  _taskList.Remove(tasksToReplace);
                  if (tasksToReplace._lpData != IntPtr.Zero)
                     FreeTask(tasksToReplace);
#endif
               }
               _taskList.Add(newTask); // this will add to the end of the list
            }
            _ewh.Set(); // wake up the task thread
         }
      }
      private MM_TASK PopNextTask()
      {
         lock (_taskLock)
         {
#if GET_HIGHEST_PRIORITY_TASK_FIRST
            MM_TASK nextTask = new MM_TASK(MM_TASK_ITEM.MM_NONE, 0, IntPtr.Zero, 0);

            foreach (var task in _taskList) // iterating from the first added item to the last
               if (task._item < nextTask._item) nextTask = task; // get highest priority or oldest of any duplicates (DICT only)

            if (nextTask._item != MM_TASK_ITEM.MM_NONE)
               _taskList.Remove(nextTask);

            return nextTask;
#else
            MM_TASK nextTask;
            if (_taskList.Count > 0)
            {
               nextTask = _taskList[0];
               _taskList.Remove(nextTask);
            }
            else
               nextTask = new MM_TASK(MM_TASK_ITEM.MM_NONE, 0, IntPtr.Zero, 0);

            return nextTask;
#endif
         }
         
      }
      private void executeTask(MM_TASK task)
      {
         mmStatus sts = mmStatus.MM_STS_NONE;
         switch (task._item)
         {
            case MM_TASK_ITEM.MM_STOP:
               _run = false;
               goto case MM_TASK_ITEM.MM_CLOSE;
            case MM_TASK_ITEM.MM_CLOSE:
                _state = MM_TASK_ITEM.MM_CLOSE;
               sts = closeSession(task._dictKey, task._lpData);
               _state = MM_TASK_ITEM.MM_CLOSED;
               break;
            case MM_TASK_ITEM.MM_OPEN:
               _state = MM_TASK_ITEM.MM_OPEN;
               sts = openSesssion(task._dictKey, task._lpData);
               _state = MM_TASK_ITEM.MM_OPENED;
               break;
            case MM_TASK_ITEM.MM_PLAY:
               _state = MM_TASK_ITEM.MM_PLAY;
               sts = playSesssion(task._dictKey, task._lpData);
               _state = MM_TASK_ITEM.MM_PLAYING;
               break;
            case MM_TASK_ITEM.MM_PAUSE:
               _state = MM_TASK_ITEM.MM_PAUSE;
               sts = pauseSesssion(task._dictKey, task._lpData);
               _state = MM_TASK_ITEM.MM_PAUSED;
               break;
            case MM_TASK_ITEM.MM_DICT:
               sts = dictSession(task._dictKey, task._lpData);
               break;
            default:
               Debug.Assert(false);
               break;
         }
      }
      private void threadLoop()
      {
         while (_run)
         {
            MM_TASK task = PopNextTask();

            if (task._item == MM_TASK_ITEM.MM_NONE)
               _ewh.WaitOne();
            else
               executeTask(task);

            FreeTask(task);
         }
      }
      private mmStatus dictSession(mmSessionDictionaryKeys flags, IntPtr lpData)
      {
         mmStatus sts = mmStatus.MM_STS_NONE;
         if (_hSession == IntPtr.Zero)
            return mmStatus.MM_STS_LIB_ERROR_INVALID_STATE;

         try
         {
            // use either local structure (startup) or lpData from synchronized SendMessage
            switch ((mmSessionDictionaryKeys)flags)
            {
               case mmSessionDictionaryKeys.CLI_DEWARP:
                  if (lpData != IntPtr.Zero)
                     _parms.Dewarp.DewarpParms = (MM_CLIENT_DEWARP)Marshal.PtrToStructure(lpData, typeof(MM_CLIENT_DEWARP));
                  else
                  {
                     lpData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MM_CLIENT_DEWARP)));
                     Marshal.StructureToPtr(_parms.Dewarp.DewarpParms, lpData, false);
                  }
                  break;
               case mmSessionDictionaryKeys.CLI_SOURCE:
                  if (lpData != IntPtr.Zero)
                  {
                     MM_CLIENT_SOURCE source = (MM_CLIENT_SOURCE)Marshal.PtrToStructure(lpData, typeof(MM_CLIENT_SOURCE));
                     if ((source.Size == (uint)Marshal.SizeOf(typeof(MM_CLIENT_SOURCE))))
                     {
                        if ((source.Flags & MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_RENDER) == MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_RENDER)
                           _parms.Source.SourceParms.BRender = source.BRender;
                        if ((source.Flags & MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_ASPECT_RATIO) == MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_ASPECT_RATIO)
                           _parms.Source.SourceParms.BEnforceAR = source.BEnforceAR;

                        _parms.Source.SourceParms.Flags |= source.Flags;
                     }
                     else
                        return mmStatus.MM_STS_SRC_ERROR_INCOMPATIBLE_API;
                  }
                  else
                  {
                     lpData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MM_CLIENT_SOURCE)));
                     Marshal.StructureToPtr(_parms.Source.SourceParms, lpData, false);
                  }
                  break;
               case mmSessionDictionaryKeys.CLI_ZOOM:
                  if (lpData != IntPtr.Zero)
                     _parms.Zoom.ZoomParms = (MM_RECT)Marshal.PtrToStructure(lpData, typeof(MM_RECT));
                  else
                  {
                     lpData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MM_RECT)));
                     Marshal.StructureToPtr(_parms.Zoom.ZoomParms, lpData, false);
                  }
                  break;
               case mmSessionDictionaryKeys.CLI_CHILD:
                  if (lpData == IntPtr.Zero)
                     Debug.Assert(false);
                  break;
               case mmSessionDictionaryKeys.CLI_RESET:
                  break;
               default:
                  break;
            }

            sts = mmMethods.mmDictionarySet(_hSession, (mmSessionDictionaryKeys)flags, lpData);
         }
         catch { Debug.Assert(false); }

         return sts;
      }
      private mmStatus openSesssion(mmSessionDictionaryKeys flags, IntPtr lpData)
      {
         if (_hSession != IntPtr.Zero)
            return mmStatus.MM_STS_LIB_ERROR_INVALID_STATE;

         if (lpData != IntPtr.Zero) // else a re-connect on error case using existing params
         {
            _parms.Open.OpenParms = (MM_CLIENT_OPEN)Marshal.PtrToStructure(lpData, typeof(MM_CLIENT_OPEN));
            if (_parms.Open.OpenParms.Size == (Marshal.SizeOf(typeof(MM_CLIENT_OPEN))))
            {
               _parms.Open.URL = MMInterop.PtrToString(lpData + Marshal.SizeOf(typeof(MM_CLIENT_OPEN)));
               _parms.Open.UserName = MMInterop.PtrToString(lpData + Marshal.SizeOf(typeof(MM_CLIENT_OPEN)) + (2 * (_parms.Open.URL.Length + 1)));
               _parms.Open.PassWord = MMInterop.PtrToString(lpData + Marshal.SizeOf(typeof(MM_CLIENT_OPEN)) + (2 * ((_parms.Open.URL.Length + 1) + (_parms.Open.UserName.Length + 1))));
            }
            else
               return mmStatus.MM_STS_SRC_ERROR_INCOMPATIBLE_API;
         }
         
         string basicAuth = _parms.Open.URL;
         if( (uint)flags != 0x80000000) // playlist case
            _windowParent.LockPaintSessionStatus("Opening URL: " + _parms.Open.URL);

         _windowParent.LockSetSessionWindowText(_parms.Open.URL);

         if (!string.IsNullOrEmpty(_parms.Open.UserName) && !string.IsNullOrEmpty(_parms.Open.PassWord))
         {
            var index = _parms.Open.URL.IndexOf("://");
            if (index != -1)
               basicAuth = basicAuth.Insert(index + 3, _parms.Open.UserName + ":" + _parms.Open.PassWord + "@");
         }

         _parms.Open.OpenParms.HWnd = _windowParent.GetHWND();

         // account for UTF8 encoded data
         int len = Encoding.UTF8.GetByteCount(basicAuth);
         byte[] utf8Bytes = new byte[len + 1];
         Encoding.UTF8.GetBytes(basicAuth, 0, basicAuth.Length, utf8Bytes, 0);
         _parms.Open.OpenParms.PURL = Marshal.AllocHGlobal(utf8Bytes.Length); 
         Marshal.Copy(utf8Bytes, 0, _parms.Open.OpenParms.PURL, utf8Bytes.Length);

         if (_parms.Open.Reserved != 0)
         {
            _parms.Open.OpenParms.PReserved = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            Marshal.WriteInt32(_parms.Open.OpenParms.PReserved, 0, ((int)_parms.Open.Reserved));
         }
         else
            _parms.Open.OpenParms.PReserved = IntPtr.Zero;

         mmStatus sts = mmMethods.mmClientOpen(out _hSession, ref _parms.Open.OpenParms);
         if (sts != mmStatus.MM_STS_NONE)
         {
            _windowParent.LockPaintSessionStatus($"STATUS 0x{sts:X} Opening URL: " + _parms.Open.URL);
            _windowParent._watchDog.Enabled = true;
            // start a fresh
            _parms.Play.StartTime = null;
            _parms.Play.EndTime = null;
         }
         else
            _windowParent.LockPaintSessionStatus("");

         if (_parms.Open.OpenParms.PReserved != IntPtr.Zero)
            Marshal.FreeHGlobal(_parms.Open.OpenParms.PReserved);
         
         Marshal.FreeHGlobal(_parms.Open.OpenParms.PURL);
         return sts;
      }
      private mmStatus pauseSesssion(mmSessionDictionaryKeys flags, IntPtr lpData)
      {
         if (_hSession == IntPtr.Zero)
            return mmStatus.MM_STS_LIB_ERROR_INVALID_STATE;

         return mmMethods.mmClientPause(_hSession);
      }
      private mmStatus playSesssion(mmSessionDictionaryKeys flags, IntPtr lpData)
      {
         if (_hSession == IntPtr.Zero)
            return mmStatus.MM_STS_LIB_ERROR_INVALID_STATE;

         if (lpData != IntPtr.Zero) // else a re-connect on error case using existing params
         {
            _parms.Play.PlayParms = (MM_CLIENT_PLAY)Marshal.PtrToStructure(lpData, typeof(MM_CLIENT_PLAY));
            if (_parms.Play.PlayParms.Size == (Marshal.SizeOf(typeof(MM_CLIENT_PLAY))))
            {
               _parms.Play.StartTime = MMInterop.PtrToString(lpData + Marshal.SizeOf(typeof(MM_CLIENT_PLAY)));
               _parms.Play.EndTime = MMInterop.PtrToString(lpData + Marshal.SizeOf(typeof(MM_CLIENT_PLAY)) + (2 * (_parms.Play.StartTime.Length + 1)));
            }
            else
               return mmStatus.MM_STS_SRC_ERROR_INCOMPATIBLE_API;
         }

         if (string.IsNullOrEmpty(_parms.Play.StartTime))
            _parms.Play.PlayParms.PStartTime = IntPtr.Zero;
         else
            _parms.Play.PlayParms.PStartTime = (IntPtr)Marshal.StringToHGlobalAnsi(_parms.Play.StartTime);

         if (string.IsNullOrEmpty(_parms.Play.EndTime))
            _parms.Play.PlayParms.PEndTime = IntPtr.Zero;
         else
            _parms.Play.PlayParms.PEndTime = (IntPtr)Marshal.StringToHGlobalAnsi(_parms.Play.EndTime);

         mmStatus sts = mmMethods.mmClientPlay(_hSession, ref _parms.Play.PlayParms);

         if (_parms.Play.PlayParms.PStartTime != IntPtr.Zero)
            Marshal.FreeHGlobal(_parms.Play.PlayParms.PStartTime);
         if (_parms.Play.PlayParms.PEndTime != IntPtr.Zero)
            Marshal.FreeHGlobal(_parms.Play.PlayParms.PEndTime);

         return sts; // any play error makes its way back via the callback
      }
      private mmStatus closeSession(mmSessionDictionaryKeys flags, IntPtr lpData)
      {
         mmStatus sts = mmStatus.MM_STS_NONE;
         if (_hSession == IntPtr.Zero)
            sts = mmStatus.MM_STS_LIB_ERROR_INVALID_STATE;
         else
         {
            sts = mmMethods.mmClose(_hSession);
            _hSession = IntPtr.Zero;
         }
         return sts;
      }
      public void Stop()
      {           
         do // drain task list, safe as we do not use _taskLock in threadLoop()
         {
            MM_TASK task = PopNextTask();
            if (task._item == MM_TASK_ITEM.MM_NONE)
               break;
            else
               FreeTask(task);
         }
         while (true);

         if (_thread != null)
         {
            // stop the task thread threadLoop()
            InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_STOP, 0, IntPtr.Zero, 0));
            if (_thread.Join(35000) == false) // mmOpen could of just been called and not return for 20+ seconds..
               Debug.Assert(false);
         }
         // stay in sync with the calling thread
         _ewh.Close();
      }
      private void FreeTask(MM_TASK task)
      {
         if (task._lpData != IntPtr.Zero)
            Marshal.FreeHGlobal(task._lpData);
         task = null;
      }
      private bool ReplaceableTask(MM_TASK task)
      {
         switch (task._dictKey)
         {
            case mmSessionDictionaryKeys.NO_OP:
               return true;
            case mmSessionDictionaryKeys.CLI_DEWARP:
               return true;
            case mmSessionDictionaryKeys.CLI_SOURCE:
               return false; // TODO: start inspecting structure flags
            case mmSessionDictionaryKeys.CLI_ZOOM:
               return true;
            case mmSessionDictionaryKeys.CLI_CHILD:
               return false; // TODO: start inspecting structure flags
            case mmSessionDictionaryKeys.CLI_RESET:
               return true;
            default:
               Debug.Assert(false);
               return false;
         }
      }
   }
}