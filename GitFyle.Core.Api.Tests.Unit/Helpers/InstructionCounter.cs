// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GitFyle.Core.Api.Tests.Unit.Helpers
{
    internal class InstructionCounter
    {
        private readonly HashSet<MethodDefinition> visitedMethods = new HashSet<MethodDefinition>();
        private readonly MethodDefinition methodDefinition = null;

        public InstructionCounter(Delegate methodDelegate)
        {
            this.methodDefinition = FindMethodDefinition(methodDelegate);
        }

        public int CountInstructionsRecursive()
        {
            if (this.methodDefinition == null)
            {
                return 0;
            }

            return CountInstructionsRecursive(this.methodDefinition);
        }

        public int CountInstructionsIncludingTryCatch()
        {
            if (this.methodDefinition == null)
            {
                return 0;
            }

            return CountMethodInstructions(this.methodDefinition) + CountTryCatchInstructions(this.methodDefinition);
        }

        private int CountMethodInstructions(MethodDefinition method)
        {
            return method.Body?.Instructions.Count ?? 0;
        }


        private int CountTryCatchInstructions(MethodDefinition method)
        {
            int lambdaInstructionCount = 0;

            foreach (var instruction in method.Body?.Instructions ?? Enumerable.Empty<Instruction>())
            {
                if (instruction.OpCode == OpCodes.Ldftn || instruction.OpCode == OpCodes.Ldvirtftn)
                {
                    var lambdaMethod = instruction.Operand as MethodReference;
                    if (lambdaMethod != null)
                    {
                        var resolvedLambdaMethod = lambdaMethod.Resolve();
                        if (resolvedLambdaMethod != null)
                        {
                            lambdaInstructionCount += CountMethodInstructions(resolvedLambdaMethod);
                        }
                    }
                }
            }

            return lambdaInstructionCount;
        }



        private int CountInstructionsRecursive(MethodDefinition method)
        {
            if (visitedMethods.Contains(method))
            {
                return 0;
            }

            visitedMethods.Add(method);

            int instructionCount = method.Body?.Instructions.Count ?? 0;

            foreach (var instruction in method.Body?.Instructions ?? Enumerable.Empty<Instruction>())
            {
                if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    var calledMethod = instruction.Operand as MethodReference;
                    if (calledMethod != null)
                    {
                        var resolvedMethod = calledMethod.Resolve();
                        if (resolvedMethod != null)
                        {
                            instructionCount += CountInstructionsRecursive(resolvedMethod);
                        }
                    }
                }

                if (instruction.OpCode == OpCodes.Newobj)
                {
                    var constructorMethod = instruction.Operand as MethodReference;
                    if (constructorMethod != null)
                    {
                        var resolvedConstructor = constructorMethod.Resolve();
                        if (resolvedConstructor != null)
                        {
                            instructionCount += CountInstructionsRecursive(resolvedConstructor);
                        }
                    }
                }

                if (instruction.OpCode == OpCodes.Ldftn || instruction.OpCode == OpCodes.Ldvirtftn)
                {
                    var lambdaMethod = instruction.Operand as MethodReference;
                    if (lambdaMethod != null)
                    {
                        var resolvedLambdaMethod = lambdaMethod.Resolve();
                        if (resolvedLambdaMethod != null)
                        {
                            instructionCount += CountInstructionsRecursive(resolvedLambdaMethod);
                        }
                    }
                }
            }

            if (IsAsyncStateMachine(method))
            {
                var moveNextMethod = GetMoveNextMethod(method);
                if (moveNextMethod != null)
                {
                    instructionCount += CountInstructionsRecursive(moveNextMethod);
                }
            }

            return instructionCount;
        }

        private static MethodDefinition FindMethodDefinition(Delegate methodDelegate)
        {
            MethodInfo methodInfo = methodDelegate.Method;
            string assemblyPath = methodInfo.DeclaringType.Assembly.Location;
            string methodName = methodInfo.DeclaringType.FullName + "::" + methodInfo.Name;

            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);

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

        private bool IsAsyncStateMachine(MethodDefinition method)
        {
            return method.CustomAttributes.Any(attr =>
                attr.AttributeType.Name.Contains("AsyncStateMachineAttribute"));
        }

        private MethodDefinition GetMoveNextMethod(MethodDefinition method)
        {
            var stateMachineType = method.DeclaringType.NestedTypes
                .FirstOrDefault(nt => nt.Interfaces.Any(i => i.InterfaceType.Name.Contains("IAsyncStateMachine")));

            return stateMachineType?.Methods.FirstOrDefault(m => m.Name == "MoveNext");
        }

    }
}
