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

        public static void Main(string[] args)
        {
            Application.Init(); // Start the GTK App

            Clipboard clipboard = Clipboard.Get(Gdk.Selection.Clipboard);

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

            // Controlling Clipboard in background continuously
            Thread clipboardMonitorThread = new Thread(() =>
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
                        int index = 1;
                        foreach (var text in copiedTexts)
                        {
                            Console.WriteLine($"{index++}: {text}");
                        }

                        newCopy = false;
                    }

                    Thread.Sleep(100);
                }
            });

            clipboardMonitorThread.IsBackground = true;
            clipboardMonitorThread.Start();

            // Run the GTK App
            Application.Run();
        }
    }
}