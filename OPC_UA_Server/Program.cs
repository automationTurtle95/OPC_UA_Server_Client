using System;
using System.Collections.Generic;
using System.Linq;
using Opc.UaFx;
using Opc.UaFx.Server;


namespace OPC_UA_Server // Note: actual namespace depends on the project name.
{
    public class Program
    {
        private static Random rnd = new Random();

        private static OpcFolderNode machine;
        private static OpcDataVariableNode<bool> running;
        private static OpcDataVariableNode<int> version;
        private static OpcDataVariableNode<int> random;
        private static OpcDataVariableNode<int> parameter;

        private static OpcDataVariableNode<bool> methodACall;
        private static OpcDataVariableNode<bool> methodBCall;
        private static OpcDataVariableNode<bool> methodCCall;
        private static OpcDataVariableNode<bool> methodDCall;

        private static OpcDataVariableNode<int> nrMethodACalls;
        private static OpcDataVariableNode<int> nrMethodBCalls;
        private static OpcDataVariableNode<int> nrMethodCCalls;
        private static OpcDataVariableNode<int> nrMethodDCalls;

        private static OpcMethodNode methodA;
        private static OpcMethodNode methodB;
        private static OpcMethodNode methodC;
        private static OpcMethodNode methodD;

        private static OpcVariableValue<object> WriteParameter(OpcContext context, OpcVariableValue<object> value)
        {
            //Console.WriteLine($"New value {value.Value}");
            return value;
        }

        private static double MethodA(double a, double b)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Called method A: add to numbers");
            Console.ForegroundColor = ConsoleColor.White;

            if(a.GetType() != typeof(double) || b.GetType() != typeof(double))
            {
                throw new OpcException();
            }

            return a+b;
        }

        private static double MethodB(double a, double b)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Called method B: subtract to numbers");
            Console.ForegroundColor = ConsoleColor.White;

            if (a.GetType() != typeof(double) || b.GetType() != typeof(double))
            {
                throw new OpcException();
            }
            return a-b;
        }

        private static double MethodC(double a, double b)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Called method C: multiply to numbers");
            Console.ForegroundColor = ConsoleColor.White;
            if (a.GetType() != typeof(double) || b.GetType() != typeof(double))
            {
                throw new OpcException();
            }
            return a*b;
        }

        private static double MethodD(double a, double b)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Called method D: divide to numbers");
            Console.ForegroundColor = ConsoleColor.White;
            if (a.GetType() != typeof(double) || b.GetType() != typeof(double))
            {
                throw new OpcException();
            }
            return a/b;
        }

        public static void Main(string[] args)
        {

            machine = new OpcFolderNode(new OpcName("Machine"));

            running = new OpcDataVariableNode<bool>("Running", true); //Variablenknoten hinzufügen
            version = new OpcDataVariableNode<int>("Version", 1);
            random = new OpcDataVariableNode<int>("Random", 0);

            nrMethodACalls = new OpcDataVariableNode<int>("CallsMethodA", 1);
            nrMethodBCalls = new OpcDataVariableNode<int>("CallsMethodB", 1);
            nrMethodCCalls = new OpcDataVariableNode<int>("CallsMethodC", 1);
            nrMethodDCalls = new OpcDataVariableNode<int>("CallsMethodD", 1);

            List<OpcDataVariableNode<int>> Calls = new List<OpcDataVariableNode<int>>();
            Calls.Add(nrMethodACalls);
            Calls.Add(nrMethodBCalls);
            Calls.Add(nrMethodCCalls);
            Calls.Add(nrMethodDCalls);

            parameter = new OpcDataVariableNode<int>(machine, "Parameter", 0);  //variablenknoten hinzufügen mit Schreibfunktion
            parameter.WriteVariableValueCallback = WriteParameter;

            methodACall = new OpcDataVariableNode<bool>(machine, "methodACall", false);
            methodACall.WriteVariableValueCallback = WriteParameter;

            methodBCall = new OpcDataVariableNode<bool>(machine, "methodBCall", false);
            methodBCall.WriteVariableValueCallback = WriteParameter;

            methodCCall = new OpcDataVariableNode<bool>(machine, "methodCCall", false);
            methodCCall.WriteVariableValueCallback = WriteParameter;

            methodDCall = new OpcDataVariableNode<bool>(machine, "methodDCall", false);
            methodDCall.WriteVariableValueCallback = WriteParameter;

            List<OpcDataVariableNode<bool>> Called = new List<OpcDataVariableNode<bool>>();
            Called.Add(methodACall);
            Called.Add(methodBCall);
            Called.Add(methodCCall);
            Called.Add(methodDCall);

            //####### Methodenknoten anlegen
            methodA = new OpcMethodNode(machine, new OpcName("MethodA"), new Func<double, double, double>(MethodA));

            methodB = new OpcMethodNode(machine, new OpcName("MethodB"), new Func<double, double, double>(MethodB));
            methodC = new OpcMethodNode(machine, new OpcName("MethodC"), new Func<double, double, double>(MethodC));
            methodD = new OpcMethodNode(machine, new OpcName("MethodD"), new Func<double, double, double>(MethodD));

            using (var server = new OpcServer("opc.tcp://localhost:4840/", machine))
            {
                server.Start();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Server started!");
                Console.ForegroundColor = ConsoleColor.White;

                while (true)
                {
                    for (int i = 0; i < Called.Count; i++)
                    {
                        if (Called[i].Value == true)
                        {
                            Calls[i].Value++;
                            Called[i].Value = false;
                            Console.WriteLine($"Calls Method: {Calls[i].Value}");
                        }
                    }

                    foreach (var item in Calls)
                    {
                        item.Status.Update(OpcStatusCode.Good);
                        item.Timestamp = DateTime.UtcNow;
                        item.Value=item.Value;
                        item.ApplyChanges(server.SystemContext);
                    }

                    //random.Status.Update(OpcStatusCode.Good);
                    //random.Timestamp = DateTime.UtcNow;
                    //random.Value = rnd.Next();
                    //random.ApplyChanges(server.SystemContext);
                    //Console.WriteLine("Alive");
                    //Thread.Sleep(1000);
                }

            }

        }


    }
}


