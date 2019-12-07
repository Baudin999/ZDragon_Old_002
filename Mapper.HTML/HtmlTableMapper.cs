using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;

namespace Mapper.HTML
{
    public class HtmlTableMapper : VisitorDefault<string>
    {
        public HtmlTableMapper(ASTGenerator generator) : base(generator) { }

        public override string VisitASTTypeField(ASTTypeField astTypeField)
        {
            var _mod = astTypeField.Type.First().Value;
            var _type = astTypeField.Type.Last().Value;

            var restrictions = String.Join(Environment.NewLine, astTypeField.Restrictions.Select(r => $"{r.Key} {r.Value}"));

            return $@"
<tr>
    <td>{astTypeField.Name}</td>
    <td>{string.Join(" ", astTypeField.Type.Select(t => t.Value).ToList())}</td>
    <td>{_mod != "Maybe"}</td>
    <td>{restrictions}</td>
    <td>{string.Join(" ", astTypeField.Annotations.Select(a => a.Value).ToList())}</td>
</tr>
";
        }

        public override string VisitASTType(ASTType astType)
        {
            var typeDescription = string.Join(" ", astType.Annotations.Select(a => a.Value).ToList()).Trim();
            if (typeDescription.Length == 0) typeDescription = "No Description";
            return $@"
<div class=""table-container"">
<table>
    <thead>
        <tr>
            <th colspan=""5"">{astType.Name}</th>
        </tr>
        <tr class=""description"">
            <th colspan=""5"">{typeDescription}</th>
        </tr>
        <tr>
            <th>Name</th>
            <th>Type</th>
            <th>Required</th>
            <th>Restrictions</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        {String.Join(Environment.NewLine, astType.Fields.Select(Visit).ToList())}
    <tbody>
</table>
</div>
";
        }
    }
}
