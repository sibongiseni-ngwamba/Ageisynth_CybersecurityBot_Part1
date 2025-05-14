using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            InitializeIgnoreWords();
            InitializeResponses();
            InitializeSpecialQuestions();
            InitializeFollowUpResponses();

            // Load memory before displaying welcome
            LoadMemory();

            DisplayWelcomeBanner();
            GetUserName();

            // Start chatbot conversation loop
            RunChatLoop();

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

        // Offer follow-up based on current topic
        private void OfferFollowUp()
        {
            if (string.IsNullOrEmpty(currentTopic))
            {
                return;
            }

            // Dictionary of follow-up questions for each topic
            Dictionary<string, List<string>> followUps = new Dictionary<string, List<string>>
            {
                { "password", new List<string> {
                    "Would you like to know more about password managers?",
                    "Are you using two-factor authentication for your accounts?",
                    "Do you want tips on creating strong passwords that are easy to remember?"
                }},
                { "phishing", new List<string> {
                    "Would you like to know how to identify suspicious emails?",
                    "Do you know what to do if you suspect you've received a phishing attempt?",
                    "Are you familiar with the latest phishing techniques?"
                }},
                { "privacy", new List<string> {
                    "Would you like to review your social media privacy settings?",
                    "Do you know how data brokers collect and sell your information?",
                    "Are you interested in tools that can help protect your online privacy?"
                }},
                { "malware", new List<string> {
                    "Would you like to know the warning signs of malware infection?",
                    "Do you have anti-malware software installed?",
                    "Are you familiar with ransomware and how to protect against it?"
                }},
                { "firewall", new List<string> {
                    "Would you like to know more about configuring your firewall?",
                    "Do you understand the difference between hardware and software firewalls?",
                    "Are you checking your firewall logs regularly?"
                }}
            };

            // If we have follow-ups for the current topic, offer one
            if (followUps.ContainsKey(currentTopic))
            {
                string followUpQuestion = GetRandomResponse(followUps[currentTopic]);
                DisplayBotMessage(followUpQuestion);

                // Store this as our pending follow-up question
                pendingFollowUp = followUpQuestion;
            }
        }//End of method

        // Get a random response from a list
        private string GetRandomResponse(List<string> responses)
        {
            if (responses == null || responses.Count == 0)
            {
                return "I don't have information on that topic yet.";
            }

            Random random = new Random();
            int index = random.Next(responses.Count);
            return responses[index];
        }//End of method

        // Get user's name
        private void GetUserName()
        {
            // Check if we already know the user from memory
            if (userMemory.TryGetValue("name", out string storedName))
            {
                DisplayBotMessage($"Welcome back, {storedName}! Is that still you? (yes/no)");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("You:-> ");
                Console.ForegroundColor = ConsoleColor.White;
                string response = Console.ReadLine().ToLower();

                if (response == "yes" || response == "y")
                {
                    userName = storedName;
                    return;
                }
                // If not the same user, continue to ask for new name
            }

            // Ask for a name (new user or returning user with different name)
            DisplayBotMessage("Please enter your name.");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("You:-> ");
            Console.ForegroundColor = ConsoleColor.White;
            userName = Console.ReadLine();

            // Store user name in memory
            userMemory["name"] = userName;

            // Save memory immediately after getting a new name
            SaveMemory();
        }//End of method

        // Methods for saving and loading memory
        private void SaveMemory()
        {
            try
            {
                // Convert memory dictionary to a list of lines
                List<string> lines = new List<string>();
                foreach (var item in userMemory)
                {
                    lines.Add($"{item.Key}={item.Value}");
                }

                // Write all lines to the memory file
                File.WriteAllLines(MEMORY_FILE, lines);

                // Optional: provide feedback about memory being saved
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[Memory saved]");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch (Exception ex)
            {
                // Handle any errors (file permissions, etc.)
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error saving memory: {ex.Message}]");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }//End of method

        private void LoadMemory()
        {
            try
            {
                // Check if memory file exists
                if (File.Exists(MEMORY_FILE))
                {
                    // Read all lines from the memory file
                    string[] lines = File.ReadAllLines(MEMORY_FILE);

                    // Clear existing memory
                    userMemory.Clear();

                    // Process each line
                    foreach (string line in lines)
                    {
                        // Split the line at the first equals sign
                        int separatorIndex = line.IndexOf('=');
                        if (separatorIndex > 0)
                        {
                            string key = line.Substring(0, separatorIndex);
                            string value = line.Substring(separatorIndex + 1);

                            // Add to memory dictionary
                            userMemory[key] = value;
                        }
                    }

                    // Optional: provide feedback about memory being loaded
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("[Memory loaded]");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            catch (Exception ex)
            {
                // Handle any errors (file corruption, etc.)
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error loading memory: {ex.Message}]");
                Console.ForegroundColor = ConsoleColor.Gray;

                // Initialize a fresh memory
                userMemory.Clear();
            }
        }//End of method

        // Display welcome banner
        private void DisplayWelcomeBanner()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("_==============================================================================================_");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("            |   Welcome to Ageisynth Awareness CyberSecurity Bot   |");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("_==============================================================================================_");
        }//End of method

        // Display bot message with typing effect
        private void DisplayBotMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("AgeisynthBot:-> ");
            Console.ForegroundColor = ConsoleColor.Gray;
            TypeEffect(message);
        }//End of method

        // Display user input prompt
        private void DisplayUserPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{userName}:-> ");
            Console.ForegroundColor = ConsoleColor.White;
        }//End of method

        // Creates a typing animation effect for bot responses
        private void TypeEffect(string message)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(20); // Slightly faster typing speed
            }
            Console.WriteLine(); // Move to the next line after typing
        }//End of method

        // Initialize the list of words to ignore in user input
        private void InitializeIgnoreWords()
        {
            string[] words = {
                "tell", "and", "me", "about", "ensure", "for", "how", "what", "is", "you", "your", "the", "can",
                "do", "would", "will", "should", "could", "are", "am", "was", "were", "be", "been", "being", "it",
                "that", "this", "these", "those", "here", "there", "where", "when", "why", "who", "which", "um",
                "uh", "no", "like", "know", "so", "actually", "okay", "sure", "yeah", "yep", "nope", "nah", "not",
                "very", "quite", "slightly", "a", "an", "in", "on", "at", "by", "of", "to", "with", "my", "i",
                "we", "they", "us", "it's", "that's", "don't", "won't", "can't", "doesn't", "didn't", "some",
                "any", "please", "thanks", "thank", "more", "give", "need", "want", "have", "has", "had", "get"
            };

            foreach (string word in words)
            {
                ignoreWords.Add(word);
            }
        }//End of method

        // Initialize responses for different cybersecurity topics
        private void InitializeResponses()
        {
            // Password responses
            topicResponses["password"] = new List<string>
            {
                "Make sure to use strong, unique passwords for each account. Aim for at least 12 characters with a mix of letters, numbers, and symbols.",
                "Consider using a password manager like LastPass, Bitwarden, or 1Password to generate and store complex passwords.",
                "Enable two-factor authentication whenever possible to add an extra layer of security beyond just your password.",
                "Avoid using personal information in your passwords, such as birthdays, names, or addresses that could be easily guessed.",
                "Change your critical passwords (email, banking) every 3-6 months to minimize the risk of breaches."
            };

            // Phishing responses
            topicResponses["phishing"] = new List<string>
            {
                "Be cautious of emails asking for personal information. Legitimate organizations rarely request sensitive details via email.",
                "Check email sender addresses carefully - phishers often use addresses that look similar to legitimate ones but with small changes.",
                "Hover over links before clicking to see where they actually lead. If in doubt, navigate to the website directly instead of clicking.",
                "Be wary of urgent requests or threats - phishers often create a false sense of urgency to make you act without thinking.",
                "If an email offer seems too good to be true, it probably is. Be skeptical of unexpected prizes or rewards."
            };

            // Privacy responses
            topicResponses["privacy"] = new List<string>
            {
                "Regularly review and update privacy settings on all your social media accounts to control what information is visible to others.",
                "Use private browsing modes and consider a VPN when accessing sensitive information on public networks.",
                "Be cautious about what personal information you share online - once it's out there, it can be difficult to remove.",
                "Regularly check for data breaches involving your accounts using services like Have I Been Pwned.",
                "Consider using privacy-focused alternatives to common services, such as DuckDuckGo instead of Google for searching."
            };

            // Malware responses
            topicResponses["malware"] = new List<string>
            {
                "Keep your operating system and software updated to patch security vulnerabilities that malware can exploit.",
                "Use reputable antivirus and anti-malware software, and ensure it's set to update and scan regularly.",
                "Be cautious when downloading files or clicking on links, especially from unknown or untrusted sources.",
                "Back up your important data regularly to external devices or cloud services that aren't continuously connected to your computer.",
                "Be wary of unexpected pop-ups, system slowdowns, or unusual behavior - these could be signs of malware infection."
            };

            // Firewall responses
            topicResponses["firewall"] = new List<string>
            {
                "Ensure your firewall is enabled at all times to filter network traffic and block unauthorized access attempts.",
                "Regularly review your firewall settings to ensure they're configured properly for your needs.",
                "Consider using both hardware (router) and software (OS) firewalls for multiple layers of protection.",
                "When installing new software, be cautious about allowing it through your firewall - only grant access if necessary.",
                "For higher security, configure your firewall to use a default-deny policy, where only explicitly allowed connections are permitted."
            };

            // Scam responses
            topicResponses["scam"] = new List<string>
            {
                "Be skeptical of unsolicited phone calls, emails, or messages, especially those requesting personal information or payment.",
                "Research unfamiliar companies or offers thoroughly before providing any information or making payments.",
                "Use secure payment methods that offer protection, such as credit cards or PayPal, rather than wire transfers or gift cards.",
                "Trust your instincts - if something feels wrong or too good to be true, it's better to walk away.",
                "Keep up with current scam techniques by checking resources like the FTC's scam alerts website."
            };

            // Attack responses
            topicResponses["attack"] = new List<string>
            {
                "Keep all your devices and software updated to protect against known vulnerabilities.",
                "Use strong authentication methods, including multi-factor authentication, for all important accounts.",
                "Be aware of social engineering tactics - attackers often manipulate people rather than technology.",
                "Segment your network to limit access between different systems and contain potential breaches.",
                "Consider implementing an intrusion detection system to alert you to suspicious activities."
            };

            // SQL injection responses
            topicResponses["sql"] = new List<string>
            {
                "Always validate and sanitize user input to prevent malicious SQL commands from being executed.",
                "Use parameterized queries or prepared statements instead of directly concatenating user input into SQL queries.",
                "Implement the principle of least privilege for database accounts to minimize damage if a breach occurs.",
                "Regularly audit your database and application code for potential SQL injection vulnerabilities.",
                "Consider using an ORM (Object-Relational Mapping) framework which can help prevent SQL injection by design."
            };
        }//End of method

        // Initialize special questions and their responses
        private void InitializeSpecialQuestions()
        {
            specialQuestions["how are you"] = "I'm a cybersecurity chatbot, always ready to help you stay safe online! How can I assist you today?";

            specialQuestions["what is your purpose"] = "My purpose is to provide helpful cybersecurity advice and raise awareness about online safety. I can answer questions about passwords, phishing, privacy, malware, and other security topics.";

            specialQuestions["what can i ask about"] = "You can ask me about various cybersecurity topics including:\n" +
                "- Password safety and management\n" +
                "- Phishing attacks and how to avoid them\n" +
                "- Privacy protection online\n" +
                "- Malware prevention and detection\n" +
                "- Firewall configuration\n" +
                "- Scam awareness\n" +
                "- SQL injection and other technical attacks\n\n" +
                "What topic are you most interested in learning about?";

            specialQuestions["who made you"] = "I was created as part of the Ageisynth Cybersecurity Awareness initiative to help people learn about staying safe online.";

            specialQuestions["help"] = "I can provide information on cybersecurity topics like passwords, phishing, privacy, malware, and more. Just ask me a question like 'Tell me about password safety' or 'How can I protect against phishing?'";

            specialQuestions["how do i exit"] = "To Exit or stop the program type exit to leave.";
        }//End of method


        // Initialize follow-up question responses
        private void InitializeFollowUpResponses()
        {
            // For phishing follow-up responses
            followUpResponses["Do you know what to do if you suspect you've received a phishing attempt?"] = new Dictionary<string, string>
            {
                ["yes"] = "That's great! It's important to be prepared. Just as a reminder, you should: 1) Don't click any links or download attachments, 2) Don't reply with personal information, 3) Report the email to your IT department if at work, or forward to the organization being impersonated, 4) Delete the email, and 5) Consider running a security scan on your device.",
                ["no"] = "If you receive a suspected phishing email: 1) Don't click any links or download attachments, 2) Don't reply with personal information, 3) Report the email to your IT department if at work, or forward to the organization being impersonated via their official contact channels, 4) Delete the email, and 5) Consider running a security scan on your device to be safe."
            };

            // For password manager follow-up
            followUpResponses["Would you like to know more about password managers?"] = new Dictionary<string, string>
            {
                ["yes"] = "Password managers are tools that securely store and manage your passwords. They can generate strong, unique passwords for all your accounts and auto-fill them when needed. Popular options include LastPass, 1Password, Bitwarden, and KeePass. They use strong encryption to protect your data, and you only need to remember one master password. Many also offer secure sharing, breach monitoring, and multi-factor authentication.",
                ["no"] = "No problem! If you ever change your mind, just ask about password managers. Meanwhile, remember to use strong, unique passwords for all your accounts, and consider enabling two-factor authentication when available."
            };

            // For two-factor authentication follow-up
            followUpResponses["Are you using two-factor authentication for your accounts?"] = new Dictionary<string, string>
            {
                ["yes"] = "Excellent! Using two-factor authentication is one of the best ways to protect your accounts. Remember to keep your authentication app or backup codes in a secure place in case you lose your phone or primary authentication device.",
                ["no"] = "I'd strongly recommend setting up two-factor authentication (2FA) for your important accounts. It adds an extra layer of security by requiring something you know (password) and something you have (like your phone). Even if someone steals your password, they can't access your account without the second factor. Most email providers, social media platforms, and financial services offer 2FA options in their security settings."
            };

            // For strong passwords follow-up
            followUpResponses["Do you want tips on creating strong passwords that are easy to remember?"] = new Dictionary<string, string>
            {
                ["yes"] = "Great! Here are some tips for creating strong, memorable passwords: 1) Use a passphrase - a sequence of random words (e.g., 'correct-horse-battery-staple'), 2) Add numbers and special characters between words, 3) Use the first letters of a meaningful sentence (e.g., 'MdGTFaFw25y!' for 'My dog Gizmo turned five and found wisdom 25 years!'), 4) Avoid personal information, and 5) Don't reuse passwords across different accounts.",
                ["no"] = "That's fine! Remember that a password manager can generate and remember strong passwords for you. If you ever need password tips in the future, just ask."
            };

            // For suspicious emails follow-up
            followUpResponses["Would you like to know how to identify suspicious emails?"] = new Dictionary<string, string>
            {
                ["yes"] = "Here are key signs of suspicious emails: 1) Mismatched or strange sender email addresses, 2) Generic greetings like 'Dear User' instead of your name, 3) Poor grammar or spelling errors, 4) A sense of urgency or threats, 5) Requests for personal information, 6) Suspicious attachments or links, and 7) Offers that seem too good to be true. Always hover over links before clicking to see the actual URL destination.",
                ["no"] = "No problem! If you ever need help identifying suspicious emails in the future, just let me know."
            };

            // For latest phishing techniques follow-up
            followUpResponses["Are you familiar with the latest phishing techniques?"] = new Dictionary<string, string>
            {
                ["yes"] = "That's great! Staying informed about the latest threats is important. Remember that phishing techniques are constantly evolving, so it's good to regularly check security news sources to stay up-to-date.",
                ["no"] = "Recent phishing techniques include: 1) Spear phishing - highly targeted attacks using personal information, 2) Clone phishing - duplicating legitimate emails but replacing links with malicious ones, 3) Voice phishing (vishing) - phone calls pretending to be legitimate companies, 4) SMS phishing (smishing) - text messages with malicious links, 5) Business Email Compromise (BEC) - impersonating executives to request wire transfers, and 6) QR code phishing - malicious QR codes leading to fake websites."
            };

            // For social media privacy settings follow-up
            followUpResponses["Would you like to review your social media privacy settings?"] = new Dictionary<string, string>
            {
                ["yes"] = "Great! Here are key privacy settings to check on social media: 1) Profile visibility - limit who can see your profile and posts, 2) Friend/connection permissions - control what connections can see, 3) Post audience settings - choose who sees each post, 4) Tag settings - review tags before they appear on your profile, 5) Search visibility - control whether search engines can find you, and 6) App permissions - review which third-party apps have access to your account.",
                ["no"] = "No problem! If you ever want to review your social media privacy settings in the future, just ask for guidance."
            };

            // For data brokers follow-up
            followUpResponses["Do you know how data brokers collect and sell your information?"] = new Dictionary<string, string>
            {
                ["yes"] = "It's good that you're informed about data brokers. Remember that you can opt out of many data broker services, though it may require contacting each company individually.",
                ["no"] = "Data brokers collect information about you from various sources including public records, online activities, purchase histories, social media, and app usage. They compile this into detailed profiles and sell it to advertisers, marketers, other businesses, and sometimes individuals. To protect yourself, you can: 1) Opt out of data collection when possible, 2) Use privacy-focused browsers and search engines, 3) Regularly check and adjust privacy settings on your accounts, and 4) Consider using services that contact data brokers to remove your information."
            };

            // For privacy tools follow-up
            followUpResponses["Are you interested in tools that can help protect your online privacy?"] = new Dictionary<string, string>
            {
                ["yes"] = "Great! Here are some helpful privacy tools: 1) VPNs (Virtual Private Networks) to encrypt your internet traffic, 2) Privacy-focused browsers like Firefox or Brave, 3) Ad and tracker blockers like uBlock Origin or Privacy Badger, 4) Secure messaging apps like Signal or Wickr, 5) Password managers, 6) Email aliases or forwarding services to hide your real email, and 7) Privacy-focused search engines like DuckDuckGo or Startpage.",
                ["no"] = "That's okay! If you ever become interested in privacy tools in the future, feel free to ask about them."
            };

            // For malware warning signs follow-up
            followUpResponses["Would you like to know the warning signs of malware infection?"] = new Dictionary<string, string>
            {
                ["yes"] = "Here are common signs of malware infection: 1) Unexpected slowdowns or crashes, 2) Pop-up ads even when browsers are closed, 3) Changes to your homepage or browser settings, 4) Unfamiliar programs running or in your app list, 5) Disabled security software, 6) Unusual network activity or data usage, 7) Missing files or storage space, 8) Overheating computer, 9) Friends receiving strange messages from your accounts, and 10) Unexpected system behavior like random restarts.",
                ["no"] = "No problem! If you notice unusual behavior on your device in the future and want to check if it might be malware, just ask."
            };

            // For anti-malware software follow-up
            followUpResponses["Do you have anti-malware software installed?"] = new Dictionary<string, string>
            {
                ["yes"] = "Excellent! Make sure your anti-malware software is set to update automatically and perform regular scans. Remember that no protection is 100% effective, so safe browsing habits are still important.",
                ["no"] = "I'd strongly recommend installing reputable anti-malware software. While Windows Defender (built into Windows) provides basic protection, additional options include Malwarebytes, Bitdefender, and Kaspersky. Look for software that offers real-time protection, automatic updates, scheduled scanning, and web protection features. Many offer free basic versions that provide essential protection."
            };

            // For ransomware follow-up
            followUpResponses["Are you familiar with ransomware and how to protect against it?"] = new Dictionary<string, string>
            {
                ["yes"] = "Great! It's good that you're aware of ransomware threats. Remember that regular backups are your best defense against ransomware attacks.",
                ["no"] = "Ransomware is malware that encrypts your files and demands payment for the decryption key. To protect yourself: 1) Keep regular backups of important files on disconnected storage, 2) Keep your operating system and software updated, 3) Be cautious with email attachments and downloads, 4) Use anti-malware software with ransomware protection, 5) Apply the principle of least privilege for user accounts, and 6) Consider using ransomware-specific protection tools. If infected, disconnect from the internet immediately and seek professional help."
            };

            // For firewall configuration follow-up
            followUpResponses["Would you like to know more about configuring your firewall?"] = new Dictionary<string, string>
            {
                ["yes"] = "Here are firewall configuration tips: 1) Ensure your firewall is enabled on all networks, 2) Create separate rules for public and private networks, 3) Block all incoming connections by default, then add exceptions only as needed, 4) Use application-based rules rather than port-based when possible, 5) Log blocked connection attempts, 6) Regularly review and remove unused rules, and 7) Test your configuration with online firewall testing tools. Most operating systems have built-in firewalls with configuration guides available.",
                ["no"] = "No problem! If you need firewall configuration help in the future, feel free to ask."
            };

            // For firewall types follow-up
            followUpResponses["Do you understand the difference between hardware and software firewalls?"] = new Dictionary<string, string>
            {
                ["yes"] = "Great! Using both hardware and software firewalls provides layered protection, which is an excellent security practice.",
                ["no"] = "Hardware firewalls are physical devices (usually built into routers) that filter traffic before it reaches any device on your network. They protect all connected devices at once. Software firewalls are programs installed on individual devices that filter traffic specific to that device. They can provide more granular control over applications. For best protection, use both: hardware firewalls to protect your entire network and software firewalls for device-specific protection."
            };

            // For firewall logs follow-up
            followUpResponses["Are you checking your firewall logs regularly?"] = new Dictionary<string, string>
            {
                ["yes"] = "Excellent practice! Regular log checking helps you spot unusual patterns that might indicate attempted intrusions.",
                ["no"] = "Checking firewall logs can help identify potential threats and attempted intrusions. Look for: 1) Multiple failed access attempts from the same source, 2) Connection attempts to unusual ports, 3) High traffic volumes at unexpected times, and 4) Connection attempts from suspicious geographical locations. Most operating systems let you access firewall logs through security settings. For routers, check the admin panel. Consider automated monitoring tools if you manage multiple systems."
            };
        }//End of method
    }// End of AgeisynthBot
}//End of Namespace