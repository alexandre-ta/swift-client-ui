using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOpenStackUI
{
    /// <summary>
    /// Represents a node including a fileModel
    /// </summary>
    public class NodeModel
    {
        /// <summary>
        /// Represents a file
        /// </summary>
        private readonly FileModel value;

        /// <summary>
        /// Represents children of this node
        /// </summary>
        public List<NodeModel> children = new List<NodeModel>();

        /// <summary>
        /// Represents the parent node
        /// </summary>
        public NodeModel Parent { get; private set; }

        /// <summary>
        /// To access the value of the node
        /// </summary>
        public FileModel Value { get { return this.value; } }

        public ReadOnlyCollection<NodeModel> Children
        {
            get { return this.children.AsReadOnly(); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public NodeModel(FileModel value)
        {
            this.value = value;
            value.Node = this;
        }

        /// <summary>
        /// Get the child of this node with the index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public NodeModel this[int i]
        {
            get { return this.children[i]; }
        }

        /// <summary>
        /// Find a child
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public NodeModel FindNode(String node)
        {
            NodeModel res = this.children.Where(x => x.value.Name.Equals(node)).FirstOrDefault();
            return res;
        }

        /// <summary>
        /// Add a file child and sort children order by ascending
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public NodeModel AddChild(FileModel value)
        {
            value.Node = this;
            var node = new NodeModel(value) { Parent = this };
            this.children.Add(node);
            this.children = (from s in this.children
             orderby s.Value.Name ascending
             select s).ToList();
            return node;
        }

        /// <summary>
        /// Add a node child and sort children order by ascending
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public NodeModel AddChild(NodeModel value)
        {
            value.Parent = this;
            this.children.Add(value);
            this.children = (from s in this.children
                             orderby s.Value.Name ascending
                             select s).ToList();
            return value;
        }

        /// <summary>
        /// Add a list of files into children
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public NodeModel[] AddChildren(params FileModel[] values)
        {
            foreach (FileModel tmp in values)
            {
                tmp.Node = this;
            }
            return values.Select(AddChild).ToArray();
        }

        /// <summary>
        /// Remove child in children
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Boolean RemoveChild(NodeModel node)
        {
            return this.children.Remove(node);
        }
    }
}
