// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace GitFyle.Core.Api.Tests.Unit.Helpers
{
    internal class AsyncMethodAnalyzer
    {
        public static int AnalyzeAsyncMethod(Delegate methodDelegate)
        {
            MethodInfo methodInfo = methodDelegate.Method;
            string assemblyPath = methodInfo.DeclaringType.Assembly.Location;
            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            var typeName = methodInfo.DeclaringType.FullName;
            var type = assembly.MainModule.GetType(typeName);
            var method = type.Methods.FirstOrDefault(m => m.Name == methodInfo.Name);

            var stateMachineType = method.DeclaringType.NestedTypes
                .FirstOrDefault(t => t.Name.Contains($"<{method.Name}>d__"));


            MethodDefinition moveNextMethod = stateMachineType switch
            {
                null => method,
                _ => stateMachineType.Methods.FirstOrDefault(m => m.Name == "MoveNext")
            };

            return CalculateMethodSize(moveNextMethod);
        }

        private static int CalculateMethodSize(MethodDefinition method)
        {
            if (!method.HasBody)
                return 0;

            return method.Body.CodeSize;
        }
    }

    public static class MethodDefinitionExtensions
    {
        public static bool IsAsyncMethod(this MethodDefinition method)
        {
            return method.CustomAttributes.Any(attr => attr.AttributeType.Name == "AsyncStateMachineAttribute");
        }
    }
}
