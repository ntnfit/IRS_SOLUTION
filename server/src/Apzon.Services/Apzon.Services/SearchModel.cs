namespace Apzon.Services;

public class SearchModel
{
	private string _code;

	private string _content;

	public string Content
	{
		get
		{
			return _content ?? string.Empty;
		}
		set
		{
			_content = (string.IsNullOrEmpty(value) ? string.Empty : value.Trim().ToLower());
		}
	}

	public string Code
	{
		get
		{
			return _code ?? string.Empty;
		}
		set
		{
			_code = (string.IsNullOrEmpty(value) ? string.Empty : value.Trim().ToLower());
		}
	}

	public int Id { get; set; }

	public int Status { get; set; }

	public bool Valid { get; set; }

	public PageModel Pager { get; set; }
}
