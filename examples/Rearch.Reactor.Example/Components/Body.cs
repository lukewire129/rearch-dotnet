using MauiReactor;
using Rearch.Reactor.Example.Models;
using Rearch.Reactor.Components;
using static Rearch.Reactor.Example.Capsules.TodoCapsules;
using Rearch.Reactor.Example.Pages;
using System;
using System.Linq;
using MauiReactor.Animations;

namespace Rearch.Reactor.Example.Components;

partial class Body : CapsuleConsumer
{
    public override VisualNode Render(ICapsuleHandle use)
    {
        var (isSearching, setIsSearching) = use.State(false);
        var (searchHeight, setSearchHeight) = use.State(0d);

        var (AddTodo, _, _) = use.Invoke(TodoItemsManagerCapsule);

        var (
            filter,
            _,
            toggleCompletionStatus) =
            use.Invoke(FilterCapsule);
        var completionStatus = filter.CompletionStatus;

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
                        height: searchHeight,
                        close: () => setIsSearching(false)),

                    new TodoList()
                    .GridRow(1),

                    new AnimationController
                    {
                        new SequenceAnimation
                        {
                            new DoubleAnimation()
                                .StartValue(isSearching ? 0 : 50)
                                .TargetValue(isSearching ? 50 : 0)
                                .Duration(1000)
                                .OnTick(setSearchHeight),
                        }
                    }
                    .IsEnabled(
                        isSearching && searchHeight < 50 ||
                        !isSearching && searchHeight > 0)
                )
            )
            .Title("rearch todos")
        );

        void ShowCreateTodoDialogAsync(
            MauiControls.Page? containerPage,
            Action<Todo> todoCreator)
        {
            containerPage?.Navigation.PushModalAsync<CreateTodoPage, CreateTodoPageProps>(p => p.TodoCreator = todoCreator);
        }
    }
}
