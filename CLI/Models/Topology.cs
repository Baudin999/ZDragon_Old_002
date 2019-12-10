
using System.Collections.Generic;

namespace CLI.Models
{
    public class Topology
    {
        public List<TopologyNode> Nodes { get; set; }
        public List<TopologyEdge> Edges { get; set; }

        public Topology(List<TopologyNode> nodes, List<TopologyEdge> edges)
        {
            this.Nodes = nodes;
            this.Edges = edges;
        }
    }

    public class TopologyNode
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public TopologyColor? Color { get; set; }
        public string? Module { get; set; }
        public string Shape { get; set; } = "hexagon";


        public TopologyNode(string id, string label, TopologyColor? color = null)
        {
            this.Id = id;
            this.Label = label;
            this.Color = color;
        }
    }

    public class TopologyEdge
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Label { get; set; }

        public TopologyEdge(string fromId, string toId, string label)
        {
            this.From = fromId;
            this.To = toId;
            this.Label = label;
        }
    }

    public class TopologyColor
    {
        public string Background { get; set; }
        public string Border { get; set; }

        public TopologyColor(string background, string border)
        {
            this.Background = background;
            this.Border = border;
        }
        public TopologyColor(string color)
        {
            this.Background = color;
            this.Border = color;
        }
    }
}
