using System.IO;
using Nebula.Models;
using Nebula.Parser;
using System.Linq;
using System;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Objects;
using System.Collections.Generic;
using Core.Plugin;
using Core.Compiler.Objects;

namespace Nebula.Renderers
{
    public abstract class AbstractRenderer
    {
        private const int TAB_SIZE = 4;
        
        protected int IndentLevel { get; set; }

        protected List<string> CurrentOutput { get; set; }

        protected AbstractCompiler Compiler { get; set; }

        protected ApiConfig ActiveConfig { get; set; }

        protected IRenderPlugin RenderPlugin { get; set; }

        protected TemplateMeta Meta { get; set; }

        protected Project Project { get; set; }
        
        protected AbstractRenderer(AbstractCompiler compiler, IRenderPlugin renderPlugin)
        {
            IndentLevel = 0;
            Compiler = compiler;
            RenderPlugin = renderPlugin;
        }

        public void Render(List<OutputFile> outputFiles, Project project, TemplateMeta meta)
        {
            Project = project;
            Meta = meta;
            
            foreach (var file in outputFiles)
            {
                var output = new List<string>();
                CurrentOutput = output;
                RenderNode(file.Root);
                file.RawContent = output;
            }
        }

        protected string Indent()
        {
            if (IndentLevel < 0)
            {
                IndentLevel = 0;
            }
            return new String(' ', IndentLevel * TAB_SIZE);
        }

        protected void WriteIndented(string text)
        {
            CurrentOutput.Add(Indent() + text);
        }

        protected void WriteIndented(List<string> text)
        {
            foreach (var t in text)
            {
                WriteIndented(t);
            }
        }

        protected abstract List<string> RenderDocBlock(string description, Dictionary<string, string> paramValues);

        protected void RenderNode(RootObject node)
        {
            switch (node)
            {
                case AbstractClass<EntityNode> ac:
                    RenderEntityClass(ac);
                    break;
                case AbstractClass<ApiNode> ac:
                    RenderApiClass(ac);
                    break;
                case AbstractConstructor ac:
                    RenderAbstractConstructor(ac);
                    break;
                case AbstractDataType adt:
                    RenderAbstractDataType(adt);
                    break;
                case AbstractFunction af:
                    RenderAbstractFunction(af);
                    break;
                case AbstractNamespace an:
                    RenderAbstractNamespace(an);
                    break;
                case AbstractProperty<EntityNode> ap:
                    RenderAbstractProperty(ap);
                    break;
                case GenericClass gc:
                    RenderGenericClass(gc);
                    break;
                case GenericConstructor gc:
                    RenderGenericConstructor(gc);
                    break;
                case GenericProperty gp:
                    RenderGenericProperty(gp);
                    break;
                case GenericVariableDefinition gvd:
                    RenderGenericVariableDefinition(gvd);
                    break;
                case GenericFunction gf:
                    RenderGenericFunction(gf);
                    break;
            }
        }

        protected abstract void RenderEntityClass(AbstractClass<EntityNode> ac);
        protected abstract void RenderApiClass(AbstractClass<ApiNode> ac);
        protected abstract void RenderAbstractConstructor(AbstractConstructor ac);
        protected abstract string RenderAbstractDataType(AbstractDataType abstractData);
        protected abstract string RenderAbstractVariableDefinition(AbstractVariableDefinition variable);
        protected abstract void RenderAbstractFunction(AbstractFunction function);
        protected abstract void RenderAbstractNamespace(AbstractNamespace ns);
        protected abstract void RenderAbstractProperty(AbstractProperty<EntityNode> prop);
        protected abstract void RenderGenericClass(GenericClass genericClass);
        protected abstract void RenderGenericConstructor(GenericConstructor genericConstructor);
        protected abstract void RenderGenericProperty(GenericProperty prop);
        protected abstract string RenderGenericVariableDefinition(GenericVariableDefinition variableDefinition);
        protected abstract void RenderGenericFunction(GenericFunction genericFunction);
        protected abstract string ConvertTypeName(string inputType);
        protected abstract void RenderGenericTryCatch(GenericTryCatch tryCatch);

        protected void RenderGenericProperties(List<GenericProperty> properties)
        {
            foreach (var prop in properties)
            {
                RenderGenericProperty(prop);
            }
        }

        protected void RenderGenericFunctions(List<GenericFunction> functions)
        {
            foreach (var func in functions)
            {
                RenderGenericFunction(func);
            }
        }

        protected void RenderNodes<T>(List<T> nodes) where T : RootObject
        {
            foreach (var node in nodes)
            {
                RenderNode(node);
            }
        }
    }
}