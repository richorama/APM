using System.IO;

namespace Two10.APM
{
    public class PluginSummary
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public string DisplayName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(Name);
            }
        }

        public override string ToString()
        {
            if (this.Size == 0)
            {
                return this.DisplayName;
            }
            return this.DisplayName.PadRight(50, ' ') + string.Format(new FileSizeFormatProvider(), "{0:fs}", this.Size).PadLeft(7, ' ');
        }
    }
}
