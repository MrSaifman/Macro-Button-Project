using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_App_WinUI3
{
    public class AppInformation
    {
        public string Name { get; set; }
        public BitmapImage Icon { get; set; }
        public string Path { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var program = (AppInformation)obj;
            return Name == program.Name && Icon.UriSource == program.Icon.UriSource && Path == program.Path;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Icon.UriSource, Path);
        }
    }
}
