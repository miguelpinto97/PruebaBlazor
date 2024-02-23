using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace RestauranteVirtual.Web.Utils
{
	public class MessageValidatorBase<TValue> : ComponentBase, IDisposable
	{
		private FieldIdentifier _fieldIdentifier;
		private EventHandler<ValidationStateChangedEventArgs> _stateChangedHandler
			=> (sender, args) => StateHasChanged();

		[CascadingParameter]
		private EditContext EditContext { get; set; }
		[Parameter]
		public Expression<Func<TValue>>? For { get; set; }
		[Parameter]
		public string? ForCustom { get; set; }
		[Parameter]
		public int? ForIndex { get; set; }
		[Parameter]
		public string Class { get; set; }
		private string FieldName { get; set; }

		protected IEnumerable<string> ValidationMessages { 
			get 
			{
				return EditContext.GetValidationMessages(_fieldIdentifier);
			} 
		}

		protected override void OnInitialized()
		{
			if (!string.IsNullOrEmpty(ForCustom))
			{
				FieldName = ForCustom;
				if(ForIndex != null)
					FieldName = string.Format(ForCustom, ForIndex);
				_fieldIdentifier = EditContext.Field(FieldName);
			}
			else
			{
				_fieldIdentifier = FieldIdentifier.Create(For);
			}

			EditContext.OnValidationStateChanged += _stateChangedHandler;
		}

		public void Dispose()
		{
			EditContext.OnValidationStateChanged -= _stateChangedHandler;
		}
	}
}
