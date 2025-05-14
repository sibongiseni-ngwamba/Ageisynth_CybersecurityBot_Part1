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


    }// End of AgeisynthBot
}//End of Namespace