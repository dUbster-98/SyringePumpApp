﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SyringePumpTest1.Services
{
    public interface IIniSetService
    {
        void SetIni(string Section, string Key, string Value, string path);
        string GetIni(string Section, string Key, string path);
    }
    public class IniSetService : IIniSetService
    {
        ////++++++++++++++++++++++++++++++++++++++++++++++++++
        /// INI 파일 만들기(쓰기)
        /// <summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        ////++++++++++++++++++++++++++++++++++++++++++++++++++
        /// INI 파일 가져오기(읽기)
        /// <summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// </summary>
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string def, StringBuilder retVal, int Size, string FilePath);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, int Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int Section, string Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);

        // INI File path
        private static string currentDirectory = Directory.GetCurrentDirectory();
        private static string settingsDirectory = Path.Combine(currentDirectory, "..");
        public static string path1 = settingsDirectory + "\\common\\ref1.ini";

        public IniSetService()
        {

        }

        /// INI 파일에 쓰기
        public void SetIni(string Section, string Key, string Value, string path)
        {
            WritePrivateProfileString(Section, Key, Value, path);
        }

        /// INI 파일 읽기
        public string GetIni(string Section, string Key, string path)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            GetPrivateProfileString(Section, Key, null, stringBuilder, 1024, path);
            return stringBuilder.ToString();
        }
    }
}
