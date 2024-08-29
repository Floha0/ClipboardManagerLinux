using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ClipboardManager
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            List<string> copiedTexts = new List<string>();
            bool newCopy = false;
            bool containsAtStart = false;
            
            Console.WriteLine("Clipboard Manager Started!");
            
            
            // Clipboard.Clear();

            if (Clipboard.ContainsText())
            {
                containsAtStart = true;
                copiedTexts.Add(Clipboard.GetText());
            }
            else
            {
                string tempText = "Empty Clipboard!";
                
                copiedTexts.Add(tempText);
            }
            
            
            while (true)
            {
                if (Clipboard.ContainsText())
                {
                    if (!containsAtStart)
                    {
                        newCopy = true;
                    }
                    else
                    {
                        if (Clipboard.GetText() != copiedTexts[copiedTexts.Count - 1])
                        {
                            newCopy = true;
                        }
                    }
                }

                if (newCopy)
                {
                    if (!containsAtStart)
                    {
                        copiedTexts.Clear();
                        containsAtStart = true;
                    }
                    
                    copiedTexts.Add(Clipboard.GetText());

                    Console.Clear();
                    for (int i = 0; i < copiedTexts.Count; i++)
                    {
                        Console.WriteLine((i + 1) + ": " + copiedTexts[i]);
                    }

                    newCopy = false;
                }

                Thread.Sleep(100);
            }
        }
    }
}