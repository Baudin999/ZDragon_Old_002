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
            var _mod = astTypeField.Types.First().Value;
            var _type = astTypeField.Types.Last().Value;

            var restrictions = String.Join(Environment.NewLine, astTypeField.Restrictions.Select(r => $"{r.Key} {r.Value}"));

            return $@"
<tr>
    <td>{astTypeField.Name}</td>
    <td>{string.Join(" ", astTypeField.Types.Select(t => t.Value).ToList())}</td>
    <td>{_mod != "Maybe"}</td>
    <td>{restrictions}</td>
    <td>
        From module: <a href=""/index.html?path=preview&module={astTypeField.Module}"" target='_parent'>{astTypeField.Module}</a>
        <br />
        {string.Join(" ", astTypeField.Annotations.Select(a => a.Value).ToList())}
    </td>
</tr>
";
        }

        public override string VisitASTType(ASTType astType)
        {
            var typeDescription = $@"From module: <a href=""/index.html?path=preview&module={astType.Module}"" target='_parent'>{astType.Module}</a>
{string.Join(" ", astType.Annotations.Select(a => a.Value).ToList()).Trim()}";

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

        public override string VisitASTAlias(ASTAlias astAlias)
        {
            var typeDescription = $@"From module: <a href=""/index.html?path=preview&module={astAlias.Module}"" target='_parent'>{astAlias.Module}</a>
{string.Join(" ", astAlias.Annotations.Select(a => a.Value).ToList()).Trim()}";

            var _mod = astAlias.Types.First().Value;
            var _type = astAlias.Types.Last().Value;
            var restrictions = String.Join(Environment.NewLine, astAlias.Restrictions.Select(r => $"{r.Key} {r.Value}"));

            return $@"
<div class=""table-container"">
<table>
    <thead>
        <tr>
            <th colspan=""5"">{astAlias.Name}</th>
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
        <tr>
    <td>{astAlias.Name}</td>
    <td>{string.Join(" ", astAlias.Types.Select(t => t.Value).ToList())}</td>
    <td>{_mod != "Maybe"}</td>
    <td>{restrictions}</td>
    <td>
        From module: <a href=""/index.html?path=preview&module={astAlias.Module}"" target='_parent'>{astAlias.Module}</a>
        <br />
        {string.Join(" ", astAlias.Annotations.Select(a => a.Value).ToList())}
    </td>
</tr>
    <tbody>
</table>
</div>
";
        }
    }
}
