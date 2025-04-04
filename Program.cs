using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ageisynth_CybersecurityBot_Part1
{
    // Main Program class that serves as the entry point of the application
    public class Program
    {
        // Main method - execution starts here
        static void Main(string[] args)
        {
            // Create a new instance of the voice_greeting class
            // This will automatically play the welcome greeting audio
            new voice_greeting() { };
        }
    }
}
