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
    private enum SearchState
    {
        Idle,
        Searching,
        Search,
        Idling
    }

    public override VisualNode Render(ICapsuleHandle use)
    {
        var (searchState, setSearchState) = use.State(SearchState.Idle);
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
                .OnClicked(() => setSearchState(
                    searchState == SearchState.Idle ? SearchState.Searching :
                    searchState == SearchState.Searching ? SearchState.Idling :
                    searchState == SearchState.Search ? SearchState.Idling :
                    SearchState.Searching)),

                ToolbarItem("Create")
                .OnClicked(() => ShowCreateTodoDialogAsync(
                    ContainerPage, AddTodo)),

                Grid("Auto, *", "*",
                    new SearchBar(
                        height: searchHeight,
                        close: () => setSearchState(
                            searchState == SearchState.Idle ? SearchState.Searching :
                            searchState == SearchState.Searching ? SearchState.Idling :
                            searchState == SearchState.Search ? SearchState.Idling :
                            SearchState.Searching)),

                    new TodoList()
                    .GridRow(1),

                    new AnimationController
                    {
                        new SequenceAnimation
                        {
                            new DoubleAnimation()
                                .StartValue(searchState == SearchState.Idling ? 50 : 0)
                                .TargetValue(searchState == SearchState.Idling ? 0 : 50)
                                .Duration(1000)
                                .OnTick(v =>
                                {
                                    setSearchHeight(v);
                                    switch (searchState)
                                    {
                                        case SearchState.Searching:
                                            if (v == 50)
                                            {
                                                setSearchState(SearchState.Search);
                                            }

                                            break;

                                        case SearchState.Idling:
                                            if (v == 0)
                                            {
                                                setSearchState(SearchState.Idle);
                                            }

                                            break;
                                    }
                                }),
                        }
                    }
                    .IsEnabled(searchState == SearchState.Searching || searchState == SearchState.Idling)
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
