

#pragma checksum "C:\Users\nikit\Documents\C#\Шахматы\Шахматы\Шахматы.Windows\View\TwoPlayers.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D4BAE2E1360E00CB7BEFF34767EB2FA9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Шахматы.View
{
    partial class TwoPlayers : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 99 "..\..\View\TwoPlayers.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.resign_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 100 "..\..\View\TwoPlayers.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.draw_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 101 "..\..\View\TwoPlayers.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.download_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


