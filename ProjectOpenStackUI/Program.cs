using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOpenStackUI
{
    static class Program
    {
        /// <summary>
        /// Run application
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            OpenStackView view = new OpenStackView();
            FormModel model = new FormModel();
            OpenStackController controller = new OpenStackController(model, view);
            Application.Run(view);
        }
    }
}
