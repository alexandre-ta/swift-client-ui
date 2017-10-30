using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOpenStackUI
{
    /// <summary>
    /// This class represents the modal form with a loading gif and a message to allow user to wait
    /// </summary>
    public partial class LoadingDialog : Form
    {
            /// <summary>
            /// Constructor
            /// </summary>
            public LoadingDialog()
            {
                InitializeComponent();
            }

        }
    }
