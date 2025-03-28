using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quizz.Controller
{
    internal class PromptController
    {
        public static string ShowPrompt(string text, string caption)
        {
            using (Prompt prompt = new Prompt(text, caption))
            {
                return (prompt.ShowDialog() == DialogResult.OK) ? prompt.InputText : string.Empty;
            }
        }
    }
}
