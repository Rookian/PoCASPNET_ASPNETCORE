using System;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace WebCore.MissingDI
{
    public sealed class DelegatingViewComponentActivator : IViewComponentActivator
    {
        private readonly Func<Type, object> viewComponentCreator;
        private readonly Action<object> viewComponentReleaser;

        public DelegatingViewComponentActivator(Func<Type, object> viewComponentCreator,
            Action<object> viewComponentReleaser = null)
        {
            if (viewComponentCreator == null) throw new ArgumentNullException(nameof(viewComponentCreator));

            this.viewComponentCreator = viewComponentCreator;
            this.viewComponentReleaser = viewComponentReleaser ?? (_ => { });
        }

        public object Create(ViewComponentContext context) =>
            this.viewComponentCreator(context.ViewComponentDescriptor.TypeInfo.AsType());

        public void Release(ViewComponentContext context, object viewComponent) =>
            this.viewComponentReleaser(viewComponent);
    }
}