// <copyright file="RearchConsumer{TProps}.cs" company="SdgApps">
// Copyright (c) SdgApps. All rights reserved.
// </copyright>

namespace Rearch.Reactor.Components;

using MauiReactor;
using MauiReactor.Parameters;

/// <summary>
/// A <see cref="Component"/> that has access to a
/// <see cref="ICapsuleHandle"/>, and can consequently consume
/// <see cref="Capsule{T}"/>s and <see cref="SideEffect{T}"/>s.
/// </summary>
/// <typeparam name="TProps">Type of component props.</typeparam>
public abstract partial class RearchConsumer<TProps> : Component<object, TProps>
    where TProps : class, new()
{
    [Param]
    private readonly IParameter<CapsuleContainerParameter> containerParameter;

    /// <summary>
    /// Gets set of callbacks to execute <see cref="Component.OnWillUnmount"/>.
    /// </summary>
    internal HashSet<SideEffectApiCallback> UnmountListeners { get; } = [];

    /// <summary>
    /// Gets data of registered side effects.
    /// </summary>
    internal List<object?> SideEffectData { get; } = [];

    /// <summary>
    /// Gets a <see cref="HashSet{T}"/> of functions that remove a dependency on
    /// a <see cref="Capsule{T}"/>.
    /// </summary>
    internal HashSet<Action> DependencyDisposers { get; } = [];

    /// <inheritdoc/>
    public sealed override VisualNode Render()
    {
        // Clears the old dependencies (which will be repopulated via
        // WidgetHandle)
        this.ClearDependencies();

        var container = this.containerParameter.Value.Container;
        return container == null ?
            throw new InvalidOperationException(
                $"No {typeof(CapsuleContainerProvider<>).Name} found in the " +
                $"component tree!\nDid you forget to add " +
                $"{nameof(MauiAppBuilderExtensions.UseRearchReactorApp)} to " +
                $"MauiProgram?") :
            this.Render(
                new ComponentHandle<TProps>(
                    new ComponentSideEffectApiProxy<TProps>(this),
                    container));
    }

    /// <summary>
    /// Render with ability to consume <see cref="Capsule{T}"/>s and
    /// <see cref="SideEffect{T}"/>s.
    /// </summary>
    /// <param name="use">Component handle.</param>
    /// <returns>Rendered node.</returns>
    public abstract VisualNode Render(ICapsuleHandle use);

    /// <summary>
    /// Invalidates component, triggering re-render.
    /// </summary>
    internal new void Invalidate() => base.Invalidate();

    /// <inheritdoc/>
    protected override void OnWillUnmount()
    {
        foreach (var listener in this.UnmountListeners)
        {
            listener();
        }

        this.ClearDependencies();

        // Clean up after any side effects to avoid possible leaks
        this.UnmountListeners.Clear();

        base.OnWillUnmount();
    }

    /// <summary>
    /// Clears out the <see cref="Capsule{T}"/> dependencies of this
    /// <see cref="RearchConsumer"/>.
    /// </summary>
    private void ClearDependencies()
    {
        foreach (var dispose in this.DependencyDisposers)
        {
            dispose();
        }

        this.DependencyDisposers.Clear();
    }
}
