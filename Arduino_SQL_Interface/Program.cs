using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_SQL_Interface
{
    public class Program
    {
        static DateTime lastError = DateTime.Now;
        static int errorCounter = 0;
        static int errorCounterDisplay = 0;

        static DatabaseLight myDB = new DatabaseLight("SQLSERVER\\SQLEXPRESS","Database","Jars","Username","Password");
        static Logger myLogger = new Logger(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)+@"\Arduino to SQL Interface Error Log\");
        
        private const int port = 1440; //The port the interface listens on
        static IPAddress ip = IPAddress.Parse("IP"); //The IP address the interface listens for
        //LAB: 192.168.12.110, Testing in GR: 192.168.3.177

        /// <summary>
        /// The main program. It starts the Interface and logs errors in a file, after a set amount of errors the Interface will stop.
        /// </summary>
        static void Main(){
            try{
                Console.Title = "Arduino -> SQL Interface";
                StartInterface(); //Starts the interface method
            }
            catch (Exception ex){
                exceptionHandler(ex); //Handles exceptions and logs them in a .txt file
            }
        }

        /// <summary>
        /// Listen to the ethernet port for a broadcast. If a broadcast is 
        /// received it will be processed and sent to the SQL server.
        /// </summary>
        private static void StartInterface(){ //Runs async with task factory
            bool done = false;

            UdpClient listener = new UdpClient(port);
            IPEndPoint senderIP = new IPEndPoint(ip, port); 

            string RFID = "";
            string weight = "";
            string status = ""; //0 = Discard, 1 = In prod., 2= OK
            string[] dataPackageStrings = new string[3];

            try{
                while (!done){
                    Console.WriteLine("Errors in runtime: " + errorCounterDisplay);
                    Console.WriteLine("Waiting for broadcast from " + ip.ToString() + ":" + port.ToString());
                    byte[] dataPackage = listener.Receive(ref senderIP);

                    Console.WriteLine("Received broadcast from {0}, " + Convert.ToString(DateTime.Now) + " :\n {1}\n",
                        senderIP.ToString(),
                        Encoding.ASCII.GetString(dataPackage, 0, dataPackage.Length));

                    //Splits UDP package into its three components
                    dataPackageStrings = (Encoding.ASCII.GetString(dataPackage, 0, dataPackage.Length)).Split(':');
                    RFID = dataPackageStrings[0];
                    weight = dataPackageStrings[1];
                    status = dataPackageStrings[2];
                    

                    //Removes spaces from data
                    RFID = RFID.Replace(" ", "");
                    weight = weight.Replace(" ", "");
                    Console.WriteLine("Attempting to write:");
                    Console.WriteLine("Status: " + status);
                    Console.WriteLine("RFID: " + RFID);
                    Console.WriteLine("Weight: " + weight);

                    Task task = Task.Factory.StartNew(() => myDB.Write(status,RFID,weight)); //Runs asynchronously by starting a new thread
                    GC.Collect();
                    Console.WriteLine("");
                }
            }
            catch (AggregateException ae) { //Handles async errors
                listener.Close();
                ae.Handle((x) => exceptionHandler(x));
                Console.WriteLine("");
            }
            catch (Exception ex){ //Handles any other errors
                listener.Close();
                exceptionHandler(ex);
                Console.WriteLine("");
            }
        }
        /// <summary>
        /// Handles exceptions
        /// </summary>
        /// <param name="exception">The exception that is to be handled.
        /// </param>
        private static bool exceptionHandler(Exception exception){
            myLogger.log(exception.ToString()); //Logs error
            errorCounterDisplay++;
            Console.WriteLine("The Interface has encountered an error:\n");
            Console.WriteLine(exception.ToString());

            if ((DateTime.Now - lastError).ToString("mm") == "00"){ //Checks if the new error comes within a minute of the previous one
                errorCounter = errorCounter + 1;
                lastError = DateTime.Now;
            }
            else{
                errorCounter = 0; //Resets counter if not within a minute
                lastError = DateTime.Now;
            }
            if (errorCounter == 10){ //Flashes and beeps if 10 errors occured withing a minute of each other
                string s = "Too many errors have occured! Check the log for error messages, and try again.";

                while (true){
                    Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.WindowHeight / 2);
                    Console.WriteLine(s);
                    Console.Beep(650,1000);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Clear();

                    Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.WindowHeight / 2);
                    Console.WriteLine(s);
                    Console.Beep(650, 1000);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Clear();
                }
            }
            else{
                Main(); //Restarts the interface
                return true;
            }
            
        }
    }
}
