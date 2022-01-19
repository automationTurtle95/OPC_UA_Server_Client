using System;
using System.Collections.Generic;
using System.Linq;

using Opc.UaFx;
using Opc.UaFx.Client;

namespace OPC_UA_Client // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static double a = 10;
        public static double b = 20;
        public static bool loop = true;

        public static void Main(string[] args)
        {
            OpcNodeInfo info;

            OpcValue value;

            OpcStatus status;

            object[] result;

            Random random = new Random();
            Random randomLetter = new Random();

            using (var client = new OpcClient("opc.tcp://localhost:4840"))
            {
                //Connect
                client.Connect();

                //Browse nodes
                info = client.BrowseNode(OpcObjectTypes.ObjectsFolder);
                //Print(info);

                //Read node
                value = client.ReadNode("ns=2;s=Machine/Running");
                Console.WriteLine($"[opc-ua-client] Read node = {value}");

                //Read node attributes
                value = client.ReadNode("ns=2;s=Machine/Running", OpcAttribute.DisplayName);
                Console.WriteLine($"[opc-ua-client] Read node display name = {value}");

                //Write node
                status = client.WriteNode("ns=2;s=Machine/Parameter", 0);
                Console.WriteLine($"[opc-ua-client] Write node = {status}");

                //Write node attribute
                status = client.WriteNode("ns=2;s=Machine/Parameter", OpcAttribute.DisplayName, "Parameter");
                Console.WriteLine($"[opc-ua-client] Write node display name = {status}");

                //read/write node
                /*for (var i = 0; i < 10; i++)
                {
                    value = client.ReadNode("ns=2;s=Machine/Parameter");
                    Console.WriteLine($"[opc-ua-client] Read node = {value}");

                    if (value.Status.IsGood && value.DataType == OpcDataType.Int32)
                    {
                        status = client.WriteNode("ns=2;s=Machine/Parameter", (int)value.Value+1);
                        Console.WriteLine($"[opc-ua-client] Write node = {status}");
                    }

                }*/
                Console.WriteLine();

                while (loop)
                {
                    string a = "";
                    string b = "";
                    double x = 0;
                    double y = 0;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Choose a operation: '+'  '-'  '*'  '/' for exit type 'q'");
                    Console.ForegroundColor = ConsoleColor.White;
                    string operation = Console.ReadLine();

                    if (operation == "q")
                    {
                        break;
                    }

                    Console.WriteLine("First Number:");
                    a = Console.ReadLine();
                    x = checkInput(a);

                    Console.WriteLine("Second Number:");
                    b = Console.ReadLine();
                    y = checkInput(b);

                    if (operation == "+")
                    {
                        //Call methodA
                        callMethod(client, x, y, "A", true);
                    }
                    else if (operation=="-")
                    {
                        ////Call methodB
                        callMethod(client, x, y, "B", true);
                    }
                    else if (operation=="*")
                    {
                        ////Call methodC
                        callMethod(client, x, y, "C", true);
                    }
                    else if (operation=="/")
                    {
                        //Call methodD
                        callMethod(client, x, y, "D", true);
                    }
                    else
                    {
                        redConsole("Please check your input!");
                    }

                }
                Console.WriteLine("Calculations finished!");

                printMethodCalls(client);

                loop = true;

                bool countActive = false;
                while (loop)
                {
                    int nr = random.Next(1, 150);
                    char l = (char)randomLetter.Next('A', 'D');
                    string letter = Convert.ToString(l);
                    double newX = 0;
                    double newY = 0;
                    for (int i = 0; i < nr; i++)
                    {
                        newX = random.Next(10, 500);
                        newY = random.Next(-10, 100);
                        callMethod(client, newX, newY, letter, countActive);
                    }
                    greenConsole($"Random Number {nr} for method {letter}!");
                    Console.WriteLine("Again? y/n");
                    string again = "";
                    again = Console.ReadLine();
                    if (again == "n")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Now with the method counter? y/n");
                        string check = Console.ReadLine();
                        if (check == "y")
                        {
                            countActive = true;
                        }
                        else
                        {
                            countActive = false;
                        }
                    }
                }
                printMethodCalls(client);
            }
            Console.WriteLine("[opc-ua-client] Press enter to exit");


        }

        static void callMethod(OpcClient client, double x, double y, string methodLetter, bool countCalls)
        {
            OpcStatus status = client.WriteNode($"ns=2;s=Machine/method{methodLetter}Call", countCalls);
            if (status.IsGood)
            {
                object[] result = client.CallMethod($"ns=2;s=Machine", $"ns=2;s=Machine/Method{methodLetter}", x, y);
                Console.WriteLine($"Result method{methodLetter} {x}/{y} = {result[0]}");
            }
            else
            {
                redConsole("Check Input");
            }
        }

        static void greenConsole(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void redConsole(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void printMethodCalls(OpcClient client)
        {
            OpcValue value;

            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine("Methodcalls:");
            Console.ForegroundColor = ConsoleColor.White;

            value = client.ReadNode("ns=2;s=Machine/NrMethodACalls");
            Console.WriteLine($"Method A {value} called");

            value = client.ReadNode("ns=2;s=Machine/NrMethodBCalls");
            Console.WriteLine($"Method B {value} called");

            value = client.ReadNode("ns=2;s=Machine/NrMethodCCalls");
            Console.WriteLine($"Method C {value} called");

            value = client.ReadNode("ns=2;s=Machine/NrMethodDCalls");
            Console.WriteLine($"Method D {value} called");
        }


        static void Print(OpcNodeInfo parent, int level = 0)
        {
            var space = new string(' ', level);
            var id = parent.NodeId;
            var name = parent.Attribute(OpcAttribute.DisplayName).Value;

            Console.WriteLine($"[opc-ua-client]{space}{id} {name}");
            foreach (var child in parent.Children())
            {
                Print(child, level+1);
            }
        }

        static double checkInput(string input)
        {
            bool loop = true;
            double number = 0;
            while (loop)
            {
                bool success = double.TryParse(input, out number);
                if (success)
                {
                    loop=false;
                }
                else
                {
                    Console.WriteLine("Enter a number");
                    Console.Write("Check your input!\t");
                    input = Console.ReadLine();
                }
            }

            return number;
        }
    }
}