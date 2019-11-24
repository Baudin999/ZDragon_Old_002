using System;
using System.Linq;
using Compiler.AST;

namespace Mapper.HTML
{
    public static class Extensions
    {
        public static string ToMermaidString(this ASTDataOption astDataOption)
        {
            return $"{astDataOption.Name} {string.Join(" ", astDataOption.Parameters)}";
        }
    }
}
