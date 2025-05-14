using System;
using System.Collections;
using System.Collections.Generic;

namespace Ageisynth_CybersecurityBot_Part1
{
    // Class responsible for chatbot interaction and response handling
    public class ageisynthBot_interface
    {
        // User information storage
        private string userName = string.Empty;
        private Dictionary<string, string> userMemory = new Dictionary<string, string>();

        // File path for memory storage
        private const string MEMORY_FILE = "memory.txt";

        // Track conversation context
        private string currentTopic = string.Empty;
        private List<string> conversationHistory = new List<string>();

        // Track active follow-up questions
        private string pendingFollowUp = string.Empty;
        private Dictionary<string, Dictionary<string, string>> followUpResponses = new Dictionary<string, Dictionary<string, string>>();

        // Sentiment tracking
        private Dictionary<string, bool> detectedSentiments = new Dictionary<string, bool>
        {
            { "worried", false },
            { "confused", false },
            { "frustrated", false },
            { "curious", false },
            { "happy", false }
        };

        // Words to ignore in user input
        private HashSet<string> ignoreWords = new HashSet<string>();

        // Keyword-based response system
        private Dictionary<string, List<string>> topicResponses = new Dictionary<string, List<string>>();

        // Special questions handling
        private Dictionary<string, string> specialQuestions = new Dictionary<string, string>();

        // Default responses for unknown inputs
        private List<string> defaultResponses = new List<string>
        {
            "I didn't quite understand that. Could you rephrase?",
            "I'm not sure I follow. Can you try asking that in a different way?",
            "I'm still learning! Could you phrase that differently?",
            "I'm not familiar with that topic yet. Would you like to know about password safety, phishing, or malware instead?"
        };


        // Constructor - This runs automatically when a new object of ChatbotResponse is created
        public ageisynthBot_interface()
        {
            

        }// End of Constructor

        //Run loop method
        private void RunChatLoop()
        {
            string userInput;
            bool firstInteraction = true;

            do
            {
                // First interaction greeting
                if (firstInteraction)
                {
                    DisplayBotMessage($"Hey {userName}, can I assist you with cybersecurity questions today? You can ask about topics like passwords, phishing, privacy, or malware.");

                    // Add check for previous interests after first greeting
                    if (userMemory.TryGetValue("interest", out string interest))
                    {
                        DisplayBotMessage($"I remember you were interested in {interest}. Would you like to continue learning about that topic?");
                    }

                    firstInteraction = false;
                }

                // Get user input
                DisplayUserPrompt();
                userInput = Console.ReadLine();

                // Store in conversation history
                if (!string.IsNullOrWhiteSpace(userInput))
                {
                    conversationHistory.Add(userInput);
                }

                // Process user input and generate response
                if (userInput?.ToLower() != "exit")
                {
                    ProcessUserInput(userInput);
                }

            } while (userInput?.ToLower() != "exit"); // Keep running until user types "exit"

            // Save memory before exiting
            SaveMemory();

            // Exit message
            DisplayBotMessage($"Thank you for using Ageisynth AI, {userName}! Stay safe online!");
        }

    }// End of AgeisynthBot
}//End of Namespace