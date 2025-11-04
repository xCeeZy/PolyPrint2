using PolyPrint2.Model;
using System.Windows;

namespace PolyPrint2
{
    public partial class App : Application
    {
        public static PolyPrintEntities context = new PolyPrintEntities();
        public static Users CurrentUser { get; set; }
    }
}
