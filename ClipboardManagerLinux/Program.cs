using System;
using System.Collections.Generic;
using System.Threading;
using Gtk;

namespace ClipboardManager
{
    internal class Program
    {
        private static Queue<string> copiedTexts = new Queue<string>();
        private static readonly int maxTextCount = 10; // Saklanacak maksimum metin sayısı

        private static bool newCopy = false;
        private static bool containsAtStart = false;
        
        private static Clipboard clipboard;

        public static void Main(string[] args)
        {
            Application.Init(); // Start the GTK App
            
            clipboard = Clipboard.Get(Gdk.Selection.Clipboard);

            // Check the clipboard on startup
            if (!string.IsNullOrEmpty(clipboard.WaitForText()))
            {
                containsAtStart = true;
                copiedTexts.Enqueue(clipboard.WaitForText());
            }
            else
            {
                string tempText = "Empty Clipboard!";
                copiedTexts.Enqueue(tempText);
            }

            Console.WriteLine("Clipboard Manager Started!");

            Console.Clear();
            for (int i = 0; i < copiedTexts.Count; i++)
            {
                Console.WriteLine((i + 1) + ": " + copiedTexts.ElementAt(i));
            }

            // Controlling Clipboard in background continuously
            Thread clipboardMonitorThread = new Thread(ManageClipboard);
            clipboardMonitorThread.IsBackground = true;
            clipboardMonitorThread.Start();
                        
            // Console input monitoring thread
            Thread consoleInputThread = new Thread(HandleConsoleInput);
            consoleInputThread.IsBackground = true;
            consoleInputThread.Start();

            // Run the GTK App
            Application.Run();
        }

        private static void ManageClipboard()
        {
            while (true)
            {
                string currentText = clipboard.WaitForText();

                if (!string.IsNullOrEmpty(currentText))
                {
                    if (!containsAtStart)
                    {
                        newCopy = true;
                    }
                    else
                    {
                        if (currentText != copiedTexts.Last())
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
                        
                    if (copiedTexts.Count >= maxTextCount)
                    {
                        copiedTexts.Dequeue(); // Dequeue the oldest text
                    }
                        
                    copiedTexts.Enqueue(currentText);

                    Console.Clear();
                    for (int i = 0; i < copiedTexts.Count; i++)
                    {
                        Console.WriteLine((i + 1) + ": " + copiedTexts.ElementAt(i));
                    }
                        
                        
                    newCopy = false;
                }

                Thread.Sleep(100);
            }
        }
        
        private static void HandleConsoleInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                int textIndex;
                bool success = int.TryParse(input, out textIndex);

                if (success && textIndex > 0 && textIndex <= copiedTexts.Count)
                {
                    clipboard.Text = copiedTexts.ElementAt(textIndex - 1);
                    Console.WriteLine($"Clipboard updated with: {copiedTexts.ElementAt(textIndex - 1)}");
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid index.");
                }
            }
        }
    }
}