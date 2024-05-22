using MauiReactor;
using Rearch.Reactor.Example.Models;
using Rearch.Reactor.Components;
using static Rearch.Reactor.Example.Capsules.TodoCapsules;
using Rearch.Reactor.Example.Pages;
using System;
using MauiReactor.Animations;

namespace Rearch.Reactor.Example.Components;

partial class Body : CapsuleConsumer
{
    public override VisualNode Render(ICapsuleHandle use)
    {
        const double SearchBarHeight = 50d;
        const double AnimationDurationMillis = 125d;

        var (
            filter,
            _,
            toggleCompletionStatus) =
            use.Invoke(FilterCapsule);
        var completionStatus = filter.CompletionStatus;

        var (AddTodo, _, _) = use.Invoke(TodoItemsManagerCapsule);

        var (animationHeight, setAnimationHeight) = use.State(0d);

        var (isSearching, setIsSearching) = use.State(false);

        return NavigationPage(
            ContentPage(
                ToolbarItem(completionStatus ?
                    "Complete" :
                    "Incomplete")
                .OnClicked(toggleCompletionStatus),

                ToolbarItem("Search")
                .OnClicked(() => setIsSearching(!isSearching)),

                ToolbarItem("Create")
                .OnClicked(() => ShowCreateTodoDialogAsync(
                    ContainerPage, AddTodo)),

                Grid("Auto, *", "*",
                    new SearchBar(
                        height: animationHeight,
                        close: () => setIsSearching(false)),

                    new TodoList()
                    .GridRow(1),

                    new AnimationController
                    {
                        new SequenceAnimation
                        {
                            new DoubleAnimation()
                                .StartValue(isSearching ? 0 : SearchBarHeight)
                                .TargetValue(isSearching ? SearchBarHeight : 0)
                                .Duration(AnimationDurationMillis)
                                .OnTick(setAnimationHeight),
                        }
                    }
                    .IsEnabled(
                        isSearching && animationHeight < SearchBarHeight ||
                        !isSearching && animationHeight > 0)
                )
            )
            .Title("rearch todos")
        );

        void ShowCreateTodoDialogAsync(
            MauiControls.Page? containerPage,
            Action<Todo> todoCreator)
        {
            containerPage?.Navigation.PushModalAsync<
                CreateTodoPage,
                CreateTodoPageProps>(
                p => p.TodoCreator = todoCreator);
        }
    }
}
