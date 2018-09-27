using System.IO;
using Nebula.Models;
using Nebula.Parser;
using System.Linq;
using System;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Objects;
using System.Collections.Generic;

namespace Nebula.Renderers
{
    public abstract class AbstractRenderer
    {
        protected int IndentLevel { get; set; }

        protected List<string> CurrentOutput { get; set; }
        
        protected AbstractRenderer()
        {
            IndentLevel = 0;
        }

        public void Render(List<OutputFile> outputFiles)
        {
            foreach (var file in outputFiles)
            {
                var output = new List<string>();
                CurrentOutput = output;
                RenderNode(file.Root);
            }
        }

        protected string Indent()
        {
            if (IndentLevel < 0)
            {
                IndentLevel = 0;
            }
            return new String('\t', IndentLevel);
        }

        protected void WriteIndented(string text)
        {
            CurrentOutput.Add(Indent() + text);
        }

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
                case AbstractProperty ap:
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
        protected abstract void RenderAbstractProperty(AbstractProperty prop);
        protected abstract void RenderGenericClass(GenericClass genericClass);
        protected abstract void RenderGenericConstructor(GenericConstructor genericConstructor);
        protected abstract void RenderGenericProperty(GenericProperty prop);
        protected abstract void RenderGenericVariableDefinition(GenericVariableDefinition variableDefinition);
        protected abstract void RenderGenericFunction(GenericFunction genericFunction);
        protected abstract string ConvertTypeName(string inputType);
    }
}