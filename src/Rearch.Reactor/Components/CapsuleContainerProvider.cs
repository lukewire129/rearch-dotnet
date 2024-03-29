// <copyright file="CapsuleContainerProvider.cs" company="SdgApps">
// Copyright (c) SdgApps. All rights reserved.
// </copyright>

namespace Rearch.Reactor.Components;

using MauiReactor;
using MauiReactor.Parameters;

/// <summary>
/// Provides a <see cref="CapsuleContainer"/> to the rest of the
/// <see cref="Component"/> tree using an <see cref="IParameter"/>.
/// </summary>
/// <typeparam name="TComponent">Type of root component.</typeparam>
/// <remarks>
/// Does not manage the lifecycle of the supplied
/// <see cref="CapsuleContainer"/>. You typically should
/// <see cref="MauiAppBuilderExtensions.UseRearchReactorApp"/> instead.
/// </remarks>
internal sealed partial class CapsuleContainerProvider<TComponent> : Component
    where TComponent : CapsuleConsumer, new()
{
    [Param]
    private readonly IParameter<CapsuleContainerParameter> containerParameter;

    /// <inheritdoc/>
    public override VisualNode Render()
    {
        return new TComponent();
    }

    /// <inheritdoc/>
    protected override void OnMounted()
    {
        base.OnMounted();

        this.containerParameter.Set(p => p.Container = new CapsuleContainer());
    }

    /// <inheritdoc/>
    protected override void OnWillUnmount()
    {
        base.OnWillUnmount();
        this.containerParameter.Value.Container?.Dispose();
    }
}
