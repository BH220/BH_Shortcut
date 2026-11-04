using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH_Shortcut.Core.Utils
{
    public class RegistryHelper
    {
        public static bool AddKey(RegistryHive Type, string Machine, string Key, string Name, object Value)
        {
            try
            {
                string existValue = null;
                object obj = GetKeyValue(Type, Machine, Key, Name);
                if (obj != null)
                {
                    existValue = obj.ToString();
                }
                if (existValue == null || string.Equals(existValue, (string)Value.ToString(), StringComparison.CurrentCultureIgnoreCase) != true)
                {
                    RegistryKey RegKey = null;
                    RegistryKey NewKey = null;
                    try
                    {
                        RegKey = RegistryKey.OpenRemoteBaseKey(Type, Machine);
                        NewKey = RegKey.CreateSubKey(Key);
                        NewKey.SetValue(Name, Value);
                    }
                    finally
                    {
                        NewKey.Flush();                       // Store the value 시간이 많이 걸림
                        NewKey.Close();                         // Store and close
                        RegKey.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool AddKeyBinary(RegistryHive Type, string Machine, string Key, string Name, byte[] Value)
        {
            try
            {
                string existValue = null;
                byte[] obj = (byte[])GetKeyValue(Type, Machine, Key, Name);
                if (obj != null)
                {
                    existValue = Encoding.Unicode.GetString(obj);
                }
                string newValue = Encoding.Unicode.GetString(Value);

                if (existValue == null || string.Equals(existValue, newValue, StringComparison.CurrentCultureIgnoreCase) != true)
                {
                    RegistryKey RegKey = RegistryKey.OpenRemoteBaseKey(Type, Machine);
                    RegistryKey NewKey = RegKey.CreateSubKey(Key);
                    NewKey.SetValue(Name, Value, RegistryValueKind.Binary);
                    NewKey.Close();                         // Store and close
                    RegKey.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool RemoveKey(RegistryHive Type, string Machine, string Key, string Name)
        {
            try
            {
                string existValue = (string)GetKeyValue(Type, Machine, Key, Name);
                if (existValue != null)
                {
                    RegistryKey RegKey = RegistryKey.OpenRemoteBaseKey(Type, Machine);
                    RegistryKey OpenKey = RegKey.OpenSubKey(Key, true);
                    OpenKey.DeleteValue(Name);
                    RegKey.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static object GetKeyValue(RegistryHive Type, string Machine, string Key, string Name)
        {
            RegistryKey RegKey = null;
            RegistryKey OpenKey = null;
            object RetObj = null;
            try
            {
                RegKey = RegistryKey.OpenRemoteBaseKey(Type, Machine);
                OpenKey = RegKey.OpenSubKey(Key, true);
                if (OpenKey != null)
                {
                    RetObj = OpenKey.GetValue(Name, null);
                }
                return RetObj;
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                if (OpenKey != null)
                {
                    OpenKey.Close();
                }
                if (RegKey != null)
                {
                    RegKey.Close();
                }
            }
        }

        public static string[] GetSubKeyNames(RegistryHive Type, string Key)
        {
            RegistryKey RegKey = null;
            RegistryKey OpenKey = null;
            string[] result = null;
            try
            {
                RegKey = RegistryKey.OpenRemoteBaseKey(Type, "");
                OpenKey = RegKey.OpenSubKey(Key, true);
                result = OpenKey.GetValueNames();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                if (OpenKey != null)
                {
                    OpenKey.Close();
                }
                if (RegKey != null)
                {
                    RegKey.Close();
                }
            }
            return result;
        }

        public static void SaveSetting(RegistryHive hive, string key, string name, string value)
        {
            AddKey(hive, string.Empty, key, name, value);
        }

        public static void SaveSetting(RegistryHive hive, string key, string name, int value)
        {
            AddKey(hive, string.Empty, key, name, value);
        }

        public static void SaveSetting(string key, string name, int value)
        {
            AddKey(RegistryHive.CurrentUser, string.Empty, key, name, value);
        }

        public static void SaveSetting(string key, string name, float value)
        {
            AddKey(RegistryHive.CurrentUser, string.Empty, key, name, value);
        }

        public static void SaveSetting(string key, string name, string value)
        {
            AddKey(RegistryHive.CurrentUser, string.Empty, key, name, value);
        }

        public static void SaveSetting(string key, string name, bool value)
        {
            AddKey(RegistryHive.CurrentUser, string.Empty, key, name, value);
        }

        public static string GetSetting(RegistryHive hive, string key, string name, string defValue)
        {
            string value = "";
            try
            {
                value = (string)GetKeyValue(hive, string.Empty, key, name);
                if (value == null)
                {
                    value = defValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return value;
        }

        public static string[] GetSetting(RegistryHive hive, string key, string name)
        {
            string[] value = null;
            try
            {
                value = GetKeyValue(hive, string.Empty, key, name) as string[];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return value;
        }

        public static string GetSetting(string key, string name, string defValue)
        {
            string value = "";
            try
            {
                value = (string)GetKeyValue(RegistryHive.CurrentUser, string.Empty, key, name);
                if (value == null)
                {
                    value = defValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return value;
        }

        public static int GetSettingInt(RegistryHive hive, string key, string name, int defValue)
        {
            object value = 0;
            try
            {
                value = GetKeyValue(hive, string.Empty, key, name);
                if (value == null)
                {
                    value = defValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (int)value;
        }

        public static float GetSettingFloat(string key, string name, float defValue)
        {
            float result = defValue;
            object value = null;
            try
            {
                value = GetKeyValue(RegistryHive.CurrentUser, string.Empty, key, name);
                if (value != null)
                {
                    float.TryParse(value.ToString(), out result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public static bool GetSettingBool(string key, string name, bool defValue)
        {
            bool result = defValue;
            object value;
            try
            {
                value = GetKeyValue(RegistryHive.CurrentUser, string.Empty, key, name);
                if (value != null)
                {
                    bool.TryParse(value.ToString(), out result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public static object GetSettingObject(string key, string name)
        {
            object value = GetKeyValue(RegistryHive.CurrentUser, string.Empty, key, name);
            return value;
        }

        public static object GetSettingObject(RegistryHive hive, string key, string name)
        {
            object value = GetKeyValue(hive, string.Empty, key, name);
            return value;
        }
    }
}
