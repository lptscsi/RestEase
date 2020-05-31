﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using RestEase.Platform;
using RestEase.Implementation.Emission;
using System.Collections.Generic;

namespace RestEase.Implementation
{
    internal class EmitImplementationFactory
    { 
        private static readonly string moduleBuilderName = "RestEaseAutoGeneratedModule";
        private readonly Emitter emitter;

        public EmitImplementationFactory()
        {
            var assemblyName = new AssemblyName(RestClient.FactoryAssemblyName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleBuilderName);
            this.emitter = new Emitter(moduleBuilder);
        }

        public Type BuildEmitImplementation(Type interfaceType)
        {
            var analyzer = new ReflectionTypeAnalyzer(interfaceType);
            var typeModel = analyzer.Analyze();
            var diagnosticReporter = new DiagnosticReporter(typeModel);
            var generator = new ImplementationGenerator(typeModel, this.emitter, diagnosticReporter);
            return generator.Generate().Type;
        }
    }
}
