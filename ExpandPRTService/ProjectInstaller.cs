using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace SF.Expand.Switch
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}