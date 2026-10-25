using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IHttpClientHelper<TEntity> where TEntity : class
{
	Task<TEntity> GetAsync(string action, object pr);

	Task<TEntity> GetAsync(string action);

	Task<List<TEntity>> GetListAsyncByPr(string action, object pr);

	Task<List<TEntity>> GetListAsync(string action);

	Task<List<ExpandoObject>> PostListDynamic(string action, TEntity obj);

	Task<string> GetStringAsync(string action, object pr);

	Task<string> GetStringAsync(string action);

	Task<int> GetIntAsync(string action, object pr);

	Task<bool> GetBoolAsync(string action, object pr);

	Task<int> GetIntAsync(string action);

	Task<DataTable> GetDataTableAsync(string action, object pr);

	Task<DataTable> GetDataTableAsync(string action);

	Task<DataTable> GetDataTableAsJsonAsync(string action, object obj);

	Task<string> PostAsJsonAsync(string action, TEntity entity);

	Task<string> PostParamsAsJsonAsync(string action, object obj);

	Task<string> PostAsJsonAsync(string action, List<TEntity> entities);

	Task<HttpResult> GetHttpResult(string action, object pr);

	string GetUri(string action, object pr);

	string GetUri(string action);

	string GetUri(string action, Type type, ref bool isParseData);

	void HandleHttpResponseMessage(HttpResponseMessage result);

	Task<TEntity> PostAsync(string action, object pr, string sessionId = "");

	TEntity Get(string action, object pr);

	TEntity Get(string action);

	List<TEntity> GetListByPr(string action, object pr);

	List<TEntity> GetList(string action);

	List<TEntity> GetList(string action, object pr);

	string GetString(string action, object pr);

	string GetString(string action);

	int GetInt(string action, object pr);

	bool GetBool(string action, object pr);

	int GetInt(string action);

	DataTable GetDataTable(string action, object pr);

	DataTable GetDataTable(string action);

	DataTable GetDataTableAsJson(string action, object obj);

	string PostAsJson(string action, TEntity entity);

	string PostParamsAsJson(string action, object obj);

	string PostAsJson(string action, List<TEntity> entities);

	TEntity Post(string action, object pr);

	HttpResult GetResult(string action, object pr);
}
