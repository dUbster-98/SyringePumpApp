using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyringePumpTest1.Services
{
    public interface ITextBoxService
    {
        string TextBoxContents { get; set; }
        string InputString {  get; set; }

        void AddText(string context, string text);
        void ChangeText(ref string context, string tet);
        void RemoveText(ref string context);

        public class TextBoxService : ITextBoxService
        {
            public string TextBoxContents { get; set; } = "";
            public string InputString { get; set; } = "";

            public void AddText(string context, string text) 
            {
                context += text; 
            }

            public void ChangeText(ref string context, string text)
            {
                context = text;
            }

            public void RemoveText(ref string context)
            {
                
            }
        }
    }
}
