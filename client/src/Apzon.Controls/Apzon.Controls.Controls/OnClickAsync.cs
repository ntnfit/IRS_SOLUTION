using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Apzon.Controls.Controls;

[ToolboxItem(true)]
public delegate Task OnClickAsync(object sender, EventArgs e);
