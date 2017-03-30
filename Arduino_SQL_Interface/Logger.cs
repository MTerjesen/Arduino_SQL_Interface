using System;
using System.IO;
using System.Text;

namespace Arduino_SQL_Interface{
    class Logger{
        string filePath = "";
        string fileName = "log";
        StringBuilder SB = new StringBuilder();

        /// <summary>
        /// The Constructor. Only the  file path is required.
        /// </summary>
        /// <param name="logPath">The path to the log file, it will create folders.</param>
        public Logger(string logPath){ //Constructor, sets up the path to the log file
            this.filePath = logPath;
        }

        /// <summary>
        /// Logs the inputted message.
        /// </summary>
        /// <param name="logMessage">The text that is to be logged.</param>
        public void log(string logMessage){
            (new FileInfo(filePath)).Directory.Create(); //Creates directory if there is none
            using (StreamWriter SW = File.AppendText(filePath + fileName + DateTime.Now.ToString("ddMMyy") + ".txt")){ //Writes the error message to the log file.
                SW.Write("Log Entry : ");
                SW.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                SW.WriteLine("   {0}", logMessage);
                SW.WriteLine("-------------------------------\n");
            }
        }

        /// <summary>
        /// Write a single line of text to the log file.
        /// </summary>
        /// <param name="text">The line of text that is to be written.</param>
        public void writeText(string text){ 
            (new FileInfo(filePath)).Directory.Create(); //Creates directory if there is none
            using (StreamWriter SW = File.AppendText(filePath + fileName + DateTime.Now.ToString("ddMMyy") + ".txt")){ //Writes a simple line of text to the log file
                SW.WriteLine(text);
            }
        }

        /// <summary>
        /// Returns the log as a string.
        /// </summary>
        /// <returns></returns>
        public string showLog(){
            using (StreamReader SR = File.OpenText(filePath + fileName + DateTime.Now.ToString("ddMMyy") + ".txt")){ //Write the whole log file as a string that can be written to the interface
                string line;
                while ((line = SR.ReadLine()) != null){
                    SB.AppendLine(line);
                }
            }
            return SB.ToString();
        }
    }
}
