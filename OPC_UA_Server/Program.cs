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

            if (a.GetType() != typeof(double) || b.GetType() != typeof(double))
            {
                throw new OpcException();
            }
            if (methodACall.Value == true)
            {
                nrMethodACalls.Value++;
                Console.WriteLine($"Method A {nrMethodACalls.Value} Calls");
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
            if (methodBCall.Value == true)
            {
                nrMethodBCalls.Value++;
                Console.WriteLine($"Method B {nrMethodBCalls.Value} Calls");
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
            if (methodCCall.Value == true)
            {
                nrMethodCCalls.Value++;
                Console.WriteLine($"Method C {nrMethodCCalls.Value} Calls");
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
            if (methodDCall.Value == true)
            {
                nrMethodDCalls.Value++;
                Console.WriteLine($"Method D {nrMethodDCalls.Value} Calls");
            }
            return a/b;
        }

        public static void Main(string[] args)
        {

            machine = new OpcFolderNode(new OpcName("Machine"));

            running = new OpcDataVariableNode<bool>("Running", true); //Variablenknoten hinzufügen
            version = new OpcDataVariableNode<int>("Version", 1);
            random = new OpcDataVariableNode<int>("Random", 0);

            nrMethodACalls = new OpcDataVariableNode<int>(machine, "NrMethodACalls", 0);
            nrMethodACalls.WriteVariableValueCallback = WriteParameter;

            nrMethodBCalls = new OpcDataVariableNode<int>(machine, "NrMethodBCalls", 0);
            nrMethodBCalls.WriteVariableValueCallback = WriteParameter;

            nrMethodCCalls = new OpcDataVariableNode<int>(machine, "NrMethodCCalls", 0);
            nrMethodCCalls.WriteVariableValueCallback = WriteParameter;

            nrMethodDCalls = new OpcDataVariableNode<int>(machine, "NrMethodDCalls", 0);
            nrMethodDCalls.WriteVariableValueCallback = WriteParameter;

            List<OpcDataVariableNode<int>> Calls = new List<OpcDataVariableNode<int>>();
            Calls.Add(nrMethodACalls);
            Calls.Add(nrMethodBCalls);
            Calls.Add(nrMethodCCalls);
            Calls.Add(nrMethodDCalls);

            parameter = new OpcDataVariableNode<int>(machine, "Parameter", 0);  //variablenknoten hinzufügen mit Schreibfunktion
            parameter.WriteVariableValueCallback = WriteParameter;

            methodACall = new OpcDataVariableNode<bool>(machine, "methodACall", false);

            methodBCall = new OpcDataVariableNode<bool>(machine, "methodBCall", false);

            methodCCall = new OpcDataVariableNode<bool>(machine, "methodCCall", false);

            methodDCall = new OpcDataVariableNode<bool>(machine, "methodDCall", false);

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


