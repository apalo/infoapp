/*
The MIT License (MIT)

Copyright (c) 2016 Andreas Palo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace InfoApp
{
    /**
     * Class used to get system info like windows edition and available memory
     */
    public class SystemInfo
    {
        public enum PrimaryKey { CurrentUser, LocalMachine, Users, ClassesRoot, CurrentConfig }

        private static string failString = "<read failed>";

        /**
         * Used as a struct in the GlobalMemoryStatusEx WinAPI call
         */
        [StructLayout(LayoutKind.Sequential)] // class members appear in the defined order in memory
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        /**
         * Get OS name, for example "Windows 10 Home"
         */
        public static string OSName
        {
            get
            {
                return GetRegistryValue(PrimaryKey.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", @"ProductName");
            }
        }

        /**
         * Get system path, likely this is "C:\Windows" 
         */
        public static string SystemPath
        {
            get
            {
                return GetRegistryValue(PrimaryKey.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", @"SystemRoot");
            }
        }

        /**
         * Gives a string representing total physical memory in MB
         */
        public static string GetPhysicalMemory()
        {
            ulong size;
            if (GetPhysicallyInstalledSystemMemory(out size))
            {
                return (size / 1024).ToString();
            }
            return SystemInfo.failString;
        }

        /**
         * Get non-reserved physical memory
         */
        public static string GetUsablePhysicalMemory()
        {
            MEMORYSTATUSEX mem = AllocateMemoryStruct();
            if (mem != null)
            {
                return (mem.ullTotalPhys / (1024*1024)).ToString();
            }
            return SystemInfo.failString;
        }

        /**
         * Get the total amount of physical memory available to the system
         */
        public static string GetAvailablePhysicalMemory()
        {
            MEMORYSTATUSEX mem = AllocateMemoryStruct();
            if (mem != null)
            {
                return (mem.ullAvailPhys / (1024*1024)).ToString();
            }
            return SystemInfo.failString;
        }

        /**
         * Get the percentage of non-reserved memory still available
         */
        public static string GetMemoryPercentFree()
        {
            MEMORYSTATUSEX mem = AllocateMemoryStruct();
            if (mem != null)
            {
                return (100 - mem.dwMemoryLoad).ToString();
            }
            return SystemInfo.failString;
        }

        /**
         * Get string representation of registry key value
         */
        public static string GetRegistryValue(PrimaryKey primaryKey, string subKeyPath, string subKey)
        {
            RegistryKey key;
            string returnValue;
            switch (primaryKey)
            {
                case PrimaryKey.LocalMachine:
                    key = Registry.LocalMachine;
                    break;
                case PrimaryKey.Users:
                    key = Registry.Users;
                    break;
                case PrimaryKey.ClassesRoot:
                    key = Registry.ClassesRoot;
                    break;
                case PrimaryKey.CurrentConfig:
                    key = Registry.CurrentConfig;
                    break;
                case PrimaryKey.CurrentUser:
                default:
                    key = Registry.CurrentUser;
                    break;
            }
            try
            {
                key = key.OpenSubKey(subKeyPath);
                returnValue = key.GetValue(subKey).ToString();
            }
            catch
            {
                returnValue = SystemInfo.failString;
            }
            key.Close();
            
            return returnValue;
        }

        /**
         * This is used in other methods that need info about available memory
         */
        private static MEMORYSTATUSEX AllocateMemoryStruct()
        {
            MEMORYSTATUSEX mem = new MEMORYSTATUSEX();
            mem.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            if (GlobalMemoryStatusEx(mem))
            {
                return mem;
            }
            return null;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out ulong memSize);
    }
}
