using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummersizeText
{
    class Program
    {
        static void Main(string[] args)
        {
            var textInFile = "Jungle Book sample.txt";
            var textOutFile = "summ.txt";
            var stopwordsInFile = "stopwords.txt";
            var sf = 50f;
            Console.Write("filename " );
            var temp = Console.ReadLine();
            if (temp != "")
                textInFile = temp;
            Console.Write("sf ");
            temp = Console.ReadLine();
            if (temp != "")
                sf = float.Parse(temp);

            var textIn = File.ReadAllText(textInFile);
            var stopwordsIn = File.ReadAllText(stopwordsInFile);

            // Replace the end of line markers
            textIn = textIn.Replace("\r\n", " ");

            // Split the text into sentences by finding and removing the special character
            var textEndmarked = textIn.Replace(". ", ".$");  // Mark the end of sentences with a special character
            //string[] sentenceArray = textIn.Split('$'); // Note: These may need trimming
            List<string> sentenceList = textEndmarked.Split('$').ToList(); // Note: These may need trimming
            List<string> sentenceList2 = new List<string>();
            sentenceList2.AddRange(sentenceList.ToList());
            // How many times does "Mowgli" appear in the text?
            //Console.WriteLine(String.Format("Mowgli appears {0} times.", GetOccurrences1(textIn, "a")));
            //Console.WriteLine(String.Format("Mowgli appears {0} times.", GetOccurrences1B(textIn, "a")));
            //Console.WriteLine(String.Format("Mowgli appears {0} times.", GetOccurrences2(textIn, "a")));

            // Write the text to a different file as one line to a sentence
            var textOut = "";
            foreach (var s in sentenceList)
            {
                textOut += s.Trim() // Remove pesky leading spaces.
                        + "\r\n";   // Restore the end of line markers
            }

            var stwords = stopwordsIn.Split("\r\n ".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).Distinct();

            var words = textIn.Split(" ()\'\"<>?/.,:;[]}{|+_\\=-()~!`@*#&$^%".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x=>x.ToLower());
            var wordsDist = words.Distinct();

            List<KeyValuePair<string, int>> countList = new List<KeyValuePair<string, int>>();
            foreach (string word in wordsDist)
            {
                if (stwords.Contains(word)) continue;
                int count = words.Count(x => x == word);
                countList.Add(new KeyValuePair<string, int>(word, count));
            }
            countList.Sort(
                delegate(KeyValuePair<string, int> x,
                KeyValuePair<string, int> y)
                {
                    return y.Value.CompareTo(x.Value);
                }
            );

            Console.Write(string.Join("\n", countList));
            ///////////////////////////////////////////////////////////////////////////////////////
            List<string> outSentenceList = new List<string>();
            List<string> outMaxCount = new List<string>();
            List<string> outKeywords = new List<string>();
            
            int summCount = 0;
            float msf = 0f;
            int incount = sentenceList.Count;
            foreach (KeyValuePair<string, int> word in countList)
            {
                int max = 0;
                string mSentence="";
                foreach(string sentence in sentenceList){
                    int count = sentence.Split(" ()\'\"<>?/.,:;[]}{|+_\\=-()~!`@*#&$^%".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).Count(x=>x==word.Key);
                    if(count>max)
                    {
                        max = count;
                        mSentence = sentence;
                    }
                }

                if (max > 0)
                {
                    summCount = summCount + 1;
                    //summCount = summCount + word.Value;
                    //if (100f * summCount / countList.Count > sf)
                    //    break;
                    //msf = 100f * summCount / countList.Count;

                    sentenceList.Remove(mSentence);

                    outKeywords.Add(word.Key);
                    outSentenceList.Add(mSentence);
                    outMaxCount.Add(max.ToString());
                }
               
                msf = 100f * summCount / incount;
                if (msf >= sf)
                    break;
                

            }

            IEnumerator EoutSentenceList = outSentenceList.GetEnumerator();
            IEnumerator EoutMaxCount = outMaxCount.GetEnumerator();
            List<string> saveStr = new List<string>();
            Console.WriteLine();
            foreach (string keyword in outKeywords)
            {
                EoutSentenceList.MoveNext();
                EoutMaxCount.MoveNext();
                String str = EoutMaxCount.Current + " : " + keyword.PadRight(10) + " : " + sentenceList2.IndexOf(EoutSentenceList.Current.ToString())+" : " + EoutSentenceList.Current.ToString().Trim().Substring(0, 50) + "...";
                saveStr.Add(str);
                Console.WriteLine(str);
            }
            Console.WriteLine("Summarization factor : {0}", msf.ToString("#.##"));

            File.WriteAllText(textOutFile, string.Join("\n",saveStr));
            Console.WriteLine("output file: " + textOutFile);
            // Read a list of lines from a file and write it back out.

            // Wait so window stays open
            Console.ReadLine();
        }

        public static int GetOccurrences1(string text, string word)
        {
            int occurrences = 0;
            int startingIndex = 0;

            while ((startingIndex = text.IndexOf(word, startingIndex)) >= 0)
            {
                occurrences++;
                startingIndex++;
            }

            return occurrences;
        }

        public static int GetOccurrences1B(string text, string word)
        {
            int occurrences = 0;
            int startingIndex = 0;

            while (true)
            {
                startingIndex = text.IndexOf(word, startingIndex);
                if (startingIndex < 0)
                    return occurrences;

                occurrences++;
                startingIndex++;
            }
        }

        public static int GetOccurrences2(string text, string word)
        {
            // Split the text into words.
            char[] delimiterChars = { 
                ' ', '"', '\'', ',', '.', ':', ';', '!', '?', '(', ')', '[', ']' };
            List<string> wordList = text.Split(delimiterChars).ToList();

            var queryCount = wordList.Where(r => r == word).Count();

            return queryCount;
        }
    }
}
