using System.Resources;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal static class ResourceHelper
    {
        private static ResourceManager _resourceManager;

        private static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager == null)
                    _resourceManager = new ResourceManager("WeifenLuo.WinFormsUI.Docking.Strings",
                                                           typeof (ResourceHelper).Assembly);
                return _resourceManager;
            }
        }

        public static string GetString(string name)
        {
            return ResourceManager.GetString(name);
        }
    }
}