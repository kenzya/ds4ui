using DS4Control;

namespace DS4Service
{
    public class DS4Events
    {
        public DS4Events()
        { }

        public void EnableExclusiveMode()
        {
            System.Diagnostics.Process[] rundll32 = System.Diagnostics.Process.GetProcessesByName("rundll32");
            foreach (System.Diagnostics.Process rundll32Instance in rundll32)
            {
                foreach (System.Diagnostics.ProcessModule module in rundll32Instance.Modules)
                {
                    if (module.FileName.Contains("joy.cpl"))
                    {
                        module.Dispose();
                    }
                }
            }

            Global.setUseExclusiveMode(true);
            Global.Save();
        }

        public void DisableExclusiveMode()
        {
            System.Diagnostics.Process[] rundll32 = System.Diagnostics.Process.GetProcessesByName("rundll32");
            foreach (System.Diagnostics.Process rundll32Instance in rundll32)
            {
                foreach (System.Diagnostics.ProcessModule module in rundll32Instance.Modules)
                {
                    if (module.FileName.Contains("joy.cpl"))
                    {
                        module.Dispose();
                    }
                }
            }

            Global.setUseExclusiveMode(false);
            Global.Save();
        }
    }
}
