using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOpenStackUI
{
    /// <summary>
    /// Main model for the view
    /// </summary>
    public class FormModel
    {
        /// <summary>
        /// Represents the current container
        /// </summary>
        public NodeModel Root { get; set; }

        /// <summary>
        /// Represents the current node
        /// </summary>
        public NodeModel CurrentNode { get; set; }

        /// <summary>
        /// Represents the list of containers
        /// </summary>
        public FilesModel Containers { get; set; }

        /// <summary>
        /// Represents the current container
        /// </summary>
        public FileModel CurrentContainer { get; set; }
    }
}
