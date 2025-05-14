using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        }//End of chat loop

        // Method that processes user input and determines appropriate response
        public void ProcessUserInput(string userInput)
        {
            // If input is empty or whitespace
            if (string.IsNullOrWhiteSpace(userInput))
            {
                DisplayBotMessage(GetRandomResponse(defaultResponses));
                return;
            }

            // First check if we're waiting for a follow-up response
            if (!string.IsNullOrEmpty(pendingFollowUp))
            {
                if (HandleFollowUpResponse(userInput))
                {
                    return;
                }
            }

            // Detect and store user sentiment
            DetectSentiment(userInput);

            // Check for memory-related queries
            if (HandleMemoryQuery(userInput))
            {
                return;
            }

            // Check for interest declarations and store them
            if (HandleInterestDeclaration(userInput))
            {
                // Save memory when a new interest is registered
                SaveMemory();
                return;
            }

            // Check for special questions first
            string specialResponse = HandleSpecialQuestions(userInput);
            if (specialResponse != null)
            {
                DisplayBotMessage(specialResponse);
                return;
            }

            // Apply keyword matching for cybersecurity topics
            string responseMessage = GenerateKeywordResponse(userInput);

            if (!string.IsNullOrEmpty(responseMessage))
            {
                // Add sentiment-based response modifications
                responseMessage = ApplySentimentContext(responseMessage);

                DisplayBotMessage(responseMessage);

                // Check if the conversation should continue with a follow-up
                OfferFollowUp();
            }
            else
            {
                // If no keywords matched, use default response
                DisplayBotMessage(GetRandomResponse(defaultResponses));
            }
        }//End of process user input

        // Handle simple responses to follow-up questions
        private bool HandleFollowUpResponse(string userInput)
        {
            string lowercaseInput = userInput.ToLower().Trim();

            // Check for yes/no type responses to our pending follow-up
            if (lowercaseInput == "yes" || lowercaseInput == "yeah" || lowercaseInput == "yep" ||
                lowercaseInput == "sure" || lowercaseInput == "ok" || lowercaseInput == "okay")
            {
                // If user says yes, provide the positive response
                if (followUpResponses.ContainsKey(pendingFollowUp) &&
                    followUpResponses[pendingFollowUp].ContainsKey("yes"))
                {
                    DisplayBotMessage(followUpResponses[pendingFollowUp]["yes"]);
                    pendingFollowUp = string.Empty;
                    return true;
                }
            }
            else if (lowercaseInput == "no" || lowercaseInput == "nope" || lowercaseInput == "nah")
            {
                // If user says no, provide the negative response
                if (followUpResponses.ContainsKey(pendingFollowUp) &&
                    followUpResponses[pendingFollowUp].ContainsKey("no"))
                {
                    DisplayBotMessage(followUpResponses[pendingFollowUp]["no"]);
                    pendingFollowUp = string.Empty;
                    return true;
                }
            }

            // If we got here, we didn't handle the follow-up
            pendingFollowUp = string.Empty;
            return false;
        }//End of method

        // Detect user sentiment from input
        private void DetectSentiment(string userInput)
        {
            string lowercaseInput = userInput.ToLower();

            // Reset previously detected sentiments
            foreach (string sentiment in detectedSentiments.Keys.ToList())
            {
                detectedSentiments[sentiment] = false;
            }

            // Check for sentiment keywords
            if (lowercaseInput.Contains("worried") || lowercaseInput.Contains("afraid") ||
                lowercaseInput.Contains("scared") || lowercaseInput.Contains("fear"))
            {
                detectedSentiments["worried"] = true;
            }

            if (lowercaseInput.Contains("confused") || lowercaseInput.Contains("don't understand") ||
                lowercaseInput.Contains("unclear") || lowercaseInput.Contains("complicated"))
            {
                detectedSentiments["confused"] = true;
            }

            if (lowercaseInput.Contains("frustrated") || lowercaseInput.Contains("annoyed") ||
                lowercaseInput.Contains("upset") || lowercaseInput.Contains("angry"))
            {
                detectedSentiments["frustrated"] = true;
            }

            if (lowercaseInput.Contains("curious") || lowercaseInput.Contains("interested") ||
                lowercaseInput.Contains("want to know") || lowercaseInput.Contains("tell me more"))
            {
                detectedSentiments["curious"] = true;
            }

            if (lowercaseInput.Contains("happy") || lowercaseInput.Contains("glad") ||
                lowercaseInput.Contains("great") || lowercaseInput.Contains("excellent"))
            {
                detectedSentiments["happy"] = true;
            }
        }//End of method

        // Apply sentiment context to responses
        private string ApplySentimentContext(string baseResponse)
        {
            string modifiedResponse = baseResponse;

            if (detectedSentiments["worried"])
            {
                modifiedResponse = "I understand you might be worried about  " + baseResponse +
                    " Remember that taking small steps to improve your security can make a big difference.";
            }
            else if (detectedSentiments["confused"])
            {
                modifiedResponse = "This topic can be confusing, so let me explain it simply. " + baseResponse +
                    " Would you like me to clarify anything specific?";
            }
            else if (detectedSentiments["frustrated"])
            {
                modifiedResponse = "I can see this might be frustrating. " + baseResponse +
                    " Let's take this one step at a time to make it more manageable.";
            }
            else if (detectedSentiments["curious"])
            {
                modifiedResponse = "I'm glad you're curious about this! " + baseResponse +
                    " Learning about cybersecurity is an important step toward staying safe online.";
            }
            else if (detectedSentiments["happy"])
            {
                modifiedResponse = "Great! I'm happy to share this information with you. " + baseResponse +
                    " It's always good to see someone enthusiastic about cybersecurity!";
            }

            return modifiedResponse;
        }//End of method

        // Handle user declaring interest in topics
        private bool HandleInterestDeclaration(string userInput)
        {
            string lowercaseInput = userInput.ToLower();

            // Check for interest declarations
            if (lowercaseInput.Contains("interested in") || lowercaseInput.Contains("curious about"))
            {
                foreach (string topic in topicResponses.Keys)
                {
                    if (lowercaseInput.Contains(topic))
                    {
                        // Store the user's interest
                        userMemory["interest"] = topic;

                        DisplayBotMessage($"Great! I'll remember that you're interested in {topic}. It's an important aspect of cybersecurity!");

                        // Provide a relevant response about their interest
                        string topicResponse = GetRandomResponse(topicResponses[topic]);
                        DisplayBotMessage($"Here's something about {topic}: {topicResponse}");

                        return true;
                    }
                }
            }

            return false;
        }//End of method

        // Handle queries about previously stored memory
        private bool HandleMemoryQuery(string userInput)
        {
            string lowercaseInput = userInput.ToLower();

            // If user asks what they were interested in before
            if (lowercaseInput.Contains("what was i interested in") && userMemory.ContainsKey("interest"))
            {
                DisplayBotMessage($"You previously mentioned an interest in {userMemory["interest"]}. Would you like to know more about it?");
                return true;
            }

            // If we have stored interest and user mentions it in a new question
            if (userMemory.ContainsKey("interest") && lowercaseInput.Contains(userMemory["interest"]))
            {
                string interest = userMemory["interest"];

                // Check if we have responses for this topic
                if (topicResponses.ContainsKey(interest))
                {
                    string response = GetRandomResponse(topicResponses[interest]);
                    DisplayBotMessage($"As someone interested in {interest}, you might find this helpful: {response}");
                    return true;
                }
            }

            // Added: If user wants to see their stored information
            if (lowercaseInput.Contains("what do you know about me") ||
                lowercaseInput.Contains("what do you remember") ||
                lowercaseInput.Contains("my information"))
            {
                if (userMemory.Count > 0)
                {
                    DisplayBotMessage($"Here's what I remember about you, {userName}:");
                    foreach (var item in userMemory)
                    {
                        if (item.Key != "name") // Skip name since we already addressed them by name
                        {
                            DisplayBotMessage($"- Your {item.Key}: {item.Value}");
                        }
                    }
                    return true;
                }
                else
                {
                    DisplayBotMessage($"I only know your name is {userName}. You can tell me about your interests in cybersecurity topics, and I'll remember them for next time!");
                    return true;
                }
            }

            return false;
        }//End of method

        // Check for special questions
        private string HandleSpecialQuestions(string userInput)
        {
            string lowercaseInput = userInput.ToLower();

            foreach (var question in specialQuestions)
            {
                if (lowercaseInput.Contains(question.Key))
                {
                    return question.Value;
                }
            }

            return null;
        }//End of method

        // Filter and match keywords to generate responses
        private string GenerateKeywordResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return GetRandomResponse(defaultResponses);
            }

            // Break input into lowercase words and filter out ignored words
            string[] inputWords = userInput.ToLower().Split(new[] { ' ', ',', '.', '?', '!' },
                StringSplitOptions.RemoveEmptyEntries);

            List<string> relevantWords = inputWords.Where(word => !ignoreWords.Contains(word)).ToList();

            // Identify topics mentioned in the input
            List<string> detectedTopics = new List<string>();

            foreach (string word in relevantWords)
            {
                foreach (string topic in topicResponses.Keys)
                {
                    // Check if the word matches a topic keyword
                    if (topic.Contains(word) || word.Contains(topic))
                    {
                        detectedTopics.Add(topic);
                        // Update current topic for context
                        currentTopic = topic;
                    }
                }
            }

            // If continuing a conversation on the same topic
            if (detectedTopics.Count == 0 && !string.IsNullOrEmpty(currentTopic) &&
                (userInput.ToLower().Contains("more") || userInput.ToLower().Contains("tell me") ||
                 userInput.ToLower().Contains("what about")))
            {
                detectedTopics.Add(currentTopic);
            }

            // If no topics detected, return empty
            if (detectedTopics.Count == 0)
            {
                return string.Empty;
            }

            // Generate responses for each detected topic
            string combinedResponse = string.Empty;

            foreach (string topic in detectedTopics.Distinct())
            {
                if (topicResponses.ContainsKey(topic))
                {
                    // Get a random response for this topic
                    string topicResponse = GetRandomResponse(topicResponses[topic]);
                    combinedResponse += $"{topic}: {topicResponse}\n\n";
                }
            }

            return combinedResponse.Trim();
        }//End of method

    }// End of AgeisynthBot
}//End of Namespace