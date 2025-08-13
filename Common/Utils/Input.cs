using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common.Utils
{
    public static class Input
    {
        public static string GetKeyName(int virtualKey)
        {
            // Convert the virtual key to a Keys enum value
            Keys key = (Keys)virtualKey;

            // Special handling for certain keys
            switch (key)
            {
                case Keys.Space:
                    return "空格";
                case Keys.Back:
                    return "退格";
                case Keys.Return:
                    return "回车";
                case Keys.Escape:
                    return "Esc";
                default:
                    // Get the key name for other keys
                    string keyName = key.ToString();

                    // For function keys and other special keys, return as is
                    if (keyName.StartsWith("F") && keyName.Length <= 3 && int.TryParse(keyName.Substring(1), out _))
                        return keyName; // Function keys (F1, F2, etc.)

                    // For keys like D1, D2, etc., return only the number part
                    if (keyName.StartsWith("D") && keyName.Length == 2 && char.IsDigit(keyName[1]))
                        return keyName.Substring(1);

                    return keyName;
            }
        }
    }
}
