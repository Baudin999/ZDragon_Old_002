using System;
using System.Collections.Generic;
using System.Linq;
using CLI.Models;
using Compiler;
using Compiler.AST;

namespace CLI
{
    public partial class Project
    {
        public Topology GetTopology(bool includeDetails)
        {
            var nodes = new List<TopologyNode>();
            var edges = new List<TopologyEdge>();

            this.Modules.ToList().ForEach(m =>
            {
                var moduleName = "m::" + m.Name;
                nodes.Add(new TopologyNode(moduleName, m.Name, new TopologyColor("Orange")) { Module = m.Name });
                m.Generator.AST.ForEach(node =>
                {
                    if (node is ASTImport)
                    {
                        var _import = ((ASTImport)node);
                        edges.Add(new TopologyEdge(moduleName, "m::" + _import.ModuleName, "")
                        {
                            Arrows = "to"
                        });
                    } else if (includeDetails && node is ASTType)
                    {
                        var _type = ((ASTType)node);
                        var _id = $"{m.Name}.{_type.Name}";
                        nodes.Add(new TopologyNode(_id, _type.Name));
                        edges.Add(new TopologyEdge(moduleName, _id, ""));
                    } else if (includeDetails && node is ASTAlias alias)
                    {
                        var _id = $"{m.Name}.{alias.Name}";
                        nodes.Add(new TopologyNode(_id, alias.Name, new TopologyColor("#0096a0")));
                        edges.Add(new TopologyEdge(moduleName, _id, ""));
                    } else if (includeDetails && node is ASTData data)
                    {
                        var _id = $"{m.Name}.{data.Name}";
                        nodes.Add(new TopologyNode(_id, data.Name, new TopologyColor("#fef6e1")));
                        edges.Add(new TopologyEdge(moduleName, _id, ""));

                        foreach (var option in data.Options)
                        {
                            //edges.Add(new TopologyEdge(_id, $"{data.Module}.{option.Name}", ""));
                        }
                    }
                });
            });

            return new Topology(nodes, edges);
        }
    }

}


/*
 * The Network
 *
 * A network is the topology of the types and modules.
 */ 