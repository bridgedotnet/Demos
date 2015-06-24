using Bridge;
using Bridge.Html5;

namespace Demo
{
	public class App
	{
		[Ready]
		public static void Main()
		{
			var button = new ButtonElement
			{
				InnerHTML = "Submit",
				OnClick = (ev) =>
				{
					Global.Alert("Welcome to Bridge.NET");
				}
			};
			
			Document.Body.AppendChild(button);		
		}
	}
}