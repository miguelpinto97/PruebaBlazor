using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RestauranteVirtual.Web.Utils;

namespace RestauranteVirtual.Web.Components.Validacion
{
    public partial class CustomFormValidator : ComponentBase, IDisposable
    {
        private ValidationMessageStore validationMessageStore;
        public IDictionary<string, string[]> Errores;

		[CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
			if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(CustomFormValidator)} requires a cascading parameter of type {nameof(EditContext)}.");
            }

            validationMessageStore = new ValidationMessageStore(CurrentEditContext);
			CurrentEditContext.SetFieldCssClassProvider(new ValidationFieldClassProvider("", "is-invalid"));
			CurrentEditContext.OnValidationRequested += (s, e) => validationMessageStore.Clear();
			CurrentEditContext.OnFieldChanged += HandleFieldChanged;
		}

		private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
		{
            validationMessageStore.Clear(e.FieldIdentifier);
		}

		public void Dispose()
		{
			CurrentEditContext.OnFieldChanged -= HandleFieldChanged;
		}


		public void DisplayFormErrors(IDictionary<string, string[]> errors)
        {
            foreach (var err in errors)
            {
                validationMessageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
            }
            Errores = errors;
			CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ClearFormErrors()
        {
            validationMessageStore.Clear();
            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
