using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.DataServices;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MobileApp;
using Newtonsoft.Json;

namespace Apzon.Api.Controllers.DashBoard;

public class DashBoardController : BaseApiController
{
	private string _messageCodePrefix = "213001";

	[HttpGet]
	public DataSet GetDataCategories(string usercode)
	{
		return UnitOfWork.DashBoard.GetDataCategories(usercode);
	}

	[HttpGet]
	public DataTable GetDataController(int tabId)
	{
		return UnitOfWork.DashBoard.GetDataController(tabId);
	}

	[HttpPost]
	public DataTable GetDataQuery([FromBody] Tuple<string, string, DataTable> model)
	{
		return UnitOfWork.DashBoard.GetDataQuery(model.Item1, model.Item2, model.Item3);
	}

	[HttpPost]
	public DataSet GetDataChidController([FromBody] Tuple<int, DataTable> model)
	{
		return UnitOfWork.DashBoard.GetDataChidController(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable GetDataObj(int tabId)
	{
		return UnitOfWork.DashBoard.GetDataObj(tabId);
	}

	[HttpPost]
	public ResponseResult GetCustomerDashboard()
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			DataTable customerDashboard = UnitOfWork.MobileDashboard.GetCustomerDashboard(SessionUserInfo.UserName);
			customerDashboard.TableName = "DashboardSummaries";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					DashboardSummaries = customerDashboard
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult GetCustomerDashboardDetail([FromBody] ProductInput product)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			if (string.IsNullOrEmpty(product.DashboardItem))
			{
				return new ResponseResult
				{
					Message = "Invalid DashboardItem",
					MessageCode = _messageCodePrefix + "404"
				};
			}
			List<DashboardDetail> list = new List<DashboardDetail>();
			DataSet customerDashboardDetail = UnitOfWork.MobileDashboard.GetCustomerDashboardDetail(SessionUserInfo.UserName, product.DashboardItem);
			if (customerDashboardDetail.IsNotNull())
			{
				DataTable dataTable = customerDashboardDetail.Tables[0];
				DataTable dataTable2 = customerDashboardDetail.Tables[1];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DashboardDetail dashboardDetail = new DashboardDetail();
					dashboardDetail.Title = Function.ToString(dataTable.Rows[i]["Title"]);
					dashboardDetail.OrderId = Function.ToString(dataTable.Rows[i]["OrderId"]);
					dashboardDetail.Status = Function.ToString(dataTable.Rows[i]["Status"]);
					dashboardDetail.Editable = Function.ToString(dataTable.Rows[i]["Editable"]);
					dashboardDetail.Cancelable = Function.ToString(dataTable.Rows[i]["Cancelable"]);
					dashboardDetail.OrderType = Function.ToString(dataTable.Rows[i]["OrderType"]);
					dashboardDetail.Attributes = new List<DashboardAttributes>();
					DataRow[] array = dataTable2.Select("OrderId = " + dashboardDetail.OrderId);
					if (array != null && array.Length != 0)
					{
						DataRow[] array2 = array;
						foreach (DataRow dataRow in array2)
						{
							for (int k = 1; k < dataTable2.Columns.Count; k++)
							{
								DashboardAttributes dashboardAttributes = new DashboardAttributes();
								dashboardAttributes.Name = dataTable2.Columns[k].ColumnName;
								dashboardAttributes.Value = Function.ToString(dataRow[dashboardAttributes.Name]);
								dashboardDetail.Attributes.Add(dashboardAttributes);
							}
						}
					}
					list.Add(dashboardDetail);
				}
			}
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					DashboardDetails = list
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult GetFormStructure()
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			new List<CustomerAvailableProductsModel>();
			DataSet formStructure = UnitOfWork.MobileDashboard.GetFormStructure(SessionUserInfo.UserName);
			FormStructureModel formStructureModel = new FormStructureModel();
			if (!formStructure.IsHasStructure())
			{
				return new ResponseResult
				{
					Message = "",
					Content = new
					{
						FormStructure = formStructureModel
					},
					MessageCode = "0"
				};
			}
			formStructureModel.OrderForm = new FormOpenModel
			{
				FormCaption = "Đặt hàng",
				Fields = new List<FieldFormModel>()
			};
			DataTable dataTable = formStructure.Tables[0];
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.Rows[i];
				FieldFormModel fieldFormModel = new FieldFormModel();
				fieldFormModel.Name = Function.ToString(dataRow[0]);
				fieldFormModel.Label = Function.ToString(dataRow[1]);
				fieldFormModel.DefaultValue = Function.ToString(dataRow[2]);
				fieldFormModel.DataType = Function.ToString(dataRow[3]);
				fieldFormModel.Value = Function.ToString(dataRow[4]);
				fieldFormModel.Nullable = Function.ToString(dataRow[5]);
				fieldFormModel.OrderNo = Function.ToString(i);
				string text = Function.ToString(dataRow[6]);
				string text2 = Function.ToString(dataRow[7]);
				if (fieldFormModel.DataType.Equals("list") && !string.IsNullOrEmpty(text))
				{
					fieldFormModel.ListOptions = new DataTable();
					fieldFormModel.ListOptions.Columns.Add("Name", typeof(string));
					fieldFormModel.ListOptions.Columns.Add("Value", typeof(string));
					string[] array = text.Split(';');
					string[] array2 = text2.Split(';');
					int num = 0;
					string[] array3 = array2;
					foreach (string text3 in array3)
					{
						if (!string.IsNullOrEmpty(text3))
						{
							DataRow dataRow2 = fieldFormModel.ListOptions.NewRow();
							dataRow2[0] = ((array.Length > num) ? array[num] : text3);
							dataRow2[1] = text3;
							fieldFormModel.ListOptions.Rows.Add(dataRow2);
						}
						num++;
					}
				}
				formStructureModel.OrderForm.Fields.Add(fieldFormModel);
			}
			formStructureModel.AddedServiceForm = new FormOpenModel
			{
				FormCaption = "Yêu Cầu Gia Công",
				Fields = new List<FieldFormModel>()
			};
			DataTable dataTable2 = formStructure.Tables[1];
			for (int k = 0; k < dataTable2.Rows.Count; k++)
			{
				DataRow dataRow3 = dataTable2.Rows[k];
				FieldFormModel fieldFormModel2 = new FieldFormModel();
				fieldFormModel2.Name = Function.ToString(dataRow3[0]);
				fieldFormModel2.Label = Function.ToString(dataRow3[1]);
				fieldFormModel2.DefaultValue = Function.ToString(dataRow3[2]);
				fieldFormModel2.DataType = Function.ToString(dataRow3[3]);
				fieldFormModel2.OrderNo = Function.ToString(k);
				fieldFormModel2.Value = Function.ToString(dataRow3[4]);
				fieldFormModel2.Nullable = Function.ToString(dataRow3[5]);
				fieldFormModel2.OrderNo = Function.ToString(k);
				string text4 = Function.ToString(dataRow3[6]);
				string text5 = Function.ToString(dataRow3[7]);
				if (fieldFormModel2.DataType.Equals("list") && !string.IsNullOrEmpty(text4))
				{
					fieldFormModel2.ListOptions = new DataTable();
					fieldFormModel2.ListOptions.Columns.Add("Name", typeof(string));
					fieldFormModel2.ListOptions.Columns.Add("Value", typeof(string));
					string[] array4 = text4.Split(';');
					string[] array5 = text5.Split(';');
					int num2 = 0;
					string[] array3 = array5;
					foreach (string text6 in array3)
					{
						DataRow dataRow4 = fieldFormModel2.ListOptions.NewRow();
						dataRow4[0] = ((array4.Length > num2) ? array4[num2] : text6);
						dataRow4[1] = text6;
						fieldFormModel2.ListOptions.Rows.Add(dataRow4);
						num2++;
					}
				}
				formStructureModel.AddedServiceForm.Fields.Add(fieldFormModel2);
			}
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					FormStructure = formStructureModel
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult GetCustomerAvailableProducts()
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			List<string> list = new List<string>();
			list.Add("https://www.upsieutoc.com/images/2019/03/14/VHM-Logo.png");
			List<CustomerAvailableProductsModel> list2 = new List<CustomerAvailableProductsModel>();
			DataSet customerAvailableProducts = UnitOfWork.MobileDashboard.GetCustomerAvailableProducts(SessionUserInfo.UserName);
			DataTable dataTable = customerAvailableProducts.Tables[0];
			DataTable dataTable2 = customerAvailableProducts.Tables[1];
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				CustomerAvailableProductsModel item = new CustomerAvailableProductsModel();
				item.Title = Function.ToString(dataTable.Rows[i]["ItemName"]);
				item.ProductId = Function.ToString(dataTable.Rows[i]["ItemCode"]);
				item.IsFavorite = (Function.ToString(dataTable.Rows[i]["U_Favorite"]).Equals("Y") ? "1" : "0");
				item.ImageUrl = list.OrderBy((string x) => Guid.NewGuid()).FirstOrDefault();
				item.Color = "";
				item.Attributes = new DataTable();
				item.Attributes.Columns.Add("Name", typeof(string));
				item.Attributes.Columns.Add("Value", typeof(string));
				DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ItemCode"]).Equals(item.ProductId));
				if (dataRow != null)
				{
					foreach (DataColumn column in dataTable2.Columns)
					{
						if (!column.ColumnName.Equals("ItemCode"))
						{
							DataRow dataRow2 = item.Attributes.NewRow();
							dataRow2["Name"] = column.ColumnName;
							dataRow2["Value"] = Function.ToString(dataRow[column]);
							item.Attributes.Rows.Add(dataRow2);
						}
					}
				}
				list2.Add(item);
			}
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Products = list2
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult AddProductToFavorite([FromBody] ProductInput product)
	{
		try
		{
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("CardCode", typeof(string));
			dataTable.Columns.Add("Substitute", typeof(string));
			dataTable.Columns.Add("ShowSCN", typeof(string));
			dataTable.Rows.Add(product.ProductId, SessionUserInfo.UserName, SessionUserInfo.UserName, "N");
			HttpResult httpResult = unitOfWork.BusinessPartnerCatalogNumbers.Save(dataTable, ApzObjectTypes.oBusinessPartners, SessionUserInfo.SessionDiServer);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "0"
				};
			}
			return new ResponseResult
			{
				Message = httpResult.Message,
				MessageCode = _messageCodePrefix + "504"
			};
		}
		catch (Exception ex)
		{
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult RemoveProductFromFavorite([FromBody] ProductInput product)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("CardCode", typeof(string));
			dataTable.Columns.Add("Substitute", typeof(string));
			dataTable.Columns.Add("ShowSCN", typeof(string));
			dataTable.Rows.Add(product.ProductId, SessionUserInfo.UserName, SessionUserInfo.UserName, "N");
			string text = unitOfWork.BusinessPartnerCatalogNumbers.RemoveItem(dataTable, ApzObjectTypes.oBusinessPartners, SessionUserInfo.SessionDiServer);
			if (string.IsNullOrEmpty(text))
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "0"
				};
			}
			return new ResponseResult
			{
				Message = text,
				MessageCode = _messageCodePrefix + "504"
			};
		}
		catch (Exception ex)
		{
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult CreateNewOrder()
	{
		try
		{
			string result = Request.Content.ReadAsStringAsync().Result;
			OrderModel orderModel = JsonConvert.DeserializeObject<OrderModel>(result);
			if (orderModel == null)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Paramater product does not exist");
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), result);
				return new ResponseResult
				{
					Message = "Paramater product does not exist",
					MessageCode = _messageCodePrefix + "404"
				};
			}
			if (orderModel.Products == null || orderModel.Products.Count == 0)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Paramater products does not exist");
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), result);
				return new ResponseResult
				{
					Message = "Paramater products does not exist",
					MessageCode = _messageCodePrefix + "404"
				};
			}
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocEntry = Function.ParseInt(orderModel.OrderId);
			if (documentsImpl.DocEntry <= 0)
			{
				documentsImpl.CardCode = SessionUserInfo.UserName;
			}
			documentsImpl.ObjType = 23;
			documentsImpl.DocStatus = "O";
			int bPBranch = UnitOfWork.MobileDashboard.GetBPBranch(SessionUserInfo.UserName);
			documentsImpl.Branch = ((bPBranch <= 0) ? 1 : bPBranch);
			documentsImpl.BPLId = ((bPBranch <= 0) ? 1 : bPBranch);
			documentsImpl.DocDate = DateTime.Now.Date;
			documentsImpl.DocDueDate = DateTime.Now.Date;
			documentsImpl.CreateDate = DateTime.Now.Date;
			DataSet dataSet = UnitOfWork.Commons.ExecuteSql("SELECT * FROM QUT1 WHERE DocEntry= " + documentsImpl.DocEntry, new List<QueryParamater>());
			if (dataSet != null && dataSet.Tables.Count == 2)
			{
				DataTable dataTable = dataSet.Tables[1].Clone();
				dataTable.Columns["Quantity"].Expression = "U_Width* U_Length*U_QtyUoM/100";
				for (int i = 0; i < orderModel.Products.Count; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					DataRow[] array = dataSet.Tables[1].Select("ItemCode='" + orderModel.Products[i].ProductId + "'");
					if (array != null && array.Length != 0)
					{
						dataRow.ItemArray = array[0].ItemArray;
					}
					dataRow["ItemCode"] = orderModel.Products[i].ProductId;
					dataRow["Dscription"] = orderModel.Products[i].Title;
					dataRow["LineNum"] = i;
					dataRow["WhsCode"] = ((documentsImpl.BPLId == 1) ? "SW03" : "HW03");
					dataRow["SlpCode"] = ((dataSet.Tables[1].Rows.Count > 0) ? dataSet.Tables[1].Rows[0]["SlpCode"] : DBNull.Value);
					dataRow["TrnsCode"] = ((dataSet.Tables[1].Rows.Count > 0) ? dataSet.Tables[1].Rows[0]["TrnsCode"] : DBNull.Value);
					dataRow["U_PriceType"] = "Price";
					dataRow["U_SaleType"] = "01";
					dataRow["U_CutType"] = "01";
					dataRow["U_ReqDel"] = "04";
					dataRow["U_isHold"] = "N";
					dataRow["U_Satus"] = "01";
					if (orderModel.Products[i].OrderRequest != null && orderModel.Products[i].OrderRequest.Fields != null)
					{
						for (int j = 0; j < orderModel.Products[i].OrderRequest.Fields.Count; j++)
						{
							if (dataTable.Columns.Contains(orderModel.Products[i].OrderRequest.Fields[j].Name))
							{
								dataRow[orderModel.Products[i].OrderRequest.Fields[j].Name] = GetDataByDataType(orderModel.Products[i].OrderRequest.Fields[j].DataType, Function.ToString(orderModel.Products[i].OrderRequest.Fields[j].Value));
							}
							if (orderModel.Products[i].OrderRequest.Fields[j].Name.Equals("ShipToCode"))
							{
								documentsImpl.ShipToCode = Function.ToString(GetDataByDataType(orderModel.Products[i].OrderRequest.Fields[j].DataType, Function.ToString(orderModel.Products[i].OrderRequest.Fields[j].Value)));
							}
						}
					}
					if (orderModel.Products[i].AddedServiceRequests != null && orderModel.Products[i].AddedServiceRequests.Count > 0)
					{
						DataTable udoByCode = UnitOfWork.UDOs.GetUdoByCode(new Common.Table_Code
						{
							Name = "OUDO",
							Code = "YCGC"
						});
						if (!udoByCode.IsNotNull())
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "UDO Table is invalid.");
							return new ResponseResult
							{
								Message = "UDO Table is invalid.",
								MessageCode = _messageCodePrefix + "501"
							};
						}
						DataSet dataSet2 = new DataSet();
						dataSet2.Tables.Add(udoByCode.Copy());
						DataTable dataByDocEntry = UnitOfWork.UDOs.GetDataByDocEntry(new Common.Table_Code
						{
							Name = "@YCGC",
							Code = "0"
						});
						dataByDocEntry.TableName = "YCGC";
						dataByDocEntry.Rows.Clear();
						DataRow dataRow2 = dataByDocEntry.NewRow();
						dataRow2["DocEntry"] = 0;
						dataRow2["U_ItemCode"] = orderModel.Products[i].ProductId;
						for (int k = 0; k < orderModel.Products[i].OrderRequest.Fields.Count; k++)
						{
							if (dataByDocEntry.Columns.Contains(orderModel.Products[i].OrderRequest.Fields[k].Name))
							{
								dataRow2[orderModel.Products[i].OrderRequest.Fields[k].Name] = GetDataByDataType(orderModel.Products[i].OrderRequest.Fields[k].DataType, Function.ToString(orderModel.Products[i].OrderRequest.Fields[k].Value));
							}
						}
						dataByDocEntry.Rows.Add(dataRow2);
						dataSet2.Tables.Add(dataByDocEntry);
						DataTable dataByDocEntry2 = UnitOfWork.UDOs.GetDataByDocEntry(new Common.Table_Code
						{
							Name = "@CGC1",
							Code = "0"
						});
						dataByDocEntry2.TableName = "CGC1";
						dataByDocEntry2.Rows.Clear();
						dataByDocEntry2.Columns["U_Qty"].Expression = "U_Width* U_Length*U_QtyUoM/100";
						for (int l = 0; l < orderModel.Products[i].AddedServiceRequests.Count; l++)
						{
							DataRow dataRow3 = dataByDocEntry2.NewRow();
							dataRow3["DocEntry"] = 0;
							dataRow3["LineId"] = l;
							for (int m = 0; m < orderModel.Products[i].AddedServiceRequests[l].Fields.Count; m++)
							{
								if (dataByDocEntry2.Columns.Contains(orderModel.Products[i].AddedServiceRequests[l].Fields[m].Name))
								{
									dataRow3[orderModel.Products[i].AddedServiceRequests[l].Fields[m].Name] = GetDataByDataType(orderModel.Products[i].AddedServiceRequests[l].Fields[m].DataType, Function.ToString(orderModel.Products[i].AddedServiceRequests[l].Fields[m].Value));
								}
							}
							dataByDocEntry2.Rows.Add(dataRow3);
						}
						dataSet2.Tables.Add(dataByDocEntry2);
						HttpResult httpResult = unitOfWork.UDOs.Save(dataSet2, SessionUserInfo.SessionDiServer);
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), result);
							return new ResponseResult
							{
								Message = httpResult.Message,
								MessageCode = _messageCodePrefix + "501"
							};
						}
						dataRow["U_ycgc"] = httpResult.Content;
					}
					else
					{
						dataRow["U_ycgc"] = DBNull.Value;
					}
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				dataTable.TableName = "Lines";
				documentsImpl.Lines = dataTable.ToDocumentLine();
				new List<DocumentsImpl>().Add(documentsImpl);
				HttpResult httpResult2 = unitOfWork.Documents.SaveData(documentsImpl, SessionId);
				if (!string.IsNullOrEmpty(httpResult2.Message))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), result);
					return new ResponseResult
					{
						Message = httpResult2.Message,
						MessageCode = _messageCodePrefix + "502"
					};
				}
				orderModel.OrderId = Function.ToString(httpResult2.Content);
				orderModel.CusCode = SessionUserInfo.UserName;
				OrderInput orderInput = new OrderInput();
				orderInput.OrderId = Function.ToString(httpResult2.Content);
				return EditOrder(orderInput);
			}
			return new ResponseResult
			{
				Message = "Can not get data",
				MessageCode = _messageCodePrefix + "404"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult EditOrder([FromBody] OrderInput Order)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			OrderOutputModel orderOutputModel = new OrderOutputModel();
			DataSet dataSet = new DataSet();
			orderOutputModel.OrderId = Order.OrderId;
			dataSet = UnitOfWork.MobileDashboard.GetOrderRequest(Function.ParseInt(Order.OrderId));
			orderOutputModel.OrderHeader = new ProductModel();
			orderOutputModel.OrderHeader.Lines = new List<FieldFormModel>();
			if (dataSet != null && dataSet.Tables.Count > 2 && dataSet.Tables[0].IsNotNull())
			{
				for (int i = 0; i < dataSet.Tables[0].Columns.Count; i++)
				{
					FieldFormModel fieldFormModel = new FieldFormModel();
					fieldFormModel.Label = dataSet.Tables[0].Columns[i].ColumnName;
					fieldFormModel.Value = Function.ToString(dataSet.Tables[0].Rows[0][i]);
					orderOutputModel.OrderHeader.Lines.Add(fieldFormModel);
				}
			}
			orderOutputModel.OrderItems = new List<ProductModel>();
			OrderModel orderModel = new OrderModel();
			orderModel.OrderId = Order.OrderId;
			orderModel.Products = new List<ProductModel>();
			if (dataSet != null && dataSet.Tables.Count > 2 && dataSet.Tables[1].IsNotNull())
			{
				var enumerable = from t in dataSet.Tables[1].AsEnumerable()
					group t by new
					{
						LineNum = Function.ToString(t["LineNum"])
					} into g
					select new
					{
						Key = g.Key,
						Items = g
					};
				int num = 0;
				foreach (var item in enumerable)
				{
					ProductModel productModel = new ProductModel();
					productModel.OrderRequest = new FormOpenModel();
					productModel.OrderRequest.Fields = new List<FieldFormModel>();
					ProductModel productModel2 = new ProductModel
					{
						Lines = new List<FieldFormModel>()
					};
					foreach (DataRow item2 in item.Items)
					{
						if (Function.ToString(item2["Name"]).Equals("ShipDate"))
						{
							productModel.Date = Function.ToString(item2["Value"]);
							productModel2.Date = Function.ToString(item2["Value"]);
						}
						if (Function.ToString(item2["Name"]).Equals("ItemCode"))
						{
							productModel.ProductId = Function.ToString(item2["Value"]);
							productModel2.ProductId = Function.ToString(item2["Value"]);
							continue;
						}
						if (Function.ToString(item2["Name"]).Equals("Dscription"))
						{
							productModel.Title = Function.ToString(item2["Value"]);
							productModel2.Title = Function.ToString(item2["Value"]);
							continue;
						}
						if (Function.ToString(item2["Name"]).Equals("Quantity"))
						{
							productModel.Quantity = Function.ToString(item2["Value"]);
							productModel2.Quantity = Function.ToString(item2["Value"]);
							continue;
						}
						FieldFormModel fieldFormModel2 = new FieldFormModel();
						fieldFormModel2.Name = Function.ToString(item2["Name"]);
						fieldFormModel2.Label = Function.ToString(item2["Label"]);
						fieldFormModel2.DefaultValue = Function.ToString(item2["DefaultValue"]);
						fieldFormModel2.DataType = Function.ToString(item2["DataType"]);
						fieldFormModel2.Value = item2["Value"];
						productModel.OrderRequest.Fields.Add(fieldFormModel2);
						productModel2.Lines.Add(fieldFormModel2);
					}
					DataRow[] array = dataSet.Tables[2].Select("LineNum = " + num);
					if (array != null && array.Length != 0)
					{
						productModel.AddedServiceRequests = new List<FormOpenModel>();
						FormOpenModel formOpenModel = new FormOpenModel();
						productModel.AddedServiceRequests.Add(formOpenModel);
						formOpenModel.Fields = new List<FieldFormModel>();
						DataRow[] array2 = array;
						foreach (DataRow dataRow in array2)
						{
							FieldFormModel fieldFormModel3 = new FieldFormModel();
							fieldFormModel3.Name = Function.ToString(dataRow["Name"]);
							fieldFormModel3.OrderNo = Function.ToString(dataRow["LineId"]);
							fieldFormModel3.Label = Function.ToString(dataRow["Label"]);
							fieldFormModel3.DefaultValue = Function.ToString(dataRow["DefaultValue"]);
							fieldFormModel3.DataType = Function.ToString(dataRow["DataType"]);
							fieldFormModel3.Value = Function.ToString(dataRow["Value"]);
							formOpenModel.Fields.Add(fieldFormModel3);
						}
					}
					num++;
					orderOutputModel.OrderItems.Add(productModel2);
					orderModel.Products.Add(productModel);
				}
			}
			orderOutputModel.CreateOrderRequest = orderModel;
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Order = orderOutputModel
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult ConfirmOrder([FromBody] OrderInput Order)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			string text = UnitOfWork.MobileDashboard.ConfirmOrder(Order.OrderId);
			if (string.IsNullOrEmpty(text))
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "0"
				};
			}
			return new ResponseResult
			{
				Message = text,
				MessageCode = _messageCodePrefix + "504"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult CancelOrder([FromBody] OrderInput Order)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			int docEntry = Function.ParseInt(Order.OrderId);
			HttpResult httpResult = BrokerService.GetUnitOfWork(SessionId).Documents.CancelDoc(docEntry, SessionUserInfo.SessionDiServer, ApzObjectTypes.oQuotations);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "0"
				};
			}
			return new ResponseResult
			{
				Message = httpResult.Message,
				MessageCode = _messageCodePrefix + "502"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult GetCustomerOrders()
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			List<CustomerOrder> list = new List<CustomerOrder>();
			DataTable customerOrders = UnitOfWork.MobileDashboard.GetCustomerOrders(SessionUserInfo.UserName);
			if (customerOrders.IsNotNull())
			{
				string text = "0";
				for (int i = 0; i < customerOrders.Rows.Count; i++)
				{
					text = Function.ToString(customerOrders.Rows[i]["DocEntry"]);
					CustomerOrder customerOrder = new CustomerOrder();
					customerOrder.Lines = new List<LineModel>();
					customerOrder.OrderId = text;
					customerOrder.Title = Function.ToString(customerOrders.Rows[i]["Title"]);
					customerOrder.Status = Function.ToString(customerOrders.Rows[i]["Status"]);
					customerOrder.Editable = Function.ToString(customerOrders.Rows[i]["Editable"]);
					customerOrder.Cancelable = Function.ToString(customerOrders.Rows[i]["Cancelable"]);
					for (int j = 5; j < customerOrders.Columns.Count; j++)
					{
						LineModel lineModel = new LineModel();
						lineModel.Label = customerOrders.Columns[j].ColumnName;
						lineModel.Value = Function.ToString(customerOrders.Rows[i][j]);
						customerOrder.Lines.Add(lineModel);
					}
					list.Add(customerOrder);
				}
			}
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Orders = list
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public ResponseResult GetCustomerOrderDetail([FromBody] OrderInput Order)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			OrderOutputModel orderOutputModel = new OrderOutputModel();
			DataSet dataSet = new DataSet();
			orderOutputModel.OrderId = Order.OrderId;
			dataSet = UnitOfWork.MobileDashboard.GetOrderRequest(Function.ParseInt(Order.OrderId), Order.DashboardItem);
			if (dataSet.IsNotNull())
			{
				orderOutputModel.Title = Function.ToString(dataSet.Tables[0].Rows[0]["Title"]);
				orderOutputModel.Status = Function.ToString(dataSet.Tables[0].Rows[0]["Status"]);
				orderOutputModel.Editable = Function.ToString(dataSet.Tables[0].Rows[0]["Editable"]);
				orderOutputModel.Cancelable = Function.ToString(dataSet.Tables[0].Rows[0]["Cancelable"]);
			}
			orderOutputModel.OrderHeader = new ProductModel();
			orderOutputModel.OrderHeader.ProductId = Order.OrderId;
			orderOutputModel.OrderHeader.Title = Function.ToString(dataSet.Tables[0].Rows[0]["Title"]);
			orderOutputModel.OrderHeader.Date = Function.ToString(dataSet.Tables[0].Rows[0]["CreateDate"]);
			orderOutputModel.OrderHeader.Lines = new List<FieldFormModel>();
			if (dataSet != null && dataSet.Tables.Count > 2 && dataSet.Tables[0].IsNotNull())
			{
				for (int i = 3; i < dataSet.Tables[0].Columns.Count; i++)
				{
					FieldFormModel fieldFormModel = new FieldFormModel();
					fieldFormModel.Label = dataSet.Tables[0].Columns[i].ColumnName;
					fieldFormModel.Value = Function.ToString(dataSet.Tables[0].Rows[0][i]);
					orderOutputModel.OrderHeader.Lines.Add(fieldFormModel);
				}
			}
			orderOutputModel.OrderItems = new List<ProductModel>();
			OrderModel orderModel = new OrderModel();
			orderModel.OrderId = Order.OrderId;
			orderModel.Products = new List<ProductModel>();
			if (dataSet != null && dataSet.Tables.Count > 2 && dataSet.Tables[1].IsNotNull())
			{
				var enumerable = from t in dataSet.Tables[1].AsEnumerable()
					group t by new
					{
						LineNum = Function.ToString(t["LineNum"])
					} into g
					select new
					{
						Key = g.Key,
						Items = g
					};
				int num = 0;
				foreach (var item in enumerable)
				{
					ProductModel productModel = new ProductModel();
					productModel.OrderRequest = new FormOpenModel();
					productModel.OrderRequest.Fields = new List<FieldFormModel>();
					productModel.AddedServiceRequests = new List<FormOpenModel>();
					ProductModel productModel2 = new ProductModel
					{
						Lines = new List<FieldFormModel>()
					};
					foreach (DataRow item2 in item.Items)
					{
						if (Function.ToString(item2["Name"]).Equals("ShipDate"))
						{
							productModel.Date = Function.ToString(item2["Value"]);
							productModel2.Date = Function.ToString(item2["Value"]);
						}
						if (Function.ToString(item2["Name"]).Equals("ItemCode"))
						{
							productModel.ProductId = Function.ToString(item2["Value"]);
							productModel2.ProductId = Function.ToString(item2["Value"]);
							continue;
						}
						if (Function.ToString(item2["Name"]).Equals("Dscription"))
						{
							productModel.Title = Function.ToString(item2["Value"]);
							productModel2.Title = Function.ToString(item2["Value"]);
							continue;
						}
						if (Function.ToString(item2["Name"]).Equals("Quantity"))
						{
							productModel.Quantity = Function.ToString(item2["Value"]);
							productModel2.Quantity = Function.ToString(item2["Value"]);
							continue;
						}
						FieldFormModel fieldFormModel2 = new FieldFormModel();
						fieldFormModel2.Name = Function.ToString(item2["Name"]);
						fieldFormModel2.Label = Function.ToString(item2["Label"]);
						fieldFormModel2.DefaultValue = Function.ToString(item2["DefaultValue"]);
						fieldFormModel2.DataType = Function.ToString(item2["DataType"]);
						fieldFormModel2.Value = item2["Value"];
						productModel.OrderRequest.Fields.Add(fieldFormModel2);
						productModel2.Lines.Add(fieldFormModel2);
					}
					foreach (var item3 in from t in dataSet.Tables[2].Select("LineNum = " + num)
						group t by new
						{
							LineId = Function.ToString(t["LineId"])
						} into g
						select new
						{
							Key = g.Key,
							Items = g
						})
					{
						FormOpenModel formOpenModel = new FormOpenModel
						{
							Fields = new List<FieldFormModel>()
						};
						foreach (DataRow item4 in item3.Items)
						{
							FieldFormModel fieldFormModel3 = new FieldFormModel();
							fieldFormModel3.Name = Function.ToString(item4["Name"]);
							fieldFormModel3.OrderNo = Function.ToString(item4["LineId"]);
							fieldFormModel3.Label = Function.ToString(item4["Label"]);
							fieldFormModel3.DefaultValue = Function.ToString(item4["DefaultValue"]);
							fieldFormModel3.DataType = Function.ToString(item4["DataType"]);
							fieldFormModel3.Value = Function.ToString(item4["Value"]);
							formOpenModel.Fields.Add(fieldFormModel3);
						}
						productModel.AddedServiceRequests.Add(formOpenModel);
					}
					num++;
					orderOutputModel.OrderItems.Add(productModel2);
					orderModel.Products.Add(productModel);
				}
			}
			orderOutputModel.CreateOrderRequest = orderModel;
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Order = orderOutputModel
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	private object GetDataByDataType(string dataType, string value)
	{
		switch (dataType)
		{
		case "text":
		case "list":
			return value;
		case "integer":
			return Function.ParseInt(value);
		case "float":
			return Function.ParseFloat(value);
		case "date":
		{
			if (DateTime.TryParseExact(value, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var result))
			{
				return result;
			}
			return Function.ParseDateTimes(value);
		}
		case "time":
			return string.Format("{0000}", value.Replace(":", ""));
		default:
			return DBNull.Value;
		}
	}

	[HttpPost]
	public ResponseResult GetServiceUserDeliveryOrder([FromBody] OrderInput DeliveryOrder)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			List<DeliveryOrder> list = new List<DeliveryOrder>();
			DataTable serviceUserDeliveryOrder = UnitOfWork.MobileDashboard.GetServiceUserDeliveryOrder(SessionUserInfo.UserName, DeliveryOrder.Status);
			if (serviceUserDeliveryOrder.IsNotNull())
			{
				for (int i = 0; i < serviceUserDeliveryOrder.Rows.Count; i++)
				{
					DeliveryOrder deliveryOrder = new DeliveryOrder();
					deliveryOrder.Lines = new List<LineModel>();
					deliveryOrder.DeliveryOrderId = Function.ToString(serviceUserDeliveryOrder.Rows[i]["DocEntry"]);
					deliveryOrder.Title = Function.ToString(serviceUserDeliveryOrder.Rows[i]["Title"]);
					deliveryOrder.Status = Function.ToString(serviceUserDeliveryOrder.Rows[i]["Status"]);
					for (int j = 3; j < serviceUserDeliveryOrder.Columns.Count; j++)
					{
						LineModel lineModel = new LineModel();
						lineModel.Label = serviceUserDeliveryOrder.Columns[j].ColumnName;
						lineModel.Value = Function.ToString(serviceUserDeliveryOrder.Rows[i][j]);
						deliveryOrder.Lines.Add(lineModel);
					}
					list.Add(deliveryOrder);
				}
			}
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					DeliveryOrders = list
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "207"
			};
		}
	}

	[HttpPost]
	public ResponseResult GetServiceUserDeliOrdersDetail([FromBody] OrderInput DeliveryOrder)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			string deliveryOrderId = DeliveryOrder.DeliveryOrderId;
			DeliveryOrder deliveryOrder = new DeliveryOrder();
			DataSet serviceUserDeliOrdersDetail = UnitOfWork.MobileDashboard.GetServiceUserDeliOrdersDetail(deliveryOrderId);
			if (serviceUserDeliOrdersDetail.IsHasStructure())
			{
				DataTable dataTable = serviceUserDeliOrdersDetail.Tables[0];
				DataTable dataTable2 = serviceUserDeliOrdersDetail.Tables[1];
				deliveryOrder.DeliveryOrderId = Function.ToString(dataTable.Rows[0]["DocEntry"]);
				deliveryOrder.Title = Function.ToString(dataTable.Rows[0]["Title"]);
				deliveryOrder.Status = Function.ToString(dataTable.Rows[0]["Status"]);
				deliveryOrder.OrderHeader = new DeliHeader();
				deliveryOrder.OrderHeader.Lines = new List<LineModel>();
				for (int i = 3; i < dataTable.Columns.Count; i++)
				{
					LineModel lineModel = new LineModel();
					lineModel.Label = dataTable.Columns[i].ColumnName;
					lineModel.Value = Function.ToString(dataTable.Rows[0][i]);
					deliveryOrder.OrderHeader.Lines.Add(lineModel);
				}
				deliveryOrder.OrderItems = new List<DeliItem>();
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					DeliItem deliItem = new DeliItem();
					deliItem.ProductId = Function.ToString(dataTable2.Rows[0]["ItemCode"]);
					deliItem.Title = Function.ToString(dataTable2.Rows[0]["Title"]);
					deliItem.Quantity = Function.ToString(dataTable2.Rows[0]["Quantity"]);
					deliItem.Date = Function.ToString(dataTable2.Rows[0]["ShipDate"]);
					deliItem.Lines = new List<LineModel>();
					for (int k = 4; k < dataTable2.Columns.Count; k++)
					{
						LineModel lineModel2 = new LineModel();
						lineModel2.Label = dataTable2.Columns[k].ColumnName;
						lineModel2.Value = Function.ToString(dataTable2.Rows[j][k]);
						deliItem.Lines.Add(lineModel2);
					}
					deliveryOrder.OrderItems.Add(deliItem);
				}
			}
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					DeliveryOrder = deliveryOrder
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "207"
			};
		}
	}

	[HttpPost]
	public ResponseResult UpdateDeliveryOrderStatus([FromBody] OrderInput DeliveryOrder)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			return new ResponseResult
			{
				Message = UnitOfWork.MobileDashboard.UpdateDeliveryOrderStatus(DeliveryOrder.DeliveryOrderId, DeliveryOrder.Status),
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "207"
			};
		}
	}

	[HttpPost]
	public ResponseResult GetDashboadDeliveryOrder()
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			List<DeliveryOrder> list = new List<DeliveryOrder>();
			DataTable serviceUserDeliveryOrder = UnitOfWork.MobileDashboard.GetServiceUserDeliveryOrder(SessionUserInfo.UserName, "");
			if (serviceUserDeliveryOrder.IsNotNull())
			{
				for (int i = 0; i < serviceUserDeliveryOrder.Rows.Count; i++)
				{
					DeliveryOrder deliveryOrder = new DeliveryOrder();
					deliveryOrder.Lines = new List<LineModel>();
					deliveryOrder.DeliveryOrderId = Function.ToString(serviceUserDeliveryOrder.Rows[i]["DocEntry"]);
					deliveryOrder.Title = Function.ToString(serviceUserDeliveryOrder.Rows[i]["Title"]);
					deliveryOrder.Status = Function.ToString(serviceUserDeliveryOrder.Rows[i]["Status"]);
					for (int j = 3; j < serviceUserDeliveryOrder.Columns.Count; j++)
					{
						LineModel lineModel = new LineModel();
						lineModel.Label = serviceUserDeliveryOrder.Columns[j].ColumnName;
						lineModel.Value = Function.ToString(serviceUserDeliveryOrder.Rows[i][j]);
						deliveryOrder.Lines.Add(lineModel);
					}
					list.Add(deliveryOrder);
				}
			}
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					DeliveryOrders = list
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "207"
			};
		}
	}

	[HttpGet]
	public DataTable GetServiceDashboard(string userName)
	{
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetListDashboard()
	{
		return UnitOfWork.DashBoard.GetListDashboard();
	}

	[HttpGet]
	public List<int> GetListDashboardByUser(string userName)
	{
		return UnitOfWork.DashBoard.GetListDashboardByUser(userName);
	}
}
