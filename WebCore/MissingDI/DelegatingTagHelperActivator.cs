using System;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebCore.MissingDI
{
    internal sealed class DelegatingTagHelperActivator : ITagHelperActivator
    {
        private readonly Predicate<Type> customCreatorSelector;
        private readonly Func<Type, object> customTagHelperCreator;
        private readonly ITagHelperActivator defaultTagHelperActivator;

        public DelegatingTagHelperActivator(Predicate<Type> customCreatorSelector, Func<Type, object> customTagHelperCreator,
            ITagHelperActivator defaultTagHelperActivator)
        {
            if (customCreatorSelector == null) throw new ArgumentNullException(nameof(customCreatorSelector));
            if (customTagHelperCreator == null) throw new ArgumentNullException(nameof(customTagHelperCreator));
            if (defaultTagHelperActivator == null) throw new ArgumentNullException(nameof(defaultTagHelperActivator));

            this.customCreatorSelector = customCreatorSelector;
            this.customTagHelperCreator = customTagHelperCreator;
            this.defaultTagHelperActivator = defaultTagHelperActivator;
        }

        public TTagHelper Create<TTagHelper>(ViewContext context) where TTagHelper : ITagHelper =>
            this.customCreatorSelector(typeof(TTagHelper))
                ? (TTagHelper)this.customTagHelperCreator(typeof(TTagHelper))
                : defaultTagHelperActivator.Create<TTagHelper>(context);
    }
}