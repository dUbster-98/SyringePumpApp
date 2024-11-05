using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyringePumpTest1.Services
{
    public interface ITextBoxService
    {
        string TextBoxContext { get; set; }
        string InputString {  get; set; }

        void AddText(string text);
        void ChangeText(string context, string tet);
        void RemoveText();

        public class TextBoxService : ITextBoxService
        {
            public string TextBoxContext { get; set; } = "";
            public string InputString { get; set; } = "";

            public void AddText(string text) 
            {
                TextBoxContext += text;                
            }

            public void ChangeText(string context, string text)
            {
                TextBoxContext = text;
            }

            public void RemoveText()
            {
                InputString = "";                
            }
        }
    }
}
