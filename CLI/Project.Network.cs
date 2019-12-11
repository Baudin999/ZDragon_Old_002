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
        public Topology GetTopology()
        {
            var nodes = new List<TopologyNode>();
            var edges = new List<TopologyEdge>();

            this.Modules.ToList().ForEach(m =>
            {
                nodes.Add(new TopologyNode(m.Name, m.Name, new TopologyColor("Orange")) { Module = m.Name });
                m.Generator.AST.ForEach(node =>
                {
                    if (node is ASTImport)
                    {
                        var _import = ((ASTImport)node);
                        edges.Add(new TopologyEdge(m.Name, _import.ModuleName, "")
                        {
                            Arrows = "to"
                        });
                    } else if (node is ASTType)
                    {
                        var _type = ((ASTType)node);
                        var _id = $"{m.Name}.{_type.Name}";
                        nodes.Add(new TopologyNode(_id, _type.Name));
                        edges.Add(new TopologyEdge(m.Name, _id, ""));
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