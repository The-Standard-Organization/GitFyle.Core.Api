using System;
using Mono.Cecil;

namespace POCs.ScanCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MethodDefinition def =
                FindMethod(
                    assembly: AssemblyDefinition.ReadAssembly(fileName: "POCs.ScanCode.dll"),
                    methodName: "POCs.ScanCode.Program::Foo2()");

            PrintInstructions(def);
        }

        static MethodDefinition FindMethod(AssemblyDefinition assembly, string methodName)
        {
            foreach (var module in assembly.Modules)
            {
                foreach (var type in module.Types)
                {
                    foreach (var method in type.Methods)
                    {
                        if (method.FullName.Contains(methodName))
                        {
                            return method;
                        }
                    }
                }
            }

            return null;
        }

        static void PrintInstructions(MethodDefinition method)
        {
            if (method.HasBody)
            {
                foreach (var instruction in method.Body.Instructions)
                {
                    Console.WriteLine(instruction);
                }
            }
            else
            {
                Console.WriteLine("The method does not have a body.");
            }
        }

        void Foo()
        {
            Console.WriteLine("Hi");
        }

        void Foo2()
        {
            string x = "John";
            Console.WriteLine(x);
        }
    }
}
