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

        public static void Main(string[] args)
        {
            OpcNodeInfo info;

            OpcValue value;

            OpcStatus status;

            object[] result;

            Random random = new Random();

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
                status = client.WriteNode("ns=2;s=MAchine/Parameter", OpcAttribute.DisplayName, "Parameter");
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

                for (int i = 0; i < 5; i++)
                {
                    //Call methodA
                    result = client.CallMethod("ns=2;s=Machine", "ns=2;s=Machine/MethodA", a+i, b);
                    Console.WriteLine($"[opc-ua-client] call methodA = {result[0]}");
                    status = client.WriteNode("ns=2;s=Machine/methodACall", true);
                }

                for (int i = 0; i < 10; i++)
                {
                    ////Call methodB
                    result = client.CallMethod("ns=2;s=Machine", "ns=2;s=Machine/MethodB", a+i, b);
                    Console.WriteLine($"[opc-ua-client] call methodB = {result[0]}");
                    status = client.WriteNode("ns=2;s=Machine/methodBCall", true);
                }

                for (int i = 0; i < 7; i++)
                {
                    ////Call methodC
                    result = client.CallMethod("ns=2;s=Machine", "ns=2;s=Machine/MethodC", a, b+i);
                    Console.WriteLine($"[opc-ua-client] call methodC = {result[0]}");
                    status = client.WriteNode("ns=2;s=Machine/methodCCall", true);
                }

                for (int i = 0; i < 3; i++)
                {
                    //Call methodD
                    result = client.CallMethod("ns=2;s=Machine", "ns=2;s=Machine/MethodD", a, b+i);
                    Console.WriteLine($"[opc-ua-client] call methodD = {result[0]}");
                    status = client.WriteNode("ns=2;s=Machine/methodDCall", true);
                }

                int rnd = random.Next(20, 70);

                for (int i = 0; i < rnd; i++)
                {
                    ////Call methodC
                    result = client.CallMethod("ns=2;s=Machine", "ns=2;s=Machine/MethodC", a, b+i);
                    Console.WriteLine($"[opc-ua-client] call methodC = {result[0]}");
                    status = client.WriteNode("ns=2;s=Machine/methodCCall", true);
                }

                value = client.ReadNode("ns=2;s=Machine/CallsMethodC");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Random number {rnd} --- Calls Method C {value}");
                Console.ForegroundColor = ConsoleColor.White;

                if (rnd == value)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Correct number of methodcalls!");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine();
            }
            Console.WriteLine("[opc-ua-client] Press enter to exit");


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
    }
}