using System;
using Configuration;

namespace CLI
{
	public partial class Project
	{
		private void InitCarConfig()
        {
            this.CarConfig = CarConfig.Load(this.ConfigPath);
            if (this.CarConfig is null)
            {
                this.CarConfig = new CarConfig();
                this.CarConfig.Save(this.ConfigPath);
            }
        }
	}
}
