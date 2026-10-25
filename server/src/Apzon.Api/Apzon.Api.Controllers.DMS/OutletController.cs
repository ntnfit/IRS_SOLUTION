using System;
using System.Collections.Concurrent;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;

namespace Apzon.Api.Controllers.DMS;

public class OutletController : BaseApiController
{
	[HttpGet]
	public DataTable GetList()
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			DataTable list = UnitOfWork.Outlet.GetList(userName);
			if (list != null)
			{
				list.BeginInit();
				list.Columns.Add("ImageBase64", typeof(string));
				list.EndInit();
				if (list.IsNotNull())
				{
					var source = list.AsEnumerable().Select((DataRow t, int index) => new
					{
						index = index,
						Item = t
					});
					ConcurrentBag<Tuple<int, string>> lstItemXml = new ConcurrentBag<Tuple<int, string>>();
					if (!Parallel.ForEach(source, row =>
					{
						try
						{
							string text = Function.ToString(row.Item["LinkDownload"]);
							if (!string.IsNullOrEmpty(text))
							{
								string image100Base = GetImage100Base64(text);
								lstItemXml.Add(new Tuple<int, string>(row.index, image100Base));
							}
						}
						catch (Exception ex3)
						{
							Logging.Write("ERROR", ex3);
						}
					}).IsCompleted)
					{
						throw new Exception("Could not complete action-parallel mapping xml");
					}
					int i;
					for (i = 0; i < list.Rows.Count; i++)
					{
						list.Rows[i]["ImageBase64"] = lstItemXml.FirstOrDefault((Tuple<int, string> t) => t.Item1 == i)?.Item2;
					}
					try
					{
						GC.Collect();
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", ex);
					}
					for (int num = 0; num < list.Rows.Count; num++)
					{
					}
				}
				return list;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
		}
		return new DataTable();
	}

	public Bitmap ResizeBitmap(Image bmp, int width, int height)
	{
		try
		{
			Bitmap bitmap = new Bitmap(width, height);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(bmp, 0, 0, width, height);
			}
			return bitmap;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return bmp as Bitmap;
	}
}
